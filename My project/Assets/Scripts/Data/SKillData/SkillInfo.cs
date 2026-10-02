
using GamePlay.Enum;
using System;

namespace TurnCardGame.Data
{
    public enum ESkillActionType
    {
        NormalAttack,
        Skill
    }

    public class SkillInfo
    {
        public int ActionId { get; set; }
        public string NameEn { get; set; }
        public string NameKo { get; set; }
        public int Damage { get; set; }

        public ESkillActionType ActionType { get; set; }
        public ETargetType TargetType { get; set; }

        public string DescriptionEn { get; set; }
        public string DescriptionKo { get; set; }
        public string AttackEffectId { get; set; }
        public string HitEffectId { get; set; }
    }
}
