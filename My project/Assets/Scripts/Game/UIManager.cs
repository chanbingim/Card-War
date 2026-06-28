using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UIBase>   _defaultUIs;
    [SerializeField] private Canvas         _popupCanvas;

    private Dictionary<Type, UIBase>        _uiTable;
    private CanvasGroup                     _canvasGroup;
    Stack<UIBase>                           _popup_Stack = new Stack<UIBase>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Close_Popup();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Clear_AllStack();
        }

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            Add_UI(typeof(SettingMenu));
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            Add_UI(typeof(UIBase));
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
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            instance = _instance;

            instance.Initialize();
            DontDestroyOnLoad(_instance);
        }
        else
            Destroy(this);
    }

    private void Initialize()
    {
        _uiTable = new Dictionary<Type, UIBase>();
        foreach (var ui in _defaultUIs)
        {
            _uiTable.TryAdd(ui.GetType(), ui);
        }

        _canvasGroup = _popupCanvas.GetComponent<CanvasGroup>();
    }
}
