using UnityEngine;
using UnityEngine.UI;

public class BuyButtonComponent : UIBase
{
    [SerializeField] private Image  _ItemIcon;
    [SerializeField] private Text   _ItemCount;
    [SerializeField] private Text   _BuyMondey;

    private CurrencyProductData    _info;

    public void SettingData(CurrencyProductData data)
    {
        _info = data;

        string AddressableKey = data.CurrencyType.ToString() + data.Price.ToString();
        var AddressableMgr = AddressableManager.instance;
        if (AddressableMgr == null)
            Debug.LogWarning("[BuyButtonComponent] Not Create Addressable Manager");

     /*   var sprite = AddressableMgr.Get<Sprite>(AddressableKey);

        if (sprite == null)
            return;

        _ItemIcon.sprite = sprite;*/
        _ItemCount.text = $"º¸¼® * {_info.Amount}";
        _BuyMondey.text = $"KRW *  {_info.Price}";
    }

    public void ClickedEvent()
    {

    }
}
