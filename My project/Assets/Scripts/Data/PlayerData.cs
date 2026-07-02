using System.Collections.Generic;

public class PlayerData
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
}
