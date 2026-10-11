using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

public class BuyPopup : UIBase
{
    [SerializeField] private List<Button>               _SelectButton;
    [SerializeField] private List<BuyButtonComponent>   _BuyitemList;
    [SerializeField] private Scrollbar          _ItemListScroll;

    private ECurrency _SelectType = ECurrency.END;

    private void Start()
    {
        for(int i = 0; i < _SelectButton.Count; i++)
        {
            int index = i;
            _SelectButton[i].gameObject.GetComponentInChildren<Text>().text = ((ECurrency)i).ToString();
            _SelectButton[i].onClick.AddListener(() =>
            {
                ButtonSelectEvent((ECurrency)index);
            });
        }
    }

    public override void Open(System.Object data = null)
    {
        base.Open(data);
        
        if (_SelectType == ECurrency.END)
            ButtonSelectEvent(ECurrency.Gold);
        else
        {
            if (data != null)
                ButtonSelectEvent((ECurrency)data);
        }
    }

    public void ButtonSelectEvent(ECurrency type)
    {
        if (_SelectType == type)
            return;

        _SelectType = type;
        _ItemListScroll.value = 0;

        // 여기서 캐싱해둔 Data에 접근해서 확인
        List<CurrencyProductData> ItemList = DataManager.instance.GetBMData(_SelectType);
        if (ItemList == null)
            return;

        for(int i = 0; i < _BuyitemList.Count; i++)
        {
            if (ItemList.Count <= i)
            {
                _BuyitemList[i].Close();
            }
            else
            {
                _BuyitemList[i].Open();
                _BuyitemList[i].SettingData(ItemList[i]);
            }
        }
    }
}
