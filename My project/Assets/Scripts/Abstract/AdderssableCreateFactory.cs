
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
}
