using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

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
        public Sprite       Icon { get; private set; }

        public string       Name;
        public EEffectType  eEffectType;
        public ETargetType  eTargetType;

        public int              Power;
        public int              Duration;
        public int              MaxStacks;
        public string           Description;
        
        public string           EffectKeys;
        public List<string>     VFXKeys { get; private set; }

        public bool ParseData()
        {
            if (string.IsNullOrWhiteSpace(EffectKeys))
                return false;

            VFXKeys = EffectKeys.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(key => key.Trim())
                        .Distinct()
                        .ToList();

            var AddressableMgr = AddressableManager.instance;
            if(AddressableMgr == null)
            {
                Debug.Log("[CardData] Not Find Addressable");
                return false;
            }

            var Atlas = AddressableMgr.Get<SpriteAtlas>("Atlas/CardIcon");
            if (Atlas == null)
                return false;

            Icon = Atlas.GetSprite(TextureKey);
            return true;
        }
    }
}
