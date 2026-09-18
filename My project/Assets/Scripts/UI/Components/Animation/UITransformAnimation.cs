using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class UITransformAnimation : UIAnimation
{
    [SerializeField] private bool    bIsLoacl = false;
    [SerializeField] private Vector3 _Position = Vector3.zero;
    [SerializeField] private Vector3 _Scale = Vector3.zero;
    [SerializeField] private Vector3 _Rotation = Vector3.zero;

    private Transform _transform = null;
    private TweenerCore<Vector3, Vector3, VectorOptions>        _ChangePosition = null;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> _ChangeRotation = null;
    private TweenerCore<Vector3, Vector3, VectorOptions>        _ChangeScale = null;

    public override void Initialize(Transform transform) 
    {
        float AnimSpeed = 1.0f;
        if (_transform == null)
        {
            _transform = transform.gameObject.transform;

            if (bIsLoacl)
            {
                _ChangePosition = _transform.DOLocalMove(transform.position + _Position, AnimSpeed)
                                            .SetAutoKill(false);
            }
            else
            {
                _ChangePosition = _transform.DOLocalMove(_Position, AnimSpeed)
                                            .SetAutoKill(false);
            }

            _ChangeRotation = _transform.DORotate(_Rotation, AnimSpeed)
                                        .SetAutoKill(false);

            _ChangeScale = _transform.DOScale(_Scale, AnimSpeed)
                                     .SetAutoKill(false);
        }
    }

    public override void Play_Animation(float AnimSpeed, bool IsReverse)
    {
        if (IsReverse)
        {
            _transform.position = _ChangePosition.endValue;
            _transform.eulerAngles = _ChangeRotation.endValue;
            _transform.localScale = _ChangeScale.endValue;

            _ChangePosition.Goto(_ChangePosition.Duration(), false);
            _ChangePosition.PlayBackwards();

            _ChangeRotation.Goto(_ChangeRotation.Duration(), false);
            _ChangeRotation.PlayBackwards();

            _ChangeScale.Goto(_ChangeScale.Duration(), false);
            _ChangeScale.PlayBackwards();
        }
        else
        {
            _transform.position = _ChangePosition.startValue;
            _transform.eulerAngles = _ChangeRotation.startValue;
            _transform.localScale = _ChangeScale.startValue;

            _ChangePosition.Restart();
            _ChangeRotation.Restart();
            _ChangeScale.Restart();
        }
    }

    public override void Release() 
    {

    }

    private void OnDestroy()
    {
        _ChangePosition?.Kill();
        _ChangeRotation?.Kill();
        _ChangeScale?.Kill();

        _ChangePosition = null;
        _ChangeRotation = null;
        _ChangeScale = null;
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
