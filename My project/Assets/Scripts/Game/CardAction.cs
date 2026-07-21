using UnityEngine;

public enum EACTION_TYPE
{
    ATTACK, DEFENCE, END
}

public class CardAction : MonoBehaviour
{
    public Character        ActObject { get; private set; }
    public EACTION_TYPE     ActType { get; private set; }

    public CardAction(Character actObject, EACTION_TYPE actType)
    {
        ActObject = actObject;
        ActType = actType;
    }
}
