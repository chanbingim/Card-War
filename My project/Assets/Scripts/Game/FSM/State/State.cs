using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public abstract class State
{
    protected Character         _character;
    protected AnimComponent     _Animator;

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
