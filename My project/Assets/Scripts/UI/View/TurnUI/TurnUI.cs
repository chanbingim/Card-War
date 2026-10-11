using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TurnUI : UIBase
{
    [SerializeField] SpriteAtlas _spriteAtlas;

    Text _text = null;
    Image _image = null;
    Sprite[] _sprites = null;
    RectTransform _rectTransform = null;

    Vector3 _StatPos = Vector3.zero;
    Vector3 _EndPos = Vector3.zero;

    void Awake()
    {
        _text = GetComponentInChildren<Text>();
        _image = GetComponentInChildren<Image>();
        _rectTransform = GetComponent<RectTransform>();

        float Width = Screen.width + 200;
        _StatPos = new Vector2(-Width, 0);
        _EndPos = new Vector2(Width + 200, 0);

        _sprites = new Sprite[_spriteAtlas.spriteCount];
        _spriteAtlas.GetSprites(_sprites);
        Array.Sort(_sprites ,(item1, item2) =>
        {
            return item1.name.CompareTo(item2.name);
        });

        EventBus.Subscribe<ChangeTurnEvent>(View_TurnUI);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _rectTransform.anchoredPosition = _StatPos;
    }

    private void View_TurnUI(ChangeTurnEvent turnStartEvent)
    {
        if (turnStartEvent._IsLocal)
            _image.sprite = _sprites[0];
        else
            _image.sprite = _sprites[1];

        Sequence seq = DOTween.Sequence()
            .Append(_rectTransform.DOAnchorPos(Vector3.zero, 0.5f))
            .AppendInterval(1f)
            .Append(_rectTransform.DOAnchorPos(_EndPos, 0.5f))
            .OnComplete(() =>
             {
                 gameObject.SetActive(false);
                 turnStartEvent._OnCompleted?.Invoke();
             });

        gameObject.SetActive(true);
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<ChangeTurnEvent>(View_TurnUI);
        base.OnDestroy();
    }
}
