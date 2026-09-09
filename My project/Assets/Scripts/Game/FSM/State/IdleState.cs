using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public class IdleState : State
{
    public override void Enter()
    {
        _Animator.AnimationName = "idle_3";
        //_Animator.SetInteger("State", (int)EFSM_STATE.Idle);
    }

    public override void Update()
    {
        _character.Idle();
    }

    public override void Exit()
    {
       // _Animator.SetInteger("State", 0);
    }

    public IdleState(Character character, SkeletonAnimation _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}
