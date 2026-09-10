using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public class DeadState : State
{
    public override void Enter()
    {
        _Animator.ChangeAnim(EFSM_STATE.DEAD);
    }

    public override void Update()
    {
        _character.Dead();
    }

    public override void Exit()
    {
       
    }

    public DeadState(Character character, AnimComponent _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}
