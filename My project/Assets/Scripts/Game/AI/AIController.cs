using Unity.Behavior;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public  BattlePlayerData    _Data { get; private set; } = null;
    private BehaviorGraphAgent  _behaviour = null;

    void Awake()
    {
        _behaviour = GetComponent<BehaviorGraphAgent>();    
        if( _behaviour == null )
        {
            Debug.Log("[AI_Character] Not Add Component Behavior");
            return;
        }
    }

    public void Initialize(StageAISO data)
    {
        _Data = new BattlePlayerData(data);

        _Data._OnTurnChangeStart += TurnChagneEvent;

        _behaviour.SetVariableValue<AIController>("Controller", this);
        _behaviour.SetVariableValue<string>("NickName", _Data.Name);
    }

    private void OnDestory()
    {
        _Data._OnTurnChangeStart -= TurnChagneEvent;
    }

    private void TurnChagneEvent(ETurnType TurnType)
    {
        var BattleMgr = BattleManager.instance;
        if (BattleMgr == null)
            return;

        if(TurnType <= ETurnType.END)
        {
            _behaviour.SetVariableValue<bool>("IsActive", true);
            _behaviour.SetVariableValue<ETurnType>("CurrentTurnType", TurnType);
        }
        else
        {
            _behaviour.SetVariableValue<bool>("IsActive", false);
        }
    }
}
