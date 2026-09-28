using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCanvasComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] bool         _OnEnable = true;

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
