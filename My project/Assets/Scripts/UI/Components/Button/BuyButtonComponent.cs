using UnityEngine;
using UnityEngine.UI;

public class BuyButtonComponent : UIBase
{
    [SerializeField] private Image  _ItemIcon;
    [SerializeField] private Text   _ItemCount;
    [SerializeField] private Text   _BuyMondey;


    public void SettingData(IapBmProduct data)
    {
        string AddressableKey = data.productId;
        var AddressableMgr = AddressableManager.instance;
        if (AddressableMgr == null)
            Debug.LogWarning("[BuyButtonComponent] Not Create Addressable Manager");

     /*   var sprite = AddressableMgr.Get<Sprite>(AddressableKey);

        if (sprite == null)
            return;

        _ItemIcon.sprite = sprite;*/
        _BuyMondey.text = $"KRW *  {data.price}";
    }

    public void ClickedEvent()
    {

    }
}
