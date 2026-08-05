using UnityEngine;

public enum EACTION_TYPE
{
    ATTACK, DEFENCE, END
}

public class CardAction : MonoBehaviour
{
    public int              PlayerID { get; private set; }
    public Character        ActObject { get; private set; }
    public EACTION_TYPE     ActType { get; private set; }

    public CardAction(int ID, Character actObject, EACTION_TYPE actType)
    {
        PlayerID = ID;
        ActObject = actObject;
        ActType = actType;
    }
}
