using NUnit.Framework;
using UnityEngine;

public abstract class State
{
    protected Character _character;
    protected Animator  _Animator;

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
