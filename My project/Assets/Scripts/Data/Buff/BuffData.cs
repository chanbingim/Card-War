using TurnCardGame.Data;
using UnityEngine;

public class BuffInstance
{
    public CardData Data;
    private int TurnCount;

    public bool IsDead() { return TurnCount >= Data?.Duration; }
    public void Update() { TurnCount++; }

    public void Initialize(CardData data)
    {
        Data = data;
        TurnCount = 0;
    }
}