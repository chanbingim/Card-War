using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CameraSystem
{
    Dictionary<string, Camera> Cameras = new Dictionary<string, Camera>();

    public void RegisterCamera(string key, Camera cam)
    {
        if(Cameras.ContainsKey(key))
        {
            Debug.Log("[Camera System] Already Camera Key");
            return;
        }

        Cameras.Add(key, cam);
    }

    public void UnRegisterCamera(string key)
    {
        if (Cameras.ContainsKey(key) == false)
        {
            Debug.Log("[Camera System] Not Find Camera Key");
            return;
        }

        Cameras.Remove(key);
    }

    public void ShakeCamera(string key)
    {
        if (Cameras.TryGetValue(key, out var Cam))
        {
            



        }
    }

    #region Defualt
    static public CameraSystem instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            return;
        }

        instance = this;
        instance.Initialize();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void Initialize()
    {

    }
    #endregion
}
