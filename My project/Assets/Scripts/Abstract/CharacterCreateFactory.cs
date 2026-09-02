
using System;
using TurnCardGame.Data;
using UI.Enum;
using Unity.VisualScripting;
using UnityEngine;

namespace Factory
{
    public class CharacterCreateFactory
    {
        public static Character Create(int ID, Transform Party, Vector3 Position, bool bIsLoacl)
        {
            var obj = Factory.AdderssableCreateFactory.Create<GameObject>("Prefabs/Character", Party);
            if (obj == null)
                return null;

            CharacterData CharacterSO = DataManager.instance.GetCharacterById(ID);
            Character character = (Character)obj.AddComponent(GetCreateCharacter(CharacterSO.ATKType));

            if (Utility.CHECK(character))
            {
                if (bIsLoacl == false)
                    character.GetComponent<SpriteRenderer>().flipX = true;

                character.Initialize(CharacterSO, Position);
            }

            var PoolAble = PoolManager.Instance.Get<PoolAbleComponent>(GamePlay.Enum.EPoolType.UI, "CharacterHP");
            if (PoolAble == null)
                return null;
            
            var CharacterHP = PoolAble.gameObject.GetComponent<BattleCharacterUI>();
            if (CharacterHP == null)
                return null;

            var UIMgr = UIManager.instance;
            if(UIMgr == null)
            {
                Debug.Log("[CharacterCreateFactory] UIManager not found");
                return null;
            }

            var WorldCanvas = UIMgr.GetCanvas(EUICanvas.Scene_World);
            if(WorldCanvas == null)
            {
                Debug.Log("[CharacterCreateFactory] UnBind Canvas Type");
                return null;
            }

            CharacterHP.Initalize(character, Position, WorldCanvas.transform);
            return character;
        }

        public static Type GetCreateCharacter(EATTAK_TYPE eType)
        {
            switch(eType)
            {
                case EATTAK_TYPE.Attacker:
                    return typeof(AttackerCharacter);

                case EATTAK_TYPE.Mage:
                    return typeof(MageCharacter);
            }

            return null;
        }
    }
}
