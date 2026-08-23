using NUnit.Framework;
using UnityEngine;

public class DeadState : State
{
    public override void Enter()
    {
        _Animator.SetInteger("State", (int)EFSM_STATE.Dead);
    }

    public override void Update()
    {
        _character.Dead();
    }

    public override void Exit()
    {
        _Animator.SetInteger("State", 0);
    }

    public DeadState(Character character, Animator _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}
