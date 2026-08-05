using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CurrencyComponent : MonoBehaviour
{
    [System.Serializable]
    public enum CurrencyType
    {
        Gold,          // 기본 골드
        Cash,          // 유료 재화(캐시, 보석 등)
        Energy,        // 행동력 / 스태미나
        Ticket,        // 뽑기권
        Key,           // 던전 입장 키
        Token,         // 이벤트 토큰
        END,
    }

    public CurrencyType Type => _Type;

    [SerializeField] private CurrencyType _Type;
    [SerializeField] private Image        _Icon;

    private void Start()
    {
        var AddressableMgr = AddressableManager.instance;
        if (AddressableMgr == null)
            Debug.LogWarning("[CurrencyComponent] Not Create Addressable");

        SpriteAtlas IconAtlas = AddressableMgr.Get<SpriteAtlas>("Altas/CurrencyTypeAltas");
        if (IconAtlas == null)
            Debug.Log("[CurrencyComponent] Not Find IconAtlas");

        _Icon.sprite = IconAtlas.GetSprite(_Type.ToString());
    }

    public void ClickedAddEvent()
    {
        // 여기서 팝업을 열고 Index에 맞게 재화 확인
        var UIMgr = UIManager.instance;
        if(UIMgr == null)
            Debug.LogWarning("[CurrencyComponent] Not Create UIManager");

        UIMgr.ShowAsync(UI.Enum.UIID.CashShop);
    }
}
