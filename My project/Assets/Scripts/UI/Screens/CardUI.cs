using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, 
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

        AnimCoroutine = StartCoroutine(DrawAnimationv(Pos));
    }

    IEnumerator DrawAnimationv(Vector3 Pos)
    {
        float AccTime = 0;
        Vector3 StartPosition = transform.position;

        while (AccTime < 0.5f)
        {
            AccTime += Time.deltaTime;
            transform.position = Vector3.Lerp(StartPosition, Pos, AccTime / 0.5f);
            yield return null;
        }

        transform.position = Pos;
    }
}
