using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CurrencyComponent;

public class BuyPopup : UIBase
{
    [SerializeField] private List<Button>               _SelectButton;
    [SerializeField] private List<BuyButtonComponent>   _BuyitemList;
    [SerializeField] private Scrollbar          _ItemListScroll;

    private CurrencyType _SelectType = CurrencyType.END;

    private void Start()
    {
        for(int i = 0; i < _SelectButton.Count; i++)
        {
            int index = i;
            _SelectButton[i].gameObject.GetComponentInChildren<Text>().text = ((CurrencyType)i).ToString();
            _SelectButton[i].onClick.AddListener(() =>
            {
                ButtonSelectEvent((CurrencyType)index);
            });
        }
    }

    public override void Open(System.Object data = null)
    {
        base.Open(data);
        
        if (_SelectType == CurrencyType.END)
            ButtonSelectEvent(CurrencyType.Gold);
        else
        {
            if (data != null)
                ButtonSelectEvent((CurrencyType)data);
        }
    }

    public void ButtonSelectEvent(CurrencyType type)
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
