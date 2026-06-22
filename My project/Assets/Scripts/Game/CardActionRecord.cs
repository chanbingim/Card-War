using TurnCardGame.Data;

namespace TurnCardGame.Game
{
    public readonly struct CardActionRecord
    {
        public CardActionRecord(CardData card, string targetId, int value)
        {
            Card = card;
            TargetId = targetId;
            Value = value;
        }

        public CardData Card { get; }
        public string TargetId { get; }
        public int Value { get; }
    }
}
