using System;

public readonly struct ChangeTurnActEvent
{
    public bool _IsLocal { get; }
    public ETurnType eTurnType { get; }
    public Action   _OnCompleted { get; }

    public ChangeTurnActEvent(ETurnType eturnType, bool IsLocal = false, Action OnCompleted = null)
    {
        _IsLocal = IsLocal;
        eTurnType = eturnType;
        _OnCompleted = OnCompleted;
    }
}
