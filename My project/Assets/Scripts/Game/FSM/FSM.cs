using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EFSM_STATE
{
    Idle = 0,
    Attack = 1,
    Move = 2,
    Hit = 3,
    Dead = 4,
}

public class FSM : MonoBehaviour
{
    public Animator            _Animator { get; private set; }
    public State               _CurState { get; private set; } = null;
    public EFSM_STATE          _CurStateType { get; private set; }

    private Character          _Owner;
    private Dictionary<EFSM_STATE, State>       _StateTable = new();
    private Dictionary<EFSM_STATE, ulong>       _TranslateTable = new();

    public void Initialized(CharacterFsmConfig Config, Character Owner, Animator Animator)
    {
        _Owner = Owner;
        _Animator = Animator;

        if(Config != null)
        {
            CreateState(ref Config._States);
            CreateTranslate(ref Config._States);
            ChangeState(EFSM_STATE.Idle);
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

    private void CreateState(ref List<FSMStateSO> States)
    {
        foreach (var state in States)
        {
            if (!_StateTable.ContainsKey(state._StateType))
            {
                var State = StateFactory.Create(state._StateType, _Owner, _Animator);
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
