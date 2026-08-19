using Cysharp.Threading.Tasks;
using GamePlay.Enum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : MonoBehaviour
{
    GameObject PoolRoot = null;
    Dictionary<EPoolType, ObjectPool<PoolAbleComponent>> _PoolList = new();

    public async UniTask InitializeAsync(string StageName)
    {
        List<PoolAbleComponent> PoolList =
            await AddressableManager.instance.GetLabelAll<PoolAbleComponent>(StageName);

        if (PoolRoot == null)
        {
            PoolRoot = new GameObject("PoolRoot");
        }

        if (PoolList.Count <= 0)
            return;

        foreach (var PoolComponent in PoolList)
        {
            CreatePoolObject(PoolComponent, PoolRoot.transform);
        }
    }

    /* 
     * Pool Root를 해당씬의 객체로 옮겨주는 역할을 한다.
     * 즉 객체를 옮기는 작업을 하기 때문에 무겁다
     * 만약 인스턴스가있다면 Release해서 다 지워야함
     */
    public void PoolRootMoveScene(Scene nextScene)
    {
        SceneManager.MoveGameObjectToScene(PoolRoot, nextScene);
    }

    public T Get<T>(EPoolType poolType, string ObjectName) 
        where T : PoolAbleComponent
    {
        if (_PoolList.TryGetValue(poolType, out var objectPool))
        {
            return objectPool.Get<T>(ObjectName);
        }

        return null;
    }

    public void Return<T>(EPoolType poolType, string poolObjectName, T obj) 
        where T : PoolAbleComponent
    {
        if(_PoolList.TryGetValue(poolType, out var objectPool))
        {
            objectPool.Return(poolObjectName, obj);
        }
    }

    public void Release()
    {
        foreach(var PoolList in _PoolList)
        {
            PoolList.Value.Release();
        }
        _PoolList.Clear();

        if(PoolRoot != null)
        {
            Destroy(PoolRoot);
            PoolRoot = null;
        }
    }

    private bool CreatePoolObject(PoolAbleComponent PoolObject, Transform PoolRoot)
    {
        if (!_PoolList.TryGetValue(PoolObject.PoolType, out var objectPool))
        {
            objectPool = new ObjectPool<PoolAbleComponent>();
            objectPool.CreatePoolObject(PoolObject, PoolRoot);

            _PoolList.Add(PoolObject.PoolType, objectPool);
        }

        return true;
    }

    #region Default
    public static   PoolManager Instance => instance;
    private static  PoolManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(instance.gameObject);
    }
    #endregion
}