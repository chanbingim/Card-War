using System;
using System.Collections.Generic;
using Unity.Behavior;

[BlackboardEnum]
public enum ETurnType
{
    USE_CARDTRUN,
    ATTACK_ACTIONTURN,
    END
}

public class TurnManager
{
    public List<BattlePlayerData> _participants { get; private set; }

    public int CurrentTurnIndex     { get; private set; } = 0;   // 현재 턴인 참가자의 인덱스
    public int CurrentPhase         { get; private set; } = 1;   // 현재 진행 중인 Phase (1부터 시작)
    public bool IsRunning           { get; private set; } = false;

    public BattlePlayerData         LocalPlayer { get; private set; }
    public ETurnType                _TurnType { get; private set; }  = ETurnType.END;

    public BattlePlayerData Current => _participants[CurrentTurnIndex];
    public int ParticipantCount => _participants.Count;

    List<BattlePlayerData>              _GameOverList = new List<BattlePlayerData>();
    List<CharacterAction>               _AllPlayerAction = new List<CharacterAction>();

    public void Release()
    {
        foreach (var participant in _participants)
        {
            participant._OnGameOver -= GameOverParticipant;
        }

        EventBus.Unsubscribe<CardActionEvent>(OnCardActionAdd);
    }

    public IReadOnlyList<CharacterAction> GetAllHistory()
    {
        return _AllPlayerAction;
    }

    public bool IsPlayerTurn() { return LocalPlayer.IsActive; }

    public List<int> GetAlivePlayerIdxList()
    {
        List<int> idxs = new List<int>();

        int PlayerCount = _participants.Count;
        for (int i = 0; i < PlayerCount; i++)
        {
            if (_participants[i].IsAlive())
                idxs.Add(i);
        }

        return idxs;
    }

    public static TurnManager Create(List<ITurnParticipant> participants)
    {
        TurnManager instance = new TurnManager();
        if (instance.Initialize(participants) == false)
            return null;

        return instance;
    }

    public void ADDHistoryActionData(CharacterAction data)
    {
        _AllPlayerAction.Add(data);
    }

    private void OnCardActionAdd(CardActionEvent data)
    {
        ADDHistoryActionData(data.Action);
        EventBus.Publish<ActionRecordedEvent>(new ActionRecordedEvent(data.Action));
    }

    private bool Initialize(List<ITurnParticipant> participants)
    {
        if (participants == null || participants.Count == 0)
            throw new ArgumentException("참가자가 최소 1명 이상 필요합니다.");

        _participants = new List<BattlePlayerData>();

        foreach (ITurnParticipant participant in participants)
        {
            var Base = participant as BattlePlayerData;
            Base._OnGameOver += GameOverParticipant;

            if (Base.IsLocal)
                LocalPlayer = Base;

            Base.SetPlayerTurn(_participants.Count);
            _participants.Add(Base);
        }

        EventBus.Subscribe<CardActionEvent>(OnCardActionAdd);
        _TurnType = ETurnType.USE_CARDTRUN;
        return true;
    }

    public void Begin()
    {
        CurrentTurnIndex = 0;
        CurrentPhase = 1;
        IsRunning = true;

        StartTurn();
    }

    private void StartTurn()
    {
        _TurnType = ETurnType.USE_CARDTRUN;

        Current.TurnBegin();
        Current.TrunChange(_TurnType);

        bool IsLocal = LocalPlayer.IsActive;
        EventBus.Publish<ChangeTurnEvent>(new ChangeTurnEvent(IsLocal, () =>
        {
            EventBus.Publish<ChangeTurnActEvent>(new ChangeTurnActEvent(_TurnType, IsLocal));
            BattleManager.instance.RequestDraw(GAME_CONST.Const.DRAW_CARDCOUNT);
        }));
    }

    public void Update()
    {
        if (Current == null)
            return;

        Current.TurnRunning();
    }

    /// 현재 참가자의 턴을 종료하고 다음 참가자로 넘김.
    /// 마지막 참가자였다면 Phase 완료 처리 후 다음 Phase 시작.
    public bool RequestEndTurn(string participantId)
    {
        if (!IsRunning)
            return false;

        if (Current.Name != participantId)
        {
            Console.WriteLine($"거부: {participantId}는 지금 턴이 아님 (현재 턴: {Current.Name})");
            return false;
        }

        if (!Current.IsActive)
        {
            Console.WriteLine($"거부: {participantId}는 비활성 상태");
            return false;
        }

        System.Action OnCompleted = null;
        if (_TurnType < ETurnType.END)
        {
            _TurnType++;
            Current.TrunChange(_TurnType);

            if (_TurnType >= ETurnType.END)
            {
                OnCompleted = () =>
                {
                    _TurnType = ETurnType.USE_CARDTRUN;
                    ExecuteEndTurn();
                };
            }

            EventBus.Publish<ChangeTurnActEvent>(new ChangeTurnActEvent(_TurnType, LocalPlayer.IsActive, OnCompleted));
        }

        return true;
    }

    private void ExecuteEndTurn()
    {
        Current.TurnEnd();
        while (true)
        {
            CurrentTurnIndex++;
            if (CurrentTurnIndex >= _participants.Count)
            {
                CurrentPhase++;
                CurrentTurnIndex = 0;
                BattleManager.instance.MakeMatchMaking();
            }

            if (!_GameOverList.Contains(Current))
                break;
        }

        StartTurn();
    }

    private void GameOverParticipant(BattlePlayerData player)
    {
        _GameOverList.Add(player);
        if(_GameOverList.Count == _participants.Count - 1)
        {
            // 여기서 GameOver순서로 등수 주고
            // 승리 실패 이거만 나누면 될듯
            var eGameMode = GameManager.instance.EGameMode;
            if (eGameMode == GameMode.SinglePlayer)
            {
                bool IsWin = !_GameOverList.Contains(LocalPlayer);
                EventBus.Publish<BattleEndEvent>(new BattleEndEvent(IsWin, 3));
            }
            else
            {
                //멀티 서버는 서버에 요청
                // 또는 순위
                UnityEngine.Debug.Log($"GameOver {player.Name}");
            }
        }
    }

    public void Stop()
    {
        IsRunning = false;
    }
}
