using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GachaComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEvent OnClicked;
    [SerializeField] private ECurrency      _CurrencyType;
    [SerializeField] private int            _UseValue;

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
            OnClicked?.Invoke();
        }
        else
        {
            Debug.Log($"[GachaComponent] Not Has Enough Currency {_CurrencyType.ToString()}");
        }
    }
}
