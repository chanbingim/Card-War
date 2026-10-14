using System.Collections.Generic;
using UnityEngine;
using static CameraShakeComponent;

public class CameraSystem
{
    Dictionary<string, CameraShakeComponent> Cameras = new Dictionary<string, CameraShakeComponent>();

    public void RegisterCamera(string key, CameraShakeComponent cam)
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

    public void ShakeCamera(string key, CameraShakeData Data)
    {
        if (Cameras.TryGetValue(key, out var Cam))
        {
            Cam.ShakeCamera(Data);
        }
    }

    #region Defualt
    public void Initialize()
    {

    }
    #endregion
}
