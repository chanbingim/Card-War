using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class AddressableManager : MonoBehaviour
{
    /// Key -> Asset
    private readonly Dictionary<string, UnityEngine.Object> _cache = new();

    /// Label -> Group
    private readonly Dictionary<string, AddressableGroup> _groups = new();

    /// 개별 로드 Handle
    private readonly Dictionary<string, AsyncOperationHandle> _handles = new();

    // 해당 작업에 로드 상태를 확인한다.
    public float GetPercent(string Handle)
    {
        if (_handles.TryGetValue(Handle, out var value))
        {
            return value.PercentComplete;
        }

        return 0;
    }

    #region Load
    public async UniTask LoadLabelAll(string label, Action<float >action = null)
    {
        if (_groups.ContainsKey(label))
            return;

        AddressableGroup group = new(label);
        
        // 함수는 Label의 Tag에 따라서 데이터를 로드한다.
        // label의 값으로 들어온 tag를 가지고있는 데이터는 전부 로드
        var locations = await Addressables.LoadResourceLocationsAsync(label).ToUniTask();
      
        // 반환된 경로값을 통해 로드한다.
        // AssetName을 통해 데이터를 로드하고 그룹에 추가하는 행위
        foreach (var location in locations)
        {
            if (location == null || location.ResourceType == typeof(Texture2D))
                continue;

            AsyncOperationHandle handle = CallLocationAsync(location);
            while (!handle.IsDone)
            {
                action?.Invoke(handle.PercentComplete);
                await UniTask.Yield();
            }

            await handle.ToUniTask();
            action?.Invoke(1f);

            if (handle.Status != AsyncOperationStatus.Succeeded)
                continue;

            // handle이 비제네릭이라 Result는 object로 나옴 -> UnityEngine.Object로 캐스팅
            UnityEngine.Object result = handle.Result as UnityEngine.Object;
            string Key = Path.ChangeExtension(location.PrimaryKey, null);

            if(group.Handles.ContainsKey(Key) == false)
            {
                group.Handles.Add(Key, handle);
                group.Assets.Add(Key, result);
                _cache[Key] = result;
            }
        }

        _groups.Add(label, group);
    }

    AsyncOperationHandle CallLocationAsync(IResourceLocation location)
    {
        if (location.ResourceType == typeof(SpriteAtlas))
            return Addressables.LoadAssetAsync<SpriteAtlas>(location);

        if (location.ResourceType == typeof(VisualTreeAsset))
            return Addressables.LoadAssetAsync<VisualTreeAsset>(location);

        if (location.ResourceType == typeof(Texture2D))
            return Addressables.LoadAssetAsync<Texture2D>(location);

        if (location.ResourceType == typeof(AudioClip))
            return Addressables.LoadAssetAsync<AudioClip>(location);

        return Addressables.LoadAssetAsync<UnityEngine.Object>(location);
    }

    public async UniTask<T> LoadAsync<T>(string tag, int level)
        where T : UnityEngine.Object
    {
        return await LoadAsync<T>($"{tag}_{level}");
    }

    public async UniTask<T> LoadAsync<T>(string key)
        where T : UnityEngine.Object
    {
        if (_cache.TryGetValue(key, out var cache))
            return cache as T;

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
        await handle.ToUniTask();

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
#if UNITY_EDITOR
            Debug.LogError($"Load Failed : {key}");
#endif
            return null;
        }

        _cache[key] = handle.Result;
        _handles[key] = handle;

        return handle.Result;
    }
    #endregion

    #region Get

    public T Get<T>(string key)
        where T : UnityEngine.Object
    {
        if (_cache.TryGetValue(key, out var obj))
            return obj as T;

#if UNITY_EDITOR
        Debug.LogError($"{key} is not loaded.");
#endif
        return null;
    }

    public T Get<T>(string tag, int level)
        where T : UnityEngine.Object
    {
        return Get<T>($"{tag}_{level}");
    }

    public bool TryGet<T>(string key, out T asset)
        where T : UnityEngine.Object
    {
        if (_cache.TryGetValue(key, out var obj))
        {
            asset = obj as T;
            return asset != null;
        }

        asset = null;
        return false;
    }

    public bool TryGet<T>(string tag, int level, out T asset)
        where T : UnityEngine.Object
    {
        return TryGet($"{tag}_{level}", out asset);
    }

    #endregion

    #region Release

    public void Release(string key)
    {
        if (!_handles.TryGetValue(key, out var handle))
            return;

        Addressables.Release(handle);

        _handles.Remove(key);
        _cache.Remove(key);
    }

    public void Release(string tag, int level)
    {
        Release($"{tag}_{level}");
    }

    public void ReleaseGroup(string label)
    {
        if (!_groups.TryGetValue(label, out var group))
            return;

        foreach (var pair in group.Handles)
        {
            Addressables.Release(pair.Value);
            _cache.Remove(pair.Key);
        }

        _groups.Remove(label);
    }

    public void ReleaseAll()
    {
        foreach (var group in _groups.Values)
        {
            foreach (var handle in group.Handles.Values)
            {
                Addressables.Release(handle);
            }
        }

        foreach (var handle in _handles.Values)
        {
            Addressables.Release(handle);
        }

        _groups.Clear();
        _handles.Clear();
        _cache.Clear();
    }

    #endregion

    private void OnDestroy()
    {
        ReleaseAll();
    }

    #region Default
    public static AddressableManager instance => _Instance;
    private static AddressableManager _Instance = null;

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async UniTask InitializeAsync(Action<float> action = null)
    {
        // 어드레서블 초기화
        await Addressables.InitializeAsync().ToUniTask();
        await LoadLabelAll("STATIC", action);

        Debug.Log("Addressable Load Compleleted");

    }
    #endregion
}