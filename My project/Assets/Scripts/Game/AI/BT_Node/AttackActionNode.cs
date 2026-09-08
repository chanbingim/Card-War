using System;
using Unity.Behavior;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackActionNode", story: "AttackAction", category: "Action", id: "e8f88ec07e2de88376c57b5209b99574")]
public partial class AttackActionNode : Action
{
    float CurTime = 0;

    protected override Status OnStart()
    {
        CurTime = 0;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        CurTime += Time.deltaTime;

        if(CurTime > 5.0f)
        {
            Debug.Log("[AttackActionNode] Turn Time Out");
            return Status.Success;
        }
        else
        {
            Debug.Log("[AttackActionNode] Attack Action");
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

