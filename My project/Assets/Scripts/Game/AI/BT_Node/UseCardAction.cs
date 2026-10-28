using System;
using Unity.Behavior;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UseCardAction", story: "CardAction", category: "Action", id: "e34c1b5a4bb00546973a328f4f31f27a")]
public partial class UseCardAction : Action
{
    [SerializeReference]
    public BlackboardVariable<AIController> _Controller;

    bool bIsCardAction = false;
    float   CurTime = 0;
    int     AccCount = 0;

    protected override Status OnStart()
    {
        CurTime = 0;
        AccCount = 0;

        bIsCardAction = false;
        _Controller.Value.TurnBegin();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        CurTime += Time.deltaTime;
        if (AccCount > 15)
        {
            Debug.Log("[UseCardAction] Turn Time Out");
            return Status.Success;
        }

        if(CurTime >= 1f)
        {
            _Controller.Value.UseCurrentTurnCardUse();
            CurTime = 0f;
            AccCount++;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

