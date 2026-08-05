using TurnCardGame.Data;
using UnityEngine;

public readonly struct ActionRecordedEvent
{
    public readonly CardAction Action;

    public ActionRecordedEvent(CardAction action)
    {
        Action = action;
    }
}
