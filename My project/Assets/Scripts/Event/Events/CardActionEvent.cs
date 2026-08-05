using TurnCardGame.Data;
using UnityEngine;

public readonly struct CardActionEvent
{
    public readonly CardAction Action;

    public CardActionEvent(CardAction action)
    {
        Action = action;
    }
}
