using DG.Tweening;
using System;
using System.Collections;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : UIBase, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public Boolean      _IsHover { get; private set; }
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
        image.sprite = PlayerDataManager.instance.Get_CardImage(_Data.CardID);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = HoverAnimScale;
        _IsHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        _IsHover = false;
    }
    
    public void DrawAnimation(Vector3 Pos)
    {
        transform.DOMove(Pos, 0.5f, false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (DragManager.instance.StartDrage(this))
            image.DOFade(0, 0.3f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragManager.instance.Darg();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.DOFade(1, 0.3f);
        DragManager.instance.EndDrage();
    }

    protected override void OnDestroy()
    {
        image.DOKill();
        base.OnDestroy();
    }
}
