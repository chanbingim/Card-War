using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeObjectActiveComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] bool         _OnEnable = true;

    [SerializeField] List<GameObject> _VisibleObject;
    [SerializeField] List<GameObject> _UnVisibleObject;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeCanvas();
    }

    protected void ChangeCanvas()
    {
        foreach (GameObject obj in _VisibleObject)
        {
            obj.gameObject.SetActive(true);
        }


        foreach (GameObject obj in _UnVisibleObject)
        {
            obj.gameObject.SetActive(false);
        }
    }
}
