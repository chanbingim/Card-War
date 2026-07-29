using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private Dictionary<KeyCode, Action>     _KeyInputs = new Dictionary<KeyCode, Action>();
    List<UIBase>                            _popup_List = new List<UIBase>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Close_Popup();
        }

        foreach (var pair in _KeyInputs)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                pair.Value?.Invoke();
            }
        }
    }

    public void BindKeyAction(KeyCode key, Action action)
    {
        if (_KeyInputs.TryGetValue(key, out var BindAction))
        {
            _KeyInputs[key] = action;
        }
        else
        {
            _KeyInputs.Add(key, action);
        }
    }

    public void Clear_AllStack()
    {
        while(0 < _popup_List.Count)
        {
            var FirstUI = _popup_List.First();
            FirstUI.Close();
            _popup_List.Remove(FirstUI);
        }

        _popup_List.Clear();
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

        if(flag)
            _popupCanvas.sortingOrder = 10;
        else
            _popupCanvas.sortingOrder = 0;
    }

    public void Add_UI(Type uiType)
    {
        if (!_uiTable.TryGetValue(uiType, out var ui))
            return;

        if (_popup_List.Contains(ui))
        {
            _popup_List.Remove(ui);
        }
        else if (_popup_List.Count == 0)
        {
            Setting_CanvasGroup(true);
        }

        ui.Open();

        _popup_List.Add(ui);
        _popupCanvas.sortingOrder = 10;
    }

    private void Close_Popup()
    {
        if (_popup_List.Count <= 0)
            return;

        var LastUI = _popup_List.Last();
        LastUI.Close();
        _popup_List.Remove(LastUI);

        if (_popup_List.Count == 0)
        {
            Setting_CanvasGroup(false);
        }
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

        var obj = InventoryController.Create(_popupLayer);
        _uiTable.TryAdd(typeof(InventoryController), obj);

        foreach (var ui in _uiTable)
        {
            var type = ui.Key;
            var runtimeInstance = ui.Value.gameObject;
            if (runtimeInstance == null)
                continue;

            var systemKey = runtimeInstance.GetComponent<OpenSystemUIComponent>();
            if (systemKey == null)
                continue;

            BindKeyAction(systemKey.Key, () => Add_UI(type));
            await Task.Yield();
        }

        _canvasGroup = _popupCanvas.GetComponent<CanvasGroup>();
    }
}
