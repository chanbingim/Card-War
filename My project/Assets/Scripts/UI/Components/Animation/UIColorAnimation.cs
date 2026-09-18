using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class UIColorAnimation : UIAnimation
{
    [SerializeField] private Color _Color;

    private TweenerCore<Color, Color, ColorOptions> _ColorChangeTween = null;
    private Image       _image = null;

    public override void Initialize(Transform transform) 
    {
        float AnimSpeed = 1.0f;
        if (_image == null)
        {
            _image = transform.gameObject.GetComponent<Image>();

        }

        if (_ColorChangeTween == null)
        {
            _ColorChangeTween = _image.DOColor(_Color, AnimSpeed)
                                      .SetAutoKill(false);
        }
    }

    public override void Play_Animation(float AnimSpeed, bool IsReverse)
    {
        if (IsReverse)
        {
            // 목표 색상에서 원본 색상으로
           _ColorChangeTween.Goto(_ColorChangeTween.Duration(), false);

            _image.color = _ColorChangeTween.endValue;
            _ColorChangeTween.PlayBackwards();
        }
        else
        {
            _image.color = _ColorChangeTween.startValue;
            _ColorChangeTween.Restart();
        }
    }

    public override void Release() 
    {
     
    }

    private void OnDestroy()
    {
        _ColorChangeTween?.Kill();
        _ColorChangeTween = null;
    }

    private UIColorAnimation(UIColorAnimData data)
    {
        _startFrame = data.StartFrame;
        _endFrame = data.EndFrame;
        _Color = data._Color;
    }

    static public UIColorAnimation Create(UIColorAnimData data)
    {
        return new UIColorAnimation(data);
    }
} 
