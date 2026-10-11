using DG.Tweening;
using System;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : UIBase, IActionDragHandler
{
    public int          _ControllerIdx {  get; private set; }
    public UI_CardData  _Data { get; private set; }

    [SerializeField] private Vector3 HoverAnimScale;

    [SerializeField] private CanvasGroup    _CanvasGroup;
    [SerializeField] private Image          _CardIcon;
    [SerializeField] private Text           _Explanation;

    void Awake()
    {
        
    }

    public void SettingData(int controllerIdx, UI_CardData data)
    {
        if (data == null)
            return;

        _ControllerIdx = controllerIdx;
        _Data = data;
        _CardIcon.sprite = _Data.CardData.Icon;
        _Explanation.text = _Data.CardData.Description;
    }

    public void DrawAnimation(Vector3 Pos)
    {
        transform.DOMove(Pos, 0.5f, false);
    }

    protected override void OnDestroy()
    {
        _CanvasGroup.DOKill();
        base.OnDestroy();
    }

    void IActionDragHandler.BeginDrag()
    {
        _CanvasGroup.DOFade(0, 0.3f);
        _CanvasGroup.interactable = false;
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
        // 여기서 영역안에 검사를 진행해야할듯
        _CanvasGroup.DOFade(1, 0.3f);
        _CanvasGroup.interactable = true;
    }

    void IActionDragHandler.OnHovering()
    {

    }
}
