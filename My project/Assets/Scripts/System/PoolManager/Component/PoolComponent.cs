using GamePlay.Enum;
using UnityEngine;

public class PoolAbleComponent : MonoBehaviour, IPoolAble
{
    public EPoolType        PoolType;
    public string           PoolName;
    public int              PoolInitCount;
    public int              ExpandCount;

    public void ReturnToPool()
    {
        // 오브젝트 반환
        PoolManager.Instance.Return<PoolAbleComponent>(PoolType, PoolName, this);
    }
}