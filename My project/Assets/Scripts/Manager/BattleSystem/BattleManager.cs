using System;
using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private BattleCardManager        _BattleCardManager = null;
    private TurnManager              _TrunMgr = null;
    private Stage                    _Cur_Stage = null;

    public Stage GetCurrentStage() { return _Cur_Stage ? _Cur_Stage : null; }

    public void Update()
    {
        if (_TrunMgr.IsRunning == false)
            _TrunMgr.Begin();
        else
            _TrunMgr.Update();
    }

    #region PlayerMgr
    public bool             IsPlayerTurn() { return _TrunMgr?.IsPlayerTurn() ?? false; }
    public BattlePlayerData GetLoaclPlayer() { return _TrunMgr?.LocalPlayer; }
    public UI_CardData      DrawCard() { return _BattleCardManager?.Draw_Card() ?? null; }
    #endregion

    #region TrunMgr
    public void RequestEndTurn()
    {
        _TrunMgr?.RequestEndTurn(_TrunMgr.Current.Name);
    }

    public IReadOnlyList<CharacterAction> GetAllHistory()
    {
        return _TrunMgr?.GetAllHistory() ?? null;
    }

    public TurnManager.ETurnType GetTurnType()
    {
        return _TrunMgr?._TurnType ?? TurnManager.ETurnType.END;
    }
    #endregion

    #region Battle Mgr
    public void RequestAttack(Character Attacker, Character Target)
    {
        CharacterRuntimeData AttackData = Attacker.Data;
        CharacterRuntimeData TargetData = Target.Data;

        int DamageAmount = ComputeDamageLogic(AttackData.CurrentATKPower);

        _TrunMgr.ADDHistoryActionData(new BattleAction(Attacker, Target, EACTION_TYPE.ATTACK));
        Target.RequestDamaged(DamageAmount);
    }


    private int ComputeDamageLogic(int OrizinDamage)
    {

        return OrizinDamage;
    }

    #endregion

    #region Defualt
    static public BattleManager instance { get; private set; }
    private void Start()
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

        if (InitBattleCardManager() == false)
        {
            Debug.LogWarning("Initialize Fail BattleCardManager");
            return;
        }

        // 이거 나중에 서버에서 받아오긴할거임
        List<ITurnParticipant> participants = new List<ITurnParticipant>();
        participants.Add(GameClientManager.instance.GetBattleData());
        participants.Add(new BattlePlayerData());

        if (InitTurnManager(participants) == false)
        {
            Debug.LogWarning("Initialize Fail TurnManager");
            return;
        }
    }

    private bool InitBattleCardManager()
    {
        _BattleCardManager = BattleCardManager.Create();
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
