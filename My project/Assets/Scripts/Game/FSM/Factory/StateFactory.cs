using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class StateFactory
{
    public static State Create(EFSM_STATE state, Character owner, AnimComponent animator)
    {
        return state switch
        {
            EFSM_STATE.IDLE => new IdleState(owner, animator),
            EFSM_STATE.RUN => new MoveState(owner, animator),
            EFSM_STATE.ATTACK => new AttackState(owner, animator),
            EFSM_STATE.HIT => new HitState(owner, animator),
            EFSM_STATE.DEAD => new DeadState(owner, animator),

            _ => null
        };
    }
}