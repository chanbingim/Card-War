using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DissolveComponent : MonoBehaviour
{
    [SerializeField] private float _speed = 0.5f;

    private Coroutine   _Dissovle = null;
    private Material    _matrial = null;
    private Image       _image;

    private void Awake()
    {
        _image = GetComponent<Image>();

        if (_image != null)
        {
            _matrial = _image.material;
            _matrial.SetTexture("_Base", _image.sprite.texture);
        }
    }

    public void OnDissloveAnim(bool bIsActive)
    {
        if (_Dissovle != null)
            StopCoroutine(_Dissovle);

        if(bIsActive)
            gameObject.SetActive(bIsActive);

        _Dissovle = StartCoroutine(Dissolve(!bIsActive, () =>
        {
            gameObject.SetActive(bIsActive);
        }));
    }

    IEnumerator Dissolve(bool Reverse, Action OnCompleted = null)
    {
        float _Time = Reverse == true ? 1f : 0f;
        _matrial.SetFloat("_DissovleHeight", _Time);

        while (_Time >= 0f && _Time <= 1f)
        {
            if(Reverse)
                _Time -= Time.deltaTime * _speed;
            else
                _Time += Time.deltaTime * _speed;

            _matrial.SetFloat("_DissovleHeight", _Time);
            yield return null;
        }

        OnCompleted?.Invoke();
        _Dissovle = null;
    }
}
