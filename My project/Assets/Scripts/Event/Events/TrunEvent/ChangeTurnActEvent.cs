using System;

public readonly struct ChangeTurnActEvent
{
    public bool _IsLocal { get; }
    public ETurnType eTurnType { get; }
    public float MaxTurnTime { get; }

    public Action   _OnCompleted { get; }
    public Action   _OnTimerCompleted { get; }

    public ChangeTurnActEvent(float maxTurnTime, ETurnType eturnType, bool IsLocal = false, Action OnCompleted = null, Action OnTimerCompleted = null)
    {
        _IsLocal = IsLocal;
        MaxTurnTime = maxTurnTime;
        eTurnType = eturnType;
        _OnCompleted = OnCompleted;
        _OnTimerCompleted = OnTimerCompleted;
    }
}
