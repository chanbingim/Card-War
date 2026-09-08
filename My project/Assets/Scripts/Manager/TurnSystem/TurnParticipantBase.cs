using System;
using UnityEngine;

public class TurnParticipantBase : ITurnParticipant
{
    #region Event
    //public event Func<string, bool> RequestTurnEnd;
    public event Action<ETurnType> _OnTurnChangeStart;
    #endregion

    public int      PlayerTurnIndex { get; private set; }
    public string   Name { get; protected set; }
    public bool     IsActive { get; protected set; }
    public bool IsLocal { get; protected set; }

    public void SetPlayerTurn(int Index)
    {
        PlayerTurnIndex = Index;
    }

    public virtual void TurnBegin()
    {
        Debug.Log($"{Name}의 턴 시작");
        IsActive = true;
    }

    public void TrunChange(ETurnType TurnType)
    {
        _OnTurnChangeStart?.Invoke( TurnType );
    }

    public virtual void TurnEnd()
    {
        Debug.Log($"{Name}의 턴 종료 (기본 처리)");
        //RequestTurnEnd.Invoke(Name);
        IsActive = false;
    }

    public virtual void TurnRunning()
    {
        
    }
}
