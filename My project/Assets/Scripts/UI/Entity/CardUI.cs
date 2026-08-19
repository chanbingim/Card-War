using DG.Tweening;
using System;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : UIBase, IActionDragHandler
{
    public UI_CardData  _Data { get; private set; }

    [SerializeField] private Vector3 HoverAnimScale;
    private Image       image = null;
    private Text        text = null;

    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponent<Text>();

        DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(200, 10);
    }

    public void SettingData(UI_CardData data)
    {
        _Data = data;
        image.sprite = DataManager.instance.GetCardSprite(data.CardID);
    }

    public void DrawAnimation(Vector3 Pos)
    {
        transform.DOMove(Pos, 0.5f, false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (DragManager.instance.StartDrag(this))
            image.DOFade(0, 0.3f);
    }

    protected override void OnDestroy()
    {
        image.DOKill();
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
        image.DOFade(1, 0.3f);
    }

    void IActionDragHandler.OnHovering()
    {

    }
}
