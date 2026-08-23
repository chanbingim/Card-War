using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "Scriptable Objects/CharacterFsmConfig")]
public class CharacterFsmConfig : ScriptableObject
{
    public List<FSMStateSO>         _States;
}
