using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct PoolCash
{
    public PoolAbleComponent        Base;
    public GameObject               BaseTarget;
    public Queue<PoolAbleComponent> Pool;
}

public class ObjectPool<T> where T : PoolAbleComponent
{
    private Dictionary<string, PoolCash> _PoolList = new();

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
            PoolQueue.Pool = new Queue<PoolAbleComponent>();

            PoolQueue.BaseTarget = new GameObject($"{typeof(T).Name}_Pool");
            PoolQueue.BaseTarget.transform.SetParent(PoolRoot);

            CreateCount = PoolObject.PoolInitCount;

            _PoolList.Add(PoolObject.name, PoolQueue);
        }

        for (int i = 0; i < CreateCount; i++)
        {
            var obj = GameObject.Instantiate(go, PoolQueue.BaseTarget.transform);
            obj.SetActive(false);

            PoolQueue.Pool.Enqueue(obj.GetComponent<PoolAbleComponent>());
        }

        return true;
    }

    public T Get<T>(string ObjectName) where T : PoolAbleComponent
    {
        if (!_PoolList.TryGetValue(ObjectName, out var objectPool))
            return null;

        if (objectPool.Pool.Count <= 0)
        {
            CreatePoolObject(objectPool.Base, null);
        }

        var poolObject = objectPool.Pool.Dequeue();
        poolObject.gameObject.SetActive(true);

        return poolObject as T;
    }

    public void Return(string PoolName, T objectToReturn)
    {
        if (_PoolList.TryGetValue(PoolName, out var objectPool))
        {
            objectPool.Pool.Enqueue(objectToReturn);
        }

        objectToReturn.gameObject.SetActive(false);
    }

    public void Release()
    {
        _PoolList = null;
    }
}
