using GamePlay.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GAME_CONST;
using UnityEngine.U2D;

namespace TurnCardGame.Data
{
    public enum ECardEffectType
    {
        Damage,
        Guard,
        Heal,
        AttackBuff,
        DefenseBuff,
        None
    }

    public class CardData
    {
        public int          ID;

        public string       TextureKey;
        public Sprite       Icon { get; private set; }
        public Vector4      SpriteUV { get; private set; }

        public string           Name;
        public ECardEffectType  eEffectType;
        public ETargetType      eTargetType;

        public int              Power;
        public int              Duration;
        public int              MaxStacks;
        public string           Description;
        
        public string           EffectKeys;
        public List<string>     VFXKeys { get; private set; }
        public Sprite           BuffIcon { get; private set; }

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

            var IconAtlas = AddressableMgr.Get<SpriteAtlas>(Const.CardIconAddress);
            if (IconAtlas == null)
                return false;

            Icon = IconAtlas.GetSprite(TextureKey);
            SpriteUV = Utility.GetAtlasUVData(Icon.texture, Icon);

            BuffIcon = IconAtlas.GetSprite(eEffectType.ToString());
            return true;
        }
    }
}
