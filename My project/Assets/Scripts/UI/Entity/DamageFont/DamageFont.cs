using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DamageFont : MonoBehaviour
{
    struct FontData
    {
        public FontData(int texIdx, Matrix4x4 matrix4X4)
        {
            Texindex = texIdx;
            WorldMatrix = matrix4X4;
        }

        public int          Texindex;
        public Matrix4x4    WorldMatrix;
    }

    [SerializeField] int        _ViewFontCount = 10;
    [SerializeField] Texture    _DamageTexture = null;
    [SerializeField] Mesh       _Mesh = null;
    [SerializeField] Material   _material = null;

    InstancingComponent         _InstancingComponent = null;
    FontData[]                  _InstanceBuffer = null;
    Coroutine                   _AnimCoroutine;

    private void Awake()
    {
        if( _InstanceBuffer == null )
        {
            _InstanceBuffer = new FontData[_ViewFontCount];
            for (int i = 0; i < _ViewFontCount; ++i)
            {
                _InstanceBuffer[i] = new FontData(0, Matrix4x4.identity);
            }
        }

        _InstancingComponent = GetComponent<InstancingComponent>();
        if (_InstancingComponent != null)
        {
            _InstancingComponent.Initailize(_Mesh, _material, _ViewFontCount, Marshal.SizeOf<FontData>());
        }

        _material.SetTexture("_BaseMap", _DamageTexture);
    }

    private void Start()
    {
        Initalize(1234567, 10);
    }

    public void Initalize(Int64 Damage, int LifeTime)
    {
        List<Int64> Data = new List<Int64>();
        while(Damage > 0)
        {
            Data.Add(Damage % 10);
            Damage /= 10;
        }

        int Count = 0;
        int half = Data.Count / 2;
        Vector3 pos = transform.position;
        for (int i = Data.Count - 1; i >= 0; i--)
        {
            var instance = _InstanceBuffer[Count];
            instance.Texindex = (int)Data[i];

            instance.WorldMatrix =
                Matrix4x4.TRS(pos + Vector3.right * (Count - half),
                              Quaternion.identity,
                              Vector3.one);

            _InstanceBuffer[Count] = instance;
            Count++;
        }

        _InstancingComponent.SetData(Count, _InstanceBuffer);
        if (_AnimCoroutine != null)
            StopCoroutine(_AnimCoroutine);

        _AnimCoroutine = StartCoroutine(AnimCorutine(LifeTime, Data.Count));
    }

    private void LateUpdate()
    {
       
    }

    IEnumerator AnimCorutine(int LifeTime, int DataCount)
    {
        float time = 0;
        int half = DataCount / 2;

        while (time < LifeTime)
        {
            time += Time.deltaTime;
            transform.position += Vector3.up * Time.deltaTime * 1f;
            _material.SetVector("_TransformOffset", new Vector4(0, transform.position.y, 0, 0));

            _InstancingComponent.OnDraw();
            yield return null;
        }
    }
}
