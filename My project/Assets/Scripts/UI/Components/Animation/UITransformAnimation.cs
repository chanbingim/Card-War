using DG.Tweening;
using UnityEngine;

public class UITransformAnimation : UIAnimation
{
    [SerializeField] private bool    bIsLoacl = false;
    [SerializeField] private Vector3 _Position = Vector3.zero;
    [SerializeField] private Vector3 _Scale = Vector3.zero;
    [SerializeField] private Vector3 _Rotation = Vector3.zero;

    private Transform _transform = null;

    public override void Play_Animation(Transform transform, float AnimSpeed)
    {
        if (transform == null)
            return;

        _transform = transform.gameObject.transform;
        if (bIsLoacl)
        {
            _transform.DOLocalMove(_transform.localPosition + _Position, AnimSpeed);
        }
        else
        {
            _transform.DOLocalMove(_Position, AnimSpeed);
        }

        _transform.DORotate(_Rotation, AnimSpeed);
        _transform.DOScale(_Scale, AnimSpeed);
    }

    public override void Release() 
    {
        _transform.DOKill();
    }

    private UITransformAnimation(TransformAnimData data)
    {
        _startFrame = data.StartFrame;
        _endFrame = data.EndFrame;

        bIsLoacl = data._bIsLoacl;
        _Position = data._Position;
        _Scale =    data._Scale;
        _Rotation = data._Rotation;
    }

    static public UITransformAnimation Create(TransformAnimData data)
    {
        return new UITransformAnimation(data);
    }
} 
