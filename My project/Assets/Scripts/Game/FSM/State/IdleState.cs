using NUnit.Framework;
using UnityEngine;

public class IdleState : State
{
    public override void Enter()
    {
        _Animator.SetInteger("State", (int)EFSM_STATE.Idle);
    }

    public override void Update()
    {
        _character.Idle();
    }

    public override void Exit()
    {
        _Animator.SetInteger("State", 0);
    }

    public IdleState(Character character, Animator _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}
