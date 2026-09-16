using UnityEngine;
using UnityEngine.EventSystems;

public interface IActionDragHandler :
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (DragManager.instance.StartDrag(this))
        {
            BeginDrag();
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        DragManager.instance.Drag();
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        EndDrag();
        DragManager.instance.EndDrag();
    }

    void BeginDrag();
    void OnDrop(MonoBehaviour DragItem);
    void EndDrag();

    void OnHoverEnter();
    void OnHovering();
    void OnHoverExit();
}