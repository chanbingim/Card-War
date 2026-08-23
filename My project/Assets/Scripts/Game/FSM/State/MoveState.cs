using NUnit.Framework;
using UnityEngine;

public class MoveState : State
{
    public override void Enter()
    {
        _Animator.SetInteger("State", (int)EFSM_STATE.Move);
    }

    public override void Update()
    {
        _character.Move();
    }

    public override void Exit()
    {

    }

    public MoveState(Character character, Animator _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}
