using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UIBase>    _defaultUIs;
    [SerializeField] private UIDocument     _document;

    [SerializeField] private Canvas         _popupCanvas;

    private Dictionary<Type, UIBase>        _uiTable;
    private CanvasGroup                     _canvasGroup;

    private VisualElement                   _mainLayer;
    private VisualElement                   _popupLayer;

    Stack<UIBase>                           _popup_Stack = new Stack<UIBase>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Close_Popup();
        }
    }

    public void Add_UI(Type uiType)
    {
        if (_popup_Stack.Count == 0)
            Setting_CanvasGroup(true);

        Debug.Log("Show Setting Menu");
        if(_uiTable.TryGetValue(uiType, out var ui))
        {
            ui.Open();
            _popup_Stack.Push(ui);
        }
    }

    public void Clear_AllStack()
    {
        while(0 < _popup_Stack.Count)
        {
            _popup_Stack.First().Close();
            _popup_Stack.Pop();
        }

        _popup_Stack.Clear();
        Setting_CanvasGroup(false);
    }

    private void Setting_CanvasGroup(bool flag)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = flag;
            _canvasGroup.blocksRaycasts = flag;
            _canvasGroup.alpha = flag ? 1f : 0f;
        }
    }

    private void Close_Popup()
    {
        if (_popup_Stack.Count <= 0)
            return;

        _popup_Stack.First().Close();
        _popup_Stack.Pop();

        if (_popup_Stack.Count == 0)
            Setting_CanvasGroup(false);
    }

    static public UIManager instance { get; private set; }
    static UIManager _instance = null;
    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            instance = _instance;
            DontDestroyOnLoad(_instance);
        }
        else
            Destroy(this);
    }

    public async Task Initialize()
    {
        var root = _document.rootVisualElement;
        _mainLayer = root.Q<VisualElement>("MainLayer");
        _popupLayer = root.Q<VisualElement>("PopupLayer");

        _uiTable = new Dictionary<Type, UIBase>();
        foreach (var ui in _defaultUIs)
        {
            _uiTable.TryAdd(ui.GetType(), ui);
            await Task.Yield();
        }

        foreach (var ui in _defaultUIs)
        {
            _uiTable.TryAdd(ui.GetType(), ui);
            await Task.Yield();
        }

        var obj = InventoryController.Create(_popupLayer);
        _uiTable.TryAdd(typeof(InventoryController), obj);

        _canvasGroup = _popupCanvas.GetComponent<CanvasGroup>();
    }
}
