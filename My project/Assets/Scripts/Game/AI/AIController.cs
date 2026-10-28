using System.Collections.Generic;
using TurnCardGame.Data.AI;
using TurnCardGame.Serialization;
using Unity.Behavior;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public  BattlePlayerData    _Data { get; private set; } = null;
    private BehaviorGraphAgent  _behaviour = null;

    private Dictionary<int, CardUseTurnData> _TurnActions = new();
    private Queue<CardUseActionData>    _Actions = new();

    private int TurnCount = 1;

    void Awake()
    {
        _behaviour = GetComponent<BehaviorGraphAgent>();    
        if( _behaviour == null )
        {
            Debug.Log("[AI_Character] Not Add Component Behavior");
            return;
        }

        _behaviour.SetVariableValue<AIController>("Controller", this);
    }

    public void Initialize(StageAISO data)
    {
        _Data = new BattlePlayerData(data);

        var Action = XmlDataLoader<CardUsePatternData>.Deserialize(data._AIAction);

        foreach (var PatternData in Action.Turns)
            _TurnActions.Add(PatternData.Number, PatternData);

        _Data._OnTurnChangeStart += TurnChagneEvent;
        _behaviour.SetVariableValue<string>("NickName", _Data.Name);
    }

    private void OnDestory()
    {
        _Data._OnTurnChangeStart -= TurnChagneEvent;
    }

    public void TurnBegin() { GetTurnActions(); }

    public void UseCurrentTurnCardUse()
    {
        if (_Actions.Count <= 0)
            return;

        var Act = _Actions.Dequeue();

        // 여기서 조건에 맞는 데이터를 Queue에서 꺼내면 될듯
        Debug.Log($"{Act.CardId} AI의 카드 사용");
    }

    private void TurnChagneEvent(ETurnType TurnType)
    {
        var BattleMgr = BattleManager.instance;
        if (BattleMgr == null)
            return;

        _behaviour.SetVariableValue<bool>("IsActive", _Data.IsActive);
        _behaviour.SetVariableValue<ETurnType>("CurrentTurnType", TurnType);

        if (TurnType > ETurnType.END)
        {
            TurnCount++;
        }
    }

    private void GetTurnActions()
    {
        if(_TurnActions.TryGetValue(TurnCount, out var turnActions))
        {
            _Actions = new Queue<CardUseActionData>(turnActions.CardUses);
        }
    }

}
