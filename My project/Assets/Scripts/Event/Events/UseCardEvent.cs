using TurnCardGame.Data;
using UnityEngine;

public readonly struct UseCardEvent
{
    public readonly Character        Target;
    public readonly CardUI           UseCard;

    public UseCardEvent(Character target, CardUI data)
    {
        Target = target;
        UseCard = data;
    }
}
