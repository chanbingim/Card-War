using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChracterBuffView : MonoBehaviour
{
    private Character       _Owner = null;
    private List<Image>     _IconList = new List<Image>();

    public void Initalize(Character Owner)
    {
        _Owner = Owner;
        _Owner.SubscribeBuffChange(OnChangBuffList);
    }

    public void Release()
    {
        if (_Owner == null)
            return;

       

        _Owner.UnSubscribeBuffChange(OnChangBuffList);
        _Owner = null;
    }

    public void OnDisable()
    {
        Release();
    }

    private void OnChangBuffList(List<BuffInstance> List)
    {
        for(int i = 0; i < List.Count; i++)
        {
            if (_IconList.Count <= i)
            {
                // 여기서 풀 에서 꺼내서 등록
                var PoolItem = PoolManager.Instance.Get<PoolAbleComponent>(GamePlay.Enum.EPoolType.UI, "BuffIcon");
                if (PoolItem != null)
                {
                    PoolItem.transform.SetParent(transform);
                    _IconList.Add(PoolItem.GetComponent<Image>());
                }
            }

            // 들어온 마지막 데이터에 sprite세팅
            var lastIcon = _IconList.Last();
            if(lastIcon != null)
            {

            }
        }

        Debug.Log($"Buff Count : {List.Count}");
        Debug.Log("On Change Buff List");
    }
}
