using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "Scriptable Objects/Formation")]
public class FormationSO : ScriptableObject
{
    public List<Vector3>         LocalPosition;
}
