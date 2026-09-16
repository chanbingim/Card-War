using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster _raycaster;
    [SerializeField] private DragArrow DragArrow = null;

    private readonly List<RaycastResult> _results = new List<RaycastResult>();

    private MonoBehaviour       _CurDrag = null;
    private IActionDragHandler  _hover = null;
    private PointerEventData    _Ponterevent = null;

    public bool StartDrag(IActionDragHandler DragItem) 
    {
        if (BattleManager.instance.IsPlayerTurn() == false)
            return false;

        _CurDrag = DragItem as MonoBehaviour;
        DragArrow.gameObject.SetActive(true);

        return true;
    }

    public void Drag()
    {
        if (_CurDrag == null)
            return;

        Vector3 mousePos = Input.mousePosition;
        DragArrow.UpdateArrow(_CurDrag.transform.position, mousePos);

        IActionDragHandler newHover = Get_RayCast();
         if(newHover == null)
             newHover = Get_WorldRayCast();

        if (newHover == _hover) return;
        _hover?.OnHoverExit();
        _hover = newHover;
        _hover?.OnHoverEnter();

    }

    public void EndDrag() 
    {
        if(_CurDrag == null) return;

        IActionDragHandler newHover = Get_RayCast();
        if (newHover == null)
            newHover = Get_WorldRayCast();

        if (newHover == _hover)
            _hover?.OnDrop(_CurDrag);

        _hover = null;
        _CurDrag = null;
        DragArrow.gameObject.SetActive(false);
    }

    IActionDragHandler Get_RayCast()
    {
        _Ponterevent.position = Input.mousePosition;
        _results.Clear();
        _raycaster.Raycast(_Ponterevent, _results);

        if (_results.Count > 0)
            return _results[0].gameObject.GetComponent<IActionDragHandler>();

        return null;
    }

    IActionDragHandler Get_WorldRayCast()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapPoint(mousePoint);
        if(col)
            return col.GetComponent<IActionDragHandler>();

        return null;
    }

    #region Defualt
    static public DragManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        instance.Initialize();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void Initialize()
    {
        DragArrow.gameObject.SetActive(false);
        _Ponterevent = new PointerEventData(EventSystem.current);
    }
    #endregion
}
