using UnityEngine;

public readonly struct TurnStartEvent
{
    public string Name { get; }

    public TurnStartEvent(string name)
    {
        Name = name;
    }
}
