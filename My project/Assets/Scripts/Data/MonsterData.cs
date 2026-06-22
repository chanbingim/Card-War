using UnityEngine;

namespace TurnCardGame.Data
{
    [CreateAssetMenu(menuName = "Turn Card Game/Monster", fileName = "MonsterData")]
    public sealed class MonsterData : ScriptableObject
    {
        [SerializeField] private string monsterId = "monster";
        [SerializeField] private string title = "Monster";
        [SerializeField] private int maxHealth = 12;
        [SerializeField] private int attackPower = 2;

        public string MonsterId => monsterId;
        public string Title => title;
        public int MaxHealth => Mathf.Max(1, maxHealth);
        public int AttackPower => Mathf.Max(0, attackPower);
    }
}
