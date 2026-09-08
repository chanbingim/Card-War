using TurnCardGame.Data;

public readonly struct CardDrawEvent
{
    public readonly bool             _IsLocal;
    public readonly UI_CardData      _CardData;

    public CardDrawEvent(bool IsLocal, UI_CardData CardData)
    {
        _IsLocal = IsLocal;
        _CardData = CardData;
    }
}
