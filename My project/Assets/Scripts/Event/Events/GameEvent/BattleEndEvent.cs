using TurnCardGame.Data;
using UnityEngine;

public readonly struct BattleEndEvent
{
    public readonly bool    IsWinner;
    public readonly int     ClearStar;

    public BattleEndEvent(bool IsWin, int clearStar)
    {
        IsWinner = IsWin;
        ClearStar = clearStar;
    }
}
