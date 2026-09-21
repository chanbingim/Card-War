using UnityEngine;

namespace TurnCardGame.Data
{
    public enum EEffectType
    {
        Damage,
        Guard,
        Heal,
        AttackBuff,
        DefenseBuff,
        None
    }

    public enum ETargetType
    {
        FriendlySelf,
        FriendlyAll,
        EnemySelf,
        EnemyAll,
        None
    }

    public class CardData
    {
        public int          ID;
        public string       TextureKey;

        public string       Name;

        public EEffectType  eEffectType;
        public ETargetType  eTargetType;

        public int          Power;
        public int          Duration;
        public int          MaxStacks;
        public string       Description;
    }
}
