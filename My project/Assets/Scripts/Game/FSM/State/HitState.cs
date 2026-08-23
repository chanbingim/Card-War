using NUnit.Framework;
using UnityEngine;

public class HitState : State
{
    public override void Enter()
    {
        _Animator.SetInteger("State", (int)EFSM_STATE.Move);
    }

    public override void Update()
    {
        _character.Hit();
    }

    public override void Exit()
    {
        _Animator.SetInteger("State", 0);
    }

    public HitState(Character character, Animator _animator)
    {
        _character = character;
        _Animator = _animator; 
    }
}
