using TurnCardGame.Data;
using UnityEngine;


public class BattleAction : CharacterAction
{
    public int PlayerID { get; private set; }
    public Character ActObject { get; private set; }
    public Character TargetObject { get; private set; }

    public BattleAction(Character actObject, Character targetObject, EACTION_TYPE actType)
        : base(actType)
    {   
        ActObject = actObject;
        TargetObject = targetObject;
    }
}