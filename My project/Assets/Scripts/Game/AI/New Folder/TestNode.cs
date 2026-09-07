using Unity.Behavior;
using UnityEngine;


[NodeDescription(
    name: "Test", 
    story: "Node Test",
    category: "Action",
    id: "TestNode"
)]

public class TestNode : Action
{
    protected override Status OnUpdate()
    {
        Debug.Log("Action Start");

        // 공격 로직

        return Status.Success;
    }
}
