using GamePlay.Enum;
using UnityEngine;

public class PoolFactory
{
    public static EffectBase GetPoolEffect(string Key, Transform Parent)
    {
        var PoolComponent = PoolManager.Instance.Get<EffectBase>(EPoolType.Effect, Key);
        if (PoolComponent != null)
        {
            PoolComponent.Play();
            PoolComponent.gameObject.transform.position = Parent.position;
        }
        else
        {
            Debug.LogWarning("[Pool Factory] Not Find Effect");
        }

        return PoolComponent;
    }
}
