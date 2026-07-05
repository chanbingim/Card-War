using System;
using UnityEngine;
using UnityEngine.UI;

public class UV_Scroll : MonoBehaviour
{
    [SerializeField] private Vector2  _Direction = Vector2.zero;
    [SerializeField] private float    _Speed = 5f;

    private Image                     _image = null;
    private Vector2                   _Offset = Vector2.zero;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        Update_Matrial();
    }

    private void Update_Matrial()
    {
        if (_image != null)
        {
            _image.material.SetFloat("_Speed", _Speed);
        }
    }
}
