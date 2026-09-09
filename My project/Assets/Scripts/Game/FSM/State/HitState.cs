using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public class HitState : State
{
    public override void Enter()
    {
        _Animator.AnimationName = "Hit";
        //_Animator.SetInteger("State", (int)EFSM_STATE.Move);
    }

    public override void Update()
    {
        _character.Hit();
    }

    public override void Exit()
    {
        _Animator.AnimationName = "Idle_3";
        //_Animator.SetInteger("State", 0);
    }

    public HitState(Character character, SkeletonAnimation _animator)
    {
        _character = character;
        _Animator = _animator; 
    }
}
