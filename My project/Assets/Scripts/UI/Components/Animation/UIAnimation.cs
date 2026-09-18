using UnityEngine;

abstract public class UIAnimation
{
    public int _startFrame  { get; protected set; }
    public int _endFrame    { get; protected set; }

    public virtual void Initialize(Transform transform) { }
    public virtual void Play_Animation( float AnimSpeed, bool sReverse = false) { }
    public virtual void Release() { }
}
