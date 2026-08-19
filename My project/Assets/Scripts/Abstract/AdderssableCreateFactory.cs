
using UI.Enum;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Factory
{
    public class AdderssableCreateFactory
    {
        public static T Create<T>(string AddressableKey ,Transform parent) 
            where T : UnityEngine.Object
        {
            GameObject Prefab = AddressableManager.instance.Get<GameObject>(AddressableKey);
               
            if (!Utility.CHECK(Prefab))
                return null;

            var obj = GameObject.Instantiate(Prefab, parent);
            if (Utility.CHECK(obj) == false)
                return null;

            return obj as T;
        }
    }

    public class CharacterCreateFactory
    {
        public static Character Create(int ID, Transform Party, Vector3 Position, bool bIsLoacl)
        {
            var obj = Factory.AdderssableCreateFactory.Create<GameObject>("Prefabs/Character", Party);
            if (obj == null)
                return null;

            Character character = obj.GetComponent<Character>();
            if (Utility.CHECK(character))
            {
                if (bIsLoacl == false)
                    character.GetComponent<SpriteRenderer>().flipX = true;

                character.Initialize(ID, Position);
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
    }
}
