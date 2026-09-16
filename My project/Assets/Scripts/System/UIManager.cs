using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Enum;
using UnityEngine;

[Serializable]
public struct CanvasData
{
    public EUICanvas Type;
    public Canvas Canvas;

}
public class UIManager : MonoBehaviour
{
    // 스크립터블 오브젝트 UI 정보
    [SerializeField] private List<UIConfig>     _Configs;
    private Dictionary<UIID, UIConfig>          _typeConfigTable = new();
    private Dictionary<string, UIConfig>        _keyConfigTable = new();

    // 팝업 Canvas Layer
    [SerializeField] private List<CanvasData>  _Canvas;
    private Dictionary<EUICanvas, Canvas>      _CanvasTypes;

    //팝업 캔버스 Group 팝업 Sorting 용
    private CanvasGroup                     _canvasGroup;

    // PopUp List
    private List<UIBase>                    _popup_List = new List<UIBase>();

    // 캐시 테이블
    private Dictionary<string, int>         _keyCashTable = new();
    private Dictionary<UIID, int>           _typeCashTable = new();
    private List<UIBase>                    _UIList = new();

    // UI Key 세팅에 따른 동작
    private Dictionary<KeyCode, Action>     _KeyInputs = new Dictionary<KeyCode, Action>();

    #region Canvas
    public Canvas GetCanvas(EUICanvas Type)
    {
        if(_CanvasTypes.TryGetValue(Type, out var canvas))
            return canvas;

        return null;
    }
    public void RegisteCanvas(EUICanvas Type, Canvas canvas)
    {
        if(!_CanvasTypes.ContainsKey(Type))
        {
            _CanvasTypes.Add(Type, canvas);
        }
    }

    public void UnRegisteCanvas(EUICanvas Type)
    {
        if (_CanvasTypes.ContainsKey(Type))
        {
            _CanvasTypes.Remove(Type);
        }
    }
    #endregion

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

    #region PopUp Action
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

    public async UniTask ShowAsync(String key, System.Object data = null)
    {
        if (!_keyCashTable.TryGetValue(key, out var idx))
        {
            idx =  await AddUIAsync(key);
        }

        if(idx == -1)
            return ;

        if (_keyConfigTable.TryGetValue(key, out var config))
        {
            ConfigureScreen(idx, config, data);
        }
    }

    public async UniTask ShowAsync(UIID ID, System.Object data = null)
    {
        if (!_typeCashTable.TryGetValue(ID, out var idx))
        {
            idx =  await AddUIAsync(ID);
        }

        if (idx == -1)
            return;

        if (_typeConfigTable.TryGetValue(ID, out var config))
        {
            ConfigureScreen(idx, config, data);
        }
    }

    public void HideAsync(String key)
    {
        if (!_keyCashTable.TryGetValue(key, out var idx))
        {
            return;
        }

        if (_popup_List.Contains(_UIList[idx]))
            _popup_List.Remove(_UIList[idx]);

        _UIList[idx].Close();
    }

    public void HideAsync(UIID ID)
    {
        if (!_typeCashTable.TryGetValue(ID, out var idx))
        {
            return;
        }

        if (_popup_List.Contains(_UIList[idx]))
            _popup_List.Remove(_UIList[idx]);

        _UIList[idx].Close();
    }

    private void ConfigureScreen(int idx, UIConfig config, System.Object data)
    {
        if (config.CanvasType == EUICanvas.Screen_Popup)
        {
            OpenPopup(_UIList[idx]);
        }

        if(data != null)
            data = _UIList[idx];

        _UIList[idx].Open(data);
    }

    private async UniTask<int> AddUIAsync(String name)
    {
        if (_keyConfigTable.TryGetValue(name, out var config))
        {
            var UI = await CreateUserInterface(config);

            int Idx = _UIList.Count;
            _UIList.Add(UI);

            _typeCashTable.Add(config.ID, Idx);
            _keyCashTable.Add(config.name, Idx);
            return Idx;
        }
        else
        {
            Debug.Log("Not Bind UI Config");
        }

        return -1;
    }

    private async UniTask<int> AddUIAsync(UIID ID)
    {
        if (_typeConfigTable.TryGetValue(ID, out var config))
        {
            var UI = await CreateUserInterface(config);

            int Idx = _UIList.Count;
            _UIList.Add(UI);

            _typeCashTable.Add(ID, Idx);
            _keyCashTable.Add(config.AddressKey, Idx);
            return Idx;
        }
        else
        {
            Debug.Log("Not Bind UI Config");
        }

        return -1;
    }

    private async UniTask<UIBase> CreateUserInterface(UIConfig Config)
    {
        try
        {
            GameObject UserInterface = null;
            var AddressableMgr = AddressableManager.instance;
            if (AddressableMgr == null)
            {
                throw new ArgumentException("어드레서블 NULL");
            }

            var Prefab = AddressableMgr.Get<GameObject>(Config.AddressKey);
            if (Prefab == null)
            {
                Prefab = await AddressableMgr.LoadAsync<GameObject>(Config.AddressKey);
            }

            UserInterface = GameObject.Instantiate(Prefab,
                    _CanvasTypes[Config.CanvasType].transform);

            if (UserInterface == null)
                throw new ArgumentException("[UIManager] Fail Create UI");

            return UserInterface.GetComponent<UIBase>();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    private void Setting_CanvasGroup(bool flag)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = flag;
            _canvasGroup.blocksRaycasts = flag;
            _canvasGroup.alpha = flag ? 1f : 0f;
        }

        if (flag)
            _CanvasTypes[EUICanvas.Screen_Popup].sortingOrder = 10;
        else
            _CanvasTypes[EUICanvas.Screen_Popup].sortingOrder = 0;
    }
    private void OpenPopup(UIBase ui)
    {
        if(_popup_List.Contains(ui))
            _popup_List.Remove(ui);

        _popup_List.Add(ui);
        Setting_CanvasGroup(true);
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
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_popup_List.Count > 0)
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

    #region Default
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

    public async UniTask InitializeAsync()
    {
        foreach (var config in _Configs)
        {
            _keyConfigTable.Add(config.name, config);
            _typeConfigTable.Add(config.ID, config);

            if (config.Key == KeyCode.None)
                continue;

            BindKeyAction(config.Key, async() => 
            {
                if (_keyCashTable.TryGetValue(config.name, out var idx))
                {
                    if (_UIList[idx].gameObject.activeSelf)
                        HideAsync(config.name);
                    else
                        ConfigureScreen(idx, config, null);
                }
                else
                    await ShowAsync(config.name);
            });
        }

        _CanvasTypes = _Canvas.ToDictionary(value => value.Type, value => value.Canvas);
        _canvasGroup = _CanvasTypes[EUICanvas.Screen_Popup].GetComponent<CanvasGroup>();
    }
    #endregion
}
