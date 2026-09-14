using TurnCardGame.Data;
using UnityEngine;

public enum EACTION_TYPE
{
    ATTACK, DEFENCE, USE_CARD, END
}

public class CharacterAction
{
    public EACTION_TYPE ActType { get; private set; }

    public CharacterAction(EACTION_TYPE actType)
    {
        ActType = actType;
    }
}