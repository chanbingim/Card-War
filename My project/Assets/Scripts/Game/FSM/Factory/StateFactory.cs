using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class StateFactory
{
    public static State Create(EFSM_STATE state, Character owner, Animator animator)
    {
        return state switch
        {
            EFSM_STATE.Idle => new IdleState(owner, animator),
            EFSM_STATE.Move => new MoveState(owner, animator),
            EFSM_STATE.Attack => new AttackState(owner, animator),
            EFSM_STATE.Hit => new HitState(owner, animator),
            EFSM_STATE.Dead => new DeadState(owner, animator),

            _ => throw new ArgumentOutOfRangeException()
        };
    }
}