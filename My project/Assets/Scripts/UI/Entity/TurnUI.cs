using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnUI : UIBase
{
    [SerializeField] List<Sprite> _sprites;

    Text _text = null;
    Image _image = null;
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

        EventBus.Subscribe<TurnUIEvent>(View_TurnUI);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _rectTransform.anchoredPosition = _StatPos;
    }

    private void View_TurnUI(TurnUIEvent turnStartEvent)
    {
        var Player = BattleManager.instance.GetLoaclPlayer();
        if (0 == (Player.Name.CompareTo(turnStartEvent.Name)))
            _image.sprite = _sprites[0]; 
        else
            _image.sprite = _sprites[1];

        _text.text = turnStartEvent.Name;
        Sequence seq = DOTween.Sequence()
            .Append(_rectTransform.DOAnchorPos(Vector3.zero, 0.5f))
            .AppendInterval(1f)
            .Append(_rectTransform.DOAnchorPos(_EndPos, 0.5f))
             .OnComplete(() =>
             {
                 gameObject.SetActive(false);
                 EventBus.Publish<TurnStartEvent>(new TurnStartEvent(_text.text));
             });

        gameObject.SetActive(true);
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<TurnUIEvent>(View_TurnUI);
        base.OnDestroy();
    }
}
