using UnityEngine;

abstract public class UIAnimation
{
    public int _startFrame  { get; protected set; }
    public int _endFrame    { get; protected set; }

    public virtual void Play_Animation(Transform transform, float AnimSpeed) { }
    public virtual void Release() { }
}
