using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.AppUI.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class PayMentComponent : MonoBehaviour
{
    [SerializeField] string ItemName;
    [SerializeField] GameObject gameObject;

    public void Show()
    {
        PayMentManager.Instance.Buy(ItemName);

        gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
    }

    public void Apply()
    {
        PayMentManager.Instance.FetchPurchases();
        gameObject.SetActive(false);
    }
}
