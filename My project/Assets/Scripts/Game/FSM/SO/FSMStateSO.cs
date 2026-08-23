using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "Scriptable Objects/FSMStateSO")]
public class FSMStateSO : ScriptableObject
{
    public EFSM_STATE       _StateType;
    public List<EFSM_STATE> _Translation;
}
