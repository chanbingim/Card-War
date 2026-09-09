using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public class DeadState : State
{
    public override void Enter()
    {
        _Animator.AnimationName = "Dead";
        //_Animator.SetInteger("State", (int)EFSM_STATE.Dead);
    }

    public override void Update()
    {
        _character.Dead();
    }

    public override void Exit()
    {
       
    }

    public DeadState(Character character, SkeletonAnimation _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}
