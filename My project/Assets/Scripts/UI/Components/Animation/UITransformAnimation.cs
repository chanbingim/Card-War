using DG.Tweening;
using UnityEngine;

public class UITransformAnimation : UIAnimation
{
    [SerializeField] private Vector3 _Position = Vector3.zero;
    [SerializeField] private Vector3 _Scale = Vector3.zero;
    [SerializeField] private Vector3 _Rotation = Vector3.zero;

    private RectTransform       _RcTransform = null;

    public override void Play_Animation(Transform transform, float AnimSpeed)
    {
        if (_RcTransform == null)
            _RcTransform = transform.gameObject.GetComponent<RectTransform>();

        _RcTransform.DOAnchorPos(_Position, AnimSpeed).SetEase(Ease.InBack);
        _RcTransform.DORotate(_Rotation, AnimSpeed).SetEase(Ease.InBack);
        _RcTransform.DOScale(_Scale, AnimSpeed).SetEase(Ease.InBack);
    }

    public override void Release() 
    {
        _RcTransform.DOKill();
    }

    private UITransformAnimation(TransformAnimData data)
    {
        _startFrame = data.StartFrame;
        _endFrame = data.EndFrame;
        _Position = data._Position;
        _Scale =    data._Scale;
        _Rotation = data._Rotation;
    }

    static public UITransformAnimation Create(TransformAnimData data)
    {
        return new UITransformAnimation(data);
    }
} 
