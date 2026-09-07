using TurnCardGame.Data;
using UnityEngine;

public readonly struct ActionRecordedEvent
{
    public readonly CharacterAction Action;

    public ActionRecordedEvent(CharacterAction action)
    {
        Action = action;
    }
}
