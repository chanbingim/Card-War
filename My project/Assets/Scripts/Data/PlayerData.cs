using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : TurnParticipantBase
{
    public List<int> Decks;
    public List<int> Hands;
    public List<int> Skills;

    public PlayerData()
    {
        Decks = new List<int>(GAME_CONST.Const.MAX_DECK);
        Hands = new List<int>(GAME_CONST.Const.MAX_HAND);
        Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);
    }

    public override void TurnRunning()
    {
        if (IsActive == false)
            return;

        if(Input.GetKeyDown(KeyCode.End))
        {
            TurnEnd();
        }
    }

    public void SetName(string name)
    {
        Name = name;
    }
}
