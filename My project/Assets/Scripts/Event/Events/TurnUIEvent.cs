using UnityEngine;

public readonly struct TurnUIEvent
{
    public string Name { get; }

    public TurnUIEvent(string name)
    {
        Name = name;
    }
}
