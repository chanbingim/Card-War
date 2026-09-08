using System;
using Unity.Behavior;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UseCardAction", story: "CardAction", category: "Action", id: "e34c1b5a4bb00546973a328f4f31f27a")]
public partial class UseCardAction : Action
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

        if (CurTime > 5.0f)
        {
            Debug.Log("[UseCardAction] Turn Time Out");
            return Status.Success;
        }
        else
        {
            Debug.Log("[UseCardAction] Use Card Action");
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

