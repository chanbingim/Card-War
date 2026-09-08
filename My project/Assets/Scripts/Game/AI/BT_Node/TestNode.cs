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
        // 공격 로직

        return Status.Success;
    }
}
