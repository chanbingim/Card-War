using NUnit.Framework;
using UnityEngine;

public class AttackState : State
{
    public override void Enter()
    {
        _Animator.SetInteger("State", (int)EFSM_STATE.Attack);
    }

    public override void Update()
    {
        //_character.Attack();
        if (_Animator != null)
        {
            AnimatorStateInfo state = _Animator.GetCurrentAnimatorStateInfo(0);
            if (state.normalizedTime >= 1f)
            {
                _character.AnimFinished();
            }
        }
    }

    public override void Exit()
    {
        _Animator.SetInteger("State", 0);
    }

    public AttackState(Character character, Animator _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}