using NUnit.Framework;
using Spine.Unity;
using UnityEngine;

public class MoveState : State
{
    public override void Enter()
    {
        _Animator.AnimationName = "run_shield";
    }

    public override void Update()
    {
        _character.Move();
    }

    public override void Exit()
    {

    }

    public MoveState(Character character, SkeletonAnimation _animator)
    {
        _character = character;
        _Animator = _animator;
    }
}
