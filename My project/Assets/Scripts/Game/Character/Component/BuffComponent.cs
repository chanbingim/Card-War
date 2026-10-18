using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;

public class BuffComponent : MonoBehaviour
{
    public delegate void BuffListChangedHandler(List<BuffInstance> List);
    public event BuffListChangedHandler OnChangeBuffList;

    private Dictionary<ECardEffectType, List<BuffInstance>> Buffs = new();

    public void Initialize()
    {

    }

    public void TurnEnd()
    {
        foreach (var iter in Buffs)
        {
            int PreCount = iter.Value.Count;
            iter.Value.RemoveAll(item =>
            {
                item.Update();
                return item.IsDead();
            });

            if(PreCount != iter.Value.Count)
                OnChangeBuffList?.Invoke(iter.Value);
        }
    }

    public void ADD_Bff(CardData data)
    {
        BuffInstance BuffInstance = new BuffInstance();
        BuffInstance.Initialize(data);

        if (Buffs.TryGetValue(data.eEffectType, out var value))
        {
            value.Add(BuffInstance);
        }
        else
        {
            List<BuffInstance> newList = new List<BuffInstance>();

            newList.Add(BuffInstance);
            Buffs.Add(data.eEffectType, newList);
        }

        OnChangeBuffList?.Invoke(Buffs[data.eEffectType]);
    }

    public void RandomRemove_Bff(ECardEffectType eEffectType, int Count = 1)
    {
        if (Buffs.TryGetValue(eEffectType, out var value))
        { 
            int RandIndex = Random.Range(0, value.Count - Count + 1);
            value.RemoveRange(RandIndex, Count);

            OnChangeBuffList?.Invoke(value);
        }
    }

    public int GetTotalEffectValue(ECardEffectType effectType)
    {
        int total = 0;

        if(Buffs.TryGetValue(effectType, out var iter))
        {
            foreach (var buff in iter)
            {
                if (buff.Data.eEffectType != effectType)
                    continue;

                total += buff.Data.Power;
            }
        }

        return total;
    }
}
