using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DissolveComponent : MonoBehaviour
{
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private Texture2D _NoiseTex = null;

    public Material Material => _material;
    public event Action          OnCompelted;

    private Coroutine   _Dissovle = null;
    private Material    _material = null;
    private Image       _image;

    private void Awake()
    {
        _image = GetComponent<Image>();

        if (_image != null)
        {
            _material = _image.material;

            _material.SetTexture("_Base", _image.sprite.texture);
            if (_NoiseTex != null)
            {
                _material.SetTexture("_NoiseTexture", _NoiseTex);
            }
        }
    }

    public void OnDissloveAnim(bool bIsActive, bool bIsReverse = false)
    {
        if (_Dissovle != null)
            StopCoroutine(_Dissovle);

        if(bIsActive)
            gameObject.SetActive(bIsActive);

        OnCompelted += () => { };
        _Dissovle = StartCoroutine(Dissolve(bIsReverse));
    }

    IEnumerator Dissolve(bool Reverse)
    {
        float _Time = Reverse == true ? 1f : 0f;
        _material.SetFloat("_DissovleHeight", _Time);

        while (_Time >= 0f && _Time <= 1f)
        {
            if(Reverse)
                _Time -= Time.deltaTime * _speed;
            else
                _Time += Time.deltaTime * _speed;

            _material.SetFloat("_DissovleHeight", _Time);
            yield return null;
        }

        OnCompelted?.Invoke();
        _Dissovle = null;
    }
}
