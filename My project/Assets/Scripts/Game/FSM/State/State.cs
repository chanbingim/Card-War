using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public abstract class State
{
    protected Character _character;
    protected SkeletonAnimation _Animator;

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
