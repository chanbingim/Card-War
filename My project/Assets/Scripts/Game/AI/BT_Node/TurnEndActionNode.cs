using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TurnChangeAction", story: "Change Turn", 
    category: "Action", id: "79bbc55bcecfec26e3dff12d80d0e8bb")]
public partial class TurnEndActionNode : Action
{
    [SerializeReference]
    public BlackboardVariable<string> _NickName;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var BattleMgr = BattleManager.instance;
        if (BattleMgr == null)
        {
            Debug.Log("[Turn End Action] Not Find Battle Manager");
            return Status.Failure;
        }

        BattleMgr.RequestEndTurn(_NickName);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

