using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public class AttackState : State
{
    public override void Enter()
    {
        _Animator.AnimationName = "shield_attack";
        //_Animator.SetInteger("State", (int)EFSM_STATE.Attack);
    }

    public override void Update()
    {
        //_character.Attack();
    }

    public override void Exit()
    {
        _Animator.AnimationName = "Idle_3";
        // _Animator.SetInteger("State", 0);
    }

    public AttackState(Character character, SkeletonAnimation _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}