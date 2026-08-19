using TurnCardGame.Data;
using UnityEngine;

public readonly struct CardActionEvent
{
    public readonly CharacterAction Action;

    public CardActionEvent(CharacterAction action)
    {
        Action = action;
    }
}
