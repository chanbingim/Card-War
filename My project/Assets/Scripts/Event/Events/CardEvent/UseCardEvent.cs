using TurnCardGame.Data;

public readonly struct UseCardEvent
{
    public readonly bool          IsAll;
    public readonly Character     Target;

    public readonly int           ContollerIdx;
    public readonly int           HandIdx;
    public readonly CardData      UseCard;

    public UseCardEvent(Character target, int handIdx, int contollerIdx, CardData data)
    {
        IsAll = false;
        HandIdx = handIdx;
        ContollerIdx = contollerIdx;
        Target = target;
        UseCard = data;
    }

    public UseCardEvent(int handIdx, int contollerIdx, CardData data)
    {
        IsAll = true;
        HandIdx = handIdx;
        ContollerIdx = contollerIdx;
        Target = null;
        UseCard = data;
    }
}
