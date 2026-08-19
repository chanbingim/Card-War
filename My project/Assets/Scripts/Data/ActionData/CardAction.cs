using TurnCardGame.Data;
using UnityEngine;

public class CardAction : CharacterAction
{
    public int              PlayerID { get; private set; }
    public Character        ActObject { get; private set; }
    public UI_CardData      CardData { get; private set; }

    public CardAction(int ID, Character actObject, UI_CardData uI_CardData, EACTION_TYPE actType)
        : base(actType)
    {
        PlayerID = ID;
        CardData = uI_CardData;
        ActObject = actObject;
    }
}