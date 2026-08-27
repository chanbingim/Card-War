using TurnCardGame.Data;
using UnityEngine;

public readonly struct CardDrawEvent
{
    public readonly UI_CardData      _CardData;

    public CardDrawEvent(UI_CardData CardData)
    {
        _CardData = CardData;
    }
}
