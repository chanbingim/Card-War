using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TurnUI : UIBase
{
    Text            _text = null;
    RectTransform   _rectTransform = null;

    Vector3         _StatPos = Vector3.zero;
    Vector3         _EndPos = Vector3.zero;

    void Awake()
    {
        _text = GetComponentInChildren<Text>();
        _rectTransform = GetComponent<RectTransform>();

        float Width = Screen.width + 200;
        _StatPos = new Vector2(-Width, 0);
        _EndPos = new Vector2(Width + 200, 0);

        EventBus.Subscribe<TurnStartEvent>(View_TurnUI);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _rectTransform.anchoredPosition = _StatPos;
    }

    private void View_TurnUI(TurnStartEvent turnStartEvent)
    {
        _text.text = turnStartEvent.Name;
        Sequence seq = DOTween.Sequence()
            .Append(_rectTransform.DOAnchorPos(Vector3.zero, 0.5f))
            .AppendInterval(1f)
            .Append(_rectTransform.DOAnchorPos(_EndPos, 0.5f))
             .OnComplete(() =>
             {
                 gameObject.SetActive(false);
             });

        gameObject.SetActive(true);
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<TurnStartEvent>(View_TurnUI);
        base.OnDestroy();
    }
}
