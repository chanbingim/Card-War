using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIColorAnimation : UIAnimation
{
    [SerializeField] private Color _Color;

    private Image       _image = null;

    public override void Play_Animation(Transform transform, float AnimSpeed)
    {
        if (_image == null)
            _image = transform.gameObject.GetComponent<Image>();

        _image.DOColor(_Color, AnimSpeed);
    }

    public override void Release() 
    {
        _image.DOKill();
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
