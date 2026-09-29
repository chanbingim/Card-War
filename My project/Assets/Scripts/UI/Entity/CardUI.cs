using DG.Tweening;
using System;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : UIBase, IActionDragHandler
{
    public UI_CardData _Data { get; private set; }

    [SerializeField] private Vector3 HoverAnimScale;

    [SerializeField] private CanvasGroup    _CanvasGroup;
    [SerializeField] private Image          _CardIcon;
    [SerializeField] private Text           _Explanation;

    void Awake()
    {
        
    }

    public void SettingData(UI_CardData data)
    {
        if (data == null)
            return;

        _Data = data;
        _CardIcon.sprite = _Data.CardData.Icon;
        _Explanation.text = _Data.CardData.Description;
    }

    public void DrawAnimation(Vector3 Pos)
    {
        transform.DOMove(Pos, 0.5f, false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (DragManager.instance.StartDrag(this))
        {
            _CanvasGroup.DOFade(0, 0.3f);
            _CanvasGroup.interactable = false;
        }
    }

    protected override void OnDestroy()
    {
        _CanvasGroup.DOKill();
        base.OnDestroy();
    }

    void IActionDragHandler.BeginDrag()
    {
        throw new NotImplementedException();
    }

    void IActionDragHandler.OnHoverEnter()
    {
        transform.localScale = HoverAnimScale;
    }

    void IActionDragHandler.OnHoverExit()
    {
        transform.localScale = Vector3.one;
    }

    void IActionDragHandler.OnDrop(MonoBehaviour DragItem)
    {
        

    }

    void IActionDragHandler.EndDrag()
    {
        _CanvasGroup.DOFade(1, 0.3f);
        _CanvasGroup.interactable = true;
    }

    void IActionDragHandler.OnHovering()
    {

    }
}
