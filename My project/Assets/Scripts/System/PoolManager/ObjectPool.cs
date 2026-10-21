using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct PoolData
{
    public float LastReturnTime;
    public PoolAbleComponent Data;
}

public struct PoolCash
{
    public PoolAbleComponent        Base;
    public GameObject               BaseTarget;
    public Queue<PoolData>          PoolData;
}

public class ObjectPool<T> where T : PoolAbleComponent
{
    private Dictionary<string, PoolCash> _PoolList = new();
    private int ReturnCount = 10;

    public bool CreatePoolObject(PoolAbleComponent PoolObject, Transform PoolRoot)
    {
        GameObject go = PoolObject.gameObject;
        if (go == null)
            return false;

        int CreateCount = PoolObject.ExpandCount;
        if (!_PoolList.TryGetValue(PoolObject.name, out var PoolQueue))
        {
            PoolQueue = new PoolCash();
            PoolQueue.Base = PoolObject;
            PoolQueue.PoolData = new Queue<PoolData>();

            PoolQueue.BaseTarget = new GameObject($"{typeof(T).Name}_Pool");
            PoolQueue.BaseTarget.transform.SetParent(PoolRoot);

            CreateCount = PoolObject.PoolInitCount;
            _PoolList.Add(PoolObject.PoolName, PoolQueue);
        }

        for (int i = 0; i < CreateCount; i++)
        {
            var obj = GameObject.Instantiate(go, PoolQueue.BaseTarget.transform);
            obj.SetActive(false);

            PoolData data;
            data.LastReturnTime = Time.time;
            data.Data = obj.GetComponent<PoolAbleComponent>();

            PoolQueue.PoolData.Enqueue(data);
        }

        return true;
    }

    public T Get<T>(string ObjectName) where T : PoolAbleComponent
    {
        if (!_PoolList.TryGetValue(ObjectName, out var objectPool))
            return null;

        if (objectPool.PoolData.Count <= 0)
        {
            CreatePoolObject(objectPool.Base, null);
        }

        var poolObject = objectPool.PoolData.Dequeue();
        poolObject.Data.gameObject.SetActive(true);

        return poolObject.Data as T;
    }

    public void Return(string PoolName, T objectToReturn)
    {
        if (_PoolList.TryGetValue(PoolName, out var objectPool))
        {
            PoolData data;
            data.LastReturnTime = Time.time;
            data.Data = objectToReturn;

            objectPool.PoolData.Enqueue(data);
            objectToReturn.transform.SetParent(objectPool.BaseTarget.transform);
        }

        objectToReturn.gameObject.SetActive(false);
        ReturnCount++;
        if(ReturnCount >= 10)
        {
            CheckRemovePool();
            ReturnCount = 0;
        }
    }

    public void Release()
    {
        _PoolList = null;
    }

    private void CheckRemovePool()
    {
        foreach(var iter in _PoolList)
        {
            var pool = iter.Value;
            while(pool.PoolData.Count > iter.Value.Base.PoolInitCount)
            {
                PoolData oldest = pool.PoolData.Dequeue();

                // 필요하면 실제 오브젝트도 삭제
                if (oldest.Data != null)
                {
                    oldest.Data.Release();
                }
            }
        }
    }
}
