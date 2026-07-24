using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster _raycaster;
    [SerializeField] private DragArrow DragArrow = null;

    private readonly List<RaycastResult> _results = new List<RaycastResult>();
    private CardUI              _CurDragUI = null;
    private IPointerHoverEvent  _hover = null;
    private PointerEventData    _Ponterevent = null;

    public bool StartDrage(CardUI UI) 
    {
        if (BattleManager.instance.IsPlayerTurn() == false)
            return false;

        _CurDragUI = UI;
        DragArrow.gameObject.SetActive(true);
        return true;
    }

    public void Darg()
    {
        if (_CurDragUI == null) return;

        IPointerHoverEvent newHover = Get_RayCast();
        Vector3 mousePos = Input.mousePosition;
        DragArrow.UpdateArrow(_CurDragUI.transform.position, mousePos);

        if(newHover == null)
            newHover = Get_WorldRayCast();

        if (newHover == _hover) return;

        _hover?.OnHoverExit();
        _hover = newHover;
        _hover?.OnHoverEnter();
    }

    public void EndDrage() 
    {
        if(_CurDragUI == null) return;

        IPointerHoverEvent newHover = Get_RayCast();
        if (newHover == null)
            newHover = Get_WorldRayCast();

        if (newHover == _hover)
            _hover?.OnDrop(_CurDragUI);

        _hover = null;
        _CurDragUI = null;
        DragArrow.gameObject.SetActive(false);
    }

    IPointerHoverEvent Get_RayCast()
    {
        _Ponterevent.position = Input.mousePosition;
        _results.Clear();
        _raycaster.Raycast(_Ponterevent, _results);

        if (_results.Count > 0)
            return _results[0].gameObject.GetComponent<IPointerHoverEvent>();

        return null;
    }

    IPointerHoverEvent Get_WorldRayCast()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = Physics2D.OverlapPoint(mousePoint);
        if(col)
            return col.GetComponent<IPointerHoverEvent>();

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
