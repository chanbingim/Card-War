using System;
using UnityEngine;

public class TurnParticipantBase : ITurnParticipant
{
    public string Name { get; protected set; }
    public bool IsActive { get; protected set; }
    public event Func<string, bool> RequestTurnEnd;

    public virtual void TurnBegin()
    {
        Debug.Log($"{Name}의 턴 시작");
        IsActive = true;
    }

    public virtual void TurnEnd()
    {
        Debug.Log($"{Name}의 턴 종료 (기본 처리)");
        RequestTurnEnd.Invoke(Name);
        IsActive = false;
    }

    public virtual void TurnRunning()
    {
        
    }
}
