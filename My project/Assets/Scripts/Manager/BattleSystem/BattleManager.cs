using System;
using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private BattleCardManager        _BattleCardManager = null;
    private TurnManager              _TrunMgr = null;
    private Stage                    _Cur_Stage = null;

    public void Update()
    {
        if (_TrunMgr.IsRunning == false)
            _TrunMgr.Begin();
        else
            _TrunMgr.Update();
    }

    #region PlayerMgr
    public bool             IsPlayerTurn() { return _BattleCardManager?.IsPlayerTurn() ?? false; }
    public BattlePlayerData GetLoaclPlayer() { return _BattleCardManager?.LocalPlayer; }
    public UI_CardData      DrawCard() { return _BattleCardManager?.Draw_Card() ?? null; }
    #endregion

    #region Defualt
    static public BattleManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        instance.Initialize();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void Initialize()
    {
        if (InitStage() == false)
        {
            Debug.LogWarning("Initialize Fail Stage");
            return;
        }

        if (InitPlayerManager() == false)
        {
            Debug.LogWarning("Initialize Fail PlayerManager");
            return;
        }

        // 이거 나중에 서버에서 받아오긴할거임
        List<ITurnParticipant> participants = new List<ITurnParticipant>();
        participants.Add(_BattleCardManager.LocalPlayer);

        AddressableManager AddressableMgr = AddressableManager.instance;
        if (AddressableMgr == null)
            throw new ArgumentException("어드레서블 매니저 생성 필요");

        var Fomation = AddressableMgr.Get<FormationSO>("SO/Formation/ThreeFormation");
        for (int i = 1; i < 2; i++)
        {
            var player = new BattlePlayerData();
            player.SetName($"Player {i}");

            if (Fomation != null)
            {
                player.Request_ADDParty(3, _Cur_Stage.GetPlayerWorldPosition(Fomation.LocalPosition[0]));
                player.Request_ADDParty(4, _Cur_Stage.GetPlayerWorldPosition(Fomation.LocalPosition[1]));
            }
            else
            {
                player.Request_ADDParty(3);
                player.Request_ADDParty(4);
            }

            participants.Add(player);
        }

        if (InitTurnManager(participants) == false)
        {
            Debug.LogWarning("Initialize Fail TurnManager");
            return;
        }

        Debug.LogWarning("Initialize Complelted BattleManager");
    }

    private bool InitPlayerManager()
    {
        _BattleCardManager = BattleCardManager.Create(_Cur_Stage);
        return _BattleCardManager != null ? true : false;
    }

    private bool InitTurnManager(List<ITurnParticipant> participants)
    {
        _TrunMgr = TurnManager.Create(participants);
        return _TrunMgr != null ? true : false;
    }

    private bool InitStage()
    {
        int stageIdx = GameManager.instance.StageIndex;
        var AddressableMgr = AddressableManager.instance;

        if (AddressableMgr == null)
            return false;

        GameObject stagePrefab = AddressableMgr.Get<GameObject>($"Stage/Stage{stageIdx}");
        if (stagePrefab == null)
        {
            Debug.LogWarning($"Not Find Stage{stageIdx} Asset ");
            return false;
        }

        var NewStage = GameObject.Instantiate(stagePrefab);
        _Cur_Stage = NewStage.GetComponent<Stage>();
        _Cur_Stage.Initalize();

        return true;
    }

    #endregion

}
