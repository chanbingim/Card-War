using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using static GachaSystem;

public class GachaController : MonoBehaviour
{
    [SerializeField] private DateComponent              _DateUI;
    [SerializeField] private List<CurrencyComponent>    _CurrencyUIs;

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        var ClientMgr = GameClientManager.instance;
        if (ClientMgr == null)
        {
            Debug.Log("[CurrencyController] Not Find Client");
            return;
        }

        ClientMgr.UnSubscribeCurrencyEvent(OnChangeCurrencyValue);

        var Gachasystem = GachaSystem.instance;
        if (Gachasystem == null)
        {
            Debug.Log("[CurrencyController] Not Find Gachasystem");
            return;
        }

        Gachasystem.OnChangeBanner -= OnChangeBanner;
    }

    private void Initialize()
    {
        var ClientMgr = GameClientManager.instance;
        if (ClientMgr == null)
        {
            Debug.Log("[CurrencyController] Not Find Client");
            return;
        }

        ClientMgr.SubscribeCurrencyEvent(OnChangeCurrencyValue);
        for (ECurrency i = ECurrency.Gold; i < ECurrency.END; i++)
            OnChangeCurrencyValue(i, ClientMgr.GetCurrency(i));

        var Gachasystem = GachaSystem.instance;
        if (Gachasystem == null)
        {
            Debug.Log("[CurrencyController] Not Find Gachasystem");
            return;
        }

        Gachasystem.OnChangeBanner += OnChangeBanner;
    }

    private void OnChangeBanner(Banner banner)
    {
        if (_DateUI == null)
            return;

        _DateUI.SettingData(banner.startsAtUtc, banner.endsAtUtc);
    }

    private void OnChangeCurrencyValue(ECurrency Type, int Value)
    {
        int idx = (int)Type;
        if (_CurrencyUIs.Count <= idx)
            return;

        _CurrencyUIs[idx].SettingData(Value);
    }
}
