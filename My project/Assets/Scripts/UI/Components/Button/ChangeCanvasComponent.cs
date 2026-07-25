using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ChangeCanvasComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] List<Canvas> _VisibleCanvas;
    [SerializeField] List<Canvas> _UnVisibleCanvas;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeCanvas();
    }

    protected void ChangeCanvas()
    {
        foreach (Canvas canvas in _VisibleCanvas)
        {
            canvas.gameObject.SetActive(true);
        }


        foreach (Canvas canvas in _UnVisibleCanvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
