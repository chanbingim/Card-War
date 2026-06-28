using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardUI : UIBase, 
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Boolean   _IsHover { get; private set; }
    public int       _CardID { get; private set; }

    [SerializeField] private Vector3 HoverAnimScale;
    private Image       image = null;
    private Text        text = null;
    private Coroutine   AnimCoroutine = null;

    void Start()
    {
        image = GetComponent<Image>();
        text = GetComponent<Text>();

        DOTween.Init();
        DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(200, 10);
    }

    public void SettingData(int CardID)
    {
        _CardID = CardID;
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
        if(AnimCoroutine != null)
            StopCoroutine(AnimCoroutine);

        transform.DOMove(Pos, 0.5f, false);
    }
}
