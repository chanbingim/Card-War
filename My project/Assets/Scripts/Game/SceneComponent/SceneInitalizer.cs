using System.Collections.Generic;
using UnityEngine;

public class SceneInitalizer : MonoBehaviour
{
    [SerializeField] private List<IInitialize> _InitList;

    public void Initalize()
    {
        foreach (var init in _InitList)
        {
            init.Initialize();
        }
    }
}
