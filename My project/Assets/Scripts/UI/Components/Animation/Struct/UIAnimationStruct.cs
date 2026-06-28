using System;
using UnityEngine;

[Serializable]
public abstract class UIAnimData
{
    [SerializeField] protected int _startFrame;
    [SerializeField] protected int _endFrame;

    public int StartFrame => _startFrame;
    public int EndFrame => _endFrame;

    public abstract UIAnimation Create();
}

[Serializable]
public class TransformAnimData : UIAnimData
{
    public Vector3 _Position;
    public Vector3 _Scale;
    public Vector3 _Rotation;

    public override UIAnimation Create()
    {
        return UITransformAnimation.Create(this);
    }
}