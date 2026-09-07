using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    struct FontData
    {
        public FontData(int texIdx)
        {
            Texindex = texIdx;
        }

        public int          Texindex;
    }

    [SerializeField] int        _ViewFontCount = 10;
    [SerializeField] Texture    _DamageTexture = null;
    [SerializeField] Mesh       _Mesh = null;
    [SerializeField] Material _material = null;

    InstancingComponent         _InstancingComponent = null;
    PoolAbleComponent           _PoolAbleComponent = null;
    DoTweenAnimator             _DoTweenAnimator = null;

    FontData[]                  _InstanceBuffer = null;
    Coroutine                   _AnimCoroutine;

    private void Awake()
    {
        if( _InstanceBuffer == null )
        {
            _InstanceBuffer = new FontData[_ViewFontCount];
            for (int i = 0; i < _ViewFontCount; ++i)
            {
                _InstanceBuffer[i] = new FontData(0);
            }
        }

        _InstancingComponent = GetComponent<InstancingComponent>();

        if (_InstancingComponent != null)
        {
            _InstancingComponent.Initailize(_Mesh, _material, _ViewFontCount, Marshal.SizeOf<FontData>());
        }

        _DoTweenAnimator = GetComponent<DoTweenAnimator>();
        _PoolAbleComponent = GetComponent<PoolAbleComponent>();

        _DoTweenAnimator.Pause_Animation();

        _material.SetTexture("_BaseMap", _DamageTexture);
    }

    public void Initalize(Int64 Damage, Transform transformParent = null)
    {
        List<Int64> Data = new List<Int64>();
        while(Damage > 0)
        {
            Data.Add(Damage % 10);
            Damage /= 10;
        }

        int Count = 0;
        int half = Data.Count / 2;

        if (transformParent)
        {
            transform.parent = transformParent;
            transform.localPosition = Vector3.up;
        }

        for (int i = Data.Count - 1; i >= 0; i--)
        {
            var instance = _InstanceBuffer[Count];
            instance.Texindex = (int)Data[i];

            _InstanceBuffer[Count] = instance;
            Count++;
        }

        _material.SetFloat("_Count", Data.Count);
        _InstancingComponent.SetData(Count, _InstanceBuffer);
        if (_AnimCoroutine != null)
            StopCoroutine(_AnimCoroutine);

        _AnimCoroutine = StartCoroutine(AnimCorutine(_DoTweenAnimator.GetToatalAnimTime()));
    }

    private void LateUpdate()
    {
        _material.SetVector("_WorldPosition", transform.position);
        _material.SetVector("_Scale", transform.localScale);
        _InstancingComponent.OnDraw();
    }

    IEnumerator AnimCorutine(float LifeTime)
    {
        float time = 0;
        if (_DoTweenAnimator == null)
            yield return null;

        _DoTweenAnimator.Play_Animation();

        while (time < LifeTime)
        {
            time += Time.deltaTime;
            float Ratio = 1 - (time / LifeTime);
            _material.SetFloat("_ParentAlpha", Ratio);

            yield return null;
        }

        _DoTweenAnimator.Pause_Animation();

        // 여기에서 객체 반환 및 삭제를 진행
        transform.parent = null;
        _PoolAbleComponent?.ReturnToPool();
    }
}
