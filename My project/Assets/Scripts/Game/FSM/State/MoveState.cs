using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public class MoveState : State
{
    public override void Enter()
    {
        _Animator.ChangeAnim(EFSM_STATE.RUN);
    }

    public override void Update()
    {
        _character.Move();
    }

    public override void Exit()
    {

    }

    public MoveState(Character character, AnimComponent _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}
