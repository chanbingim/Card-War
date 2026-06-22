using UnityEngine;

namespace TurnCardGame.Data
{
    public enum CardEffectType
    {
        Damage,
        Guard,
        Heal
    }

    [CreateAssetMenu(menuName = "Turn Card Game/Card", fileName = "CardData")]
    public sealed class CardData : ScriptableObject
    {
        [SerializeField] private string cardId = "card";
        [SerializeField] private string title = "Card";
        [SerializeField] private CardEffectType effectType = CardEffectType.Damage;
        [SerializeField] private int power = 3;
        [TextArea]
        [SerializeField] private string description = "Deal damage.";

        public string CardId => cardId;
        public string Title => title;
        public CardEffectType EffectType => effectType;
        public int Power => Mathf.Max(0, power);
        public string Description => description;
    }
}
