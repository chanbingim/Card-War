using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CurrencyComponent : MonoBehaviour
{
    public ECurrency        Type => _Type;

    [SerializeField] private ECurrency    _Type;
    [SerializeField] private Image        _Icon;
    [SerializeField] private Text         _text;

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

    public void SettingData(int Value)
    {
        _text.text = Value.ToString();
    }

    public void ClickedAddEvent()
    {
        // 여기서 팝업을 열고 Index에 맞게 재화 확인
        var UIMgr = UIManager.instance;
        if(UIMgr == null)
            Debug.LogWarning("[CurrencyComponent] Not Create UIManager");

        if (_Type == ECurrency.Cash)
            UIMgr.ShowAsync(UI.Enum.UIID.CashShop);
        else
            GameClientManager.instance.ADDCurrency(Type, 1000, 0);
    }
}
