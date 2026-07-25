using UnityEngine;
using UnityEngine.EventSystems;

public abstract class HoverHandler : MonoBehaviour,
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    IPointerEnterHandler,
    IPointerExitHandler
#else
    IPointerDownHandler,
    IPointerUpHandler
#endif
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseHoverEvent(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseHoverEvent(false);
    }
#else
    public void OnPointerDown(PointerEventData eventData)
    {
        OnMouseHoverEvent(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnMouseHoverEvent(false);
    }
#endif

    protected abstract void OnMouseHoverEvent(bool IsHover);
}
