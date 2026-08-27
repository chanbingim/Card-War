using System;
using UnityEngine;

public readonly struct ChangeTurnEvent
{
    public bool     _IsLocal { get; }
    public Action   _OnCompleted { get; }

    public ChangeTurnEvent(bool IsLocal = false, Action OnCompleted = null)
    {
        _IsLocal = IsLocal;
        _OnCompleted = OnCompleted;
    }
}
