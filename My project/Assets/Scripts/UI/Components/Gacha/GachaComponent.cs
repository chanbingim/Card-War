using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GachaComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEvent OnClicked;
    [SerializeField] private ECurrency      _CurrencyType;
    [SerializeField] private int            _UseValue;

    private ChangeCanvasComponent _ChangeCanvas = null;
    private void Awake()
    {
        _ChangeCanvas = GetComponent<ChangeCanvasComponent>();
        _ChangeCanvas.enabled = false;
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

            // 여기서 연출 호출
            _ChangeCanvas.OnPointerClick(null);
            OnClicked?.Invoke();
        }
        else
        {
            Debug.Log($"[GachaComponent] Not Has Enough Currency {_CurrencyType.ToString()}");
        }
    }
}
