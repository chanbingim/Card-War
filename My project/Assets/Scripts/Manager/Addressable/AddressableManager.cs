using GamePlay.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    public async Task LoadLabelAll(string label)
    {
        if (_groups.ContainsKey(label))
            return;

        AddressableGroup group = new(label);
        
        // 함수는 Label의 Tag에 따라서 데이터를 로드한다.
        // label의 값으로 들어온 tag를 가지고있는 데이터는 전부 로드
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;

        // 반환된 경로값을 통해 로드한다.
        // AssetName을 통해 데이터를 로드하고 그룹에 추가하는 행위
        foreach (var location in locations)
        {
            if (location == null)
                continue;

            var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(location);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                continue;

            group.Handles.Add(location.PrimaryKey, handle);
            group.Assets.Add(location.PrimaryKey, handle.Result);

            _cache[location.PrimaryKey] = handle.Result;
        }

        _groups.Add(label, group);
    }

    public async Task<T> LoadAsync<T>(string tag, int level)
        where T : UnityEngine.Object
    {
        return await LoadAsync<T>($"{tag}_{level}");
    }

    public async Task<T> LoadAsync<T>(string key)
        where T : UnityEngine.Object
    {
        if (_cache.TryGetValue(key, out var cache))
            return cache as T;

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
        await handle.Task;

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
        _Instance.InitializeAsync();

        DontDestroyOnLoad(gameObject);
    }

    public async Task InitializeAsync()
    {
        // 어드레서블 초기화
        await Addressables.InitializeAsync().Task;
        await LoadLabelAll("STATIC");

        Debug.Log("Addressable Load Compleleted");

    }
    #endregion
}