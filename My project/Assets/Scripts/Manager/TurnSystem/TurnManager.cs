using System;
using System.Collections.Generic;
using System.Linq;

public class TurnManager
{
    public enum ETurnType
    {
        USE_CARDTRUN,
        ATTACK_ACTIONTURN,
        END
    }

    public List<ITurnParticipant> _participants { get; private set; }

    public int CurrentTurnIndex     { get; private set; } = 0;   // 현재 턴인 참가자의 인덱스
    public int CurrentPhase         { get; private set; } = 1;        // 현재 진행 중인 Phase (1부터 시작)
    public bool IsRunning           { get; private set; } = false;
    public BattlePlayerData         LocalPlayer { get; private set; }
    public ETurnType                _TurnType { get; private set; }  = ETurnType.END;

    public ITurnParticipant Current => _participants[CurrentTurnIndex];
    public int ParticipantCount => _participants.Count;
   
    List<CharacterAction>               _AllPlayerAction = new List<CharacterAction>();

    public void Release()
    {
        EventBus.Unsubscribe<CardActionEvent>(OnCardActionAdd);
    }

    public IReadOnlyList<CharacterAction> GetAllHistory()
    {
        return _AllPlayerAction;
    }

    public bool IsPlayerTurn() { return LocalPlayer.IsActive; }

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

        _participants = participants;
        foreach (ITurnParticipant participant in participants)
        {
            var Base = participant as TurnParticipantBase;
            Base.RequestTurnEnd += RequestEndTurn;

            if (Base.IsLocal)
                LocalPlayer = Base as BattlePlayerData;
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
        EventBus.Publish<TurnUIEvent>(new TurnUIEvent(Current.Name, _TurnType));
        Current.TurnBegin();
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

        _TurnType++;
        if (_TurnType == ETurnType.END)
        {
            ExecuteEndTurn();
            _TurnType = ETurnType.USE_CARDTRUN;
        }

        return true;
    }

    private void ExecuteEndTurn()
    {
        Current.TurnEnd();
        CurrentTurnIndex++;

        if (CurrentTurnIndex >= _participants.Count)
        {
            CurrentTurnIndex = 0;
            CurrentPhase++;
        }

        StartTurn();
    }

    public void Stop()
    {
        IsRunning = false;
    }
}
