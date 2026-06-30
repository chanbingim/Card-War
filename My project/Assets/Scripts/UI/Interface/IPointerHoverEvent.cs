using UnityEngine;

interface IPointerHoverEvent
{
    void OnHoverEnter();
    void OnHoverExit();

    void OnDrop(UIBase DragUI);
}