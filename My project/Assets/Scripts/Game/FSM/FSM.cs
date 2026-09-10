using Google.MiniJSON;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EFSM_STATE
{
    IDLE = 0,
    ATTACK,
    WALK,
    RUN,
    STUN,
    HIT,
    BUFF,
    DEAD,
}

public class FSM : MonoBehaviour
{
    public State               _CurState { get; private set; } = null;
    public EFSM_STATE          _CurStateType { get; private set; }

    private Character          _Owner;
    private Dictionary<EFSM_STATE, State>       _StateTable = new();
    private Dictionary<EFSM_STATE, ulong>       _TranslateTable = new();

    public void Initialized(CharacterFsmConfig Config, Character Owner, AnimComponent animationComponent)
    {
        _Owner = Owner;

        if(Config != null)
        {
            CreateState(animationComponent, ref Config._States);
            CreateTranslate(ref Config._States);
            ChangeState(EFSM_STATE.IDLE);
        }
    }

    public void UpdateFSM()
    {
        if (_CurState == null)
            return;

        _CurState.Update();
    }

    public void ChangeState(EFSM_STATE _state)
    {
        if (_CurState != null)
        {
            _CurState.Exit();
        }

        _CurStateType = _state;
        if(_StateTable.TryGetValue(_state, out var state))
        {
            _CurState = state;
            _CurState.Enter();
        }
    }

    private void CreateState(AnimComponent animationComponent, ref List<FSMStateSO> States)
    {
        foreach (var state in States)
        {
            if (!_StateTable.ContainsKey(state._StateType))
            {
                var State = StateFactory.Create(state._StateType, _Owner, animationComponent);
                _StateTable.Add(state._StateType, State);
            }
        }
    }

    private void CreateTranslate(ref List<FSMStateSO> States)
    {
        foreach (var state in States)
        {
            if (!_TranslateTable.ContainsKey(_CurStateType))
            {
                ulong flag = 0;
                foreach (var transition in state._Translation)
                {
                    flag |= 1UL << (int)transition;
                }

                _TranslateTable.Add(_CurStateType, flag);
            }
        }
    }
}
