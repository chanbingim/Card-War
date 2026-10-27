using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GachaComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEvent OnClicked;
    [SerializeField] private ECurrency      _CurrencyType;
    [SerializeField] private int            _UseValue;
    [SerializeField] private int            _GachaCount = 1;

    private SpriteAnimation _Animation;

    private ChangeCanvasComponent _ChangeCanvas = null;
    private void Awake()
    {
        _ChangeCanvas = GetComponent<ChangeCanvasComponent>();
        if (_ChangeCanvas != null)
            _ChangeCanvas.enabled = false;
        _Animation = FindFirstObjectByType<SpriteAnimation>(FindObjectsInactive.Include);
    }

    public void ConfigureDraw(int count, int cost, SpriteAnimation animation)
    {
        _GachaCount = count;
        _UseValue = cost;
        _Animation = animation;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        var ClientMgr = GameClientManager.instance;
        if(ClientMgr == null)
        {
            Debug.Log("[GachaComponent] Not Find GameClientManager");
            return;
        }

        if(ClientMgr.HasEnoughCurrency(_CurrencyType, _UseValue))
        {
            ClientMgr.ADDCurrency(_CurrencyType, -_UseValue, 0);

            // ���⼭ ���� ȣ��
            _ChangeCanvas?.OnPointerClick(null);
            if (_GachaCount == 1)
                OnClicked?.Invoke();
            else
                _Animation?.PlayAnimation(_GachaCount);
        }
        else
        {
            Debug.Log($"[GachaComponent] Not Has Enough Currency {_CurrencyType.ToString()}");
        }
    }
}

