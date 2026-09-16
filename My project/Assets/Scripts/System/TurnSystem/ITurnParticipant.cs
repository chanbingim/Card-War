using System;
using UnityEngine;

public interface ITurnParticipant
{
    string  Name { get; }
    bool    IsActive { get; }

    public void TurnBegin();
    public void TurnRunning();
    public void TurnEnd();
}
