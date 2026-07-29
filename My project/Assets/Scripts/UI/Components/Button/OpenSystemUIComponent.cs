using System;
using Unity.VisualScripting;
using UnityEngine;

public class OpenSystemUIComponent : MonoBehaviour
{
    [SerializeField] private KeyCode _Key;
    public KeyCode Key => _Key;
   
}
