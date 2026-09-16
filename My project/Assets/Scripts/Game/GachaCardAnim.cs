using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GachaCardAnim : MonoBehaviour, IPointerClickHandler
{
    public enum ECardType { EffectCard, Character };
    public event Action OnCompelted;

    private Image _image;
    private ECardType Type;

    private int ID;
    private bool bIsSecret = true;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Initialize(int id)
    {
        var Gachasystem = GachaSystem.instance;
        if (Gachasystem == null)
        {
            Debug.Log("[GachaCardAnim] Not Find GachaSystem");
            return;
        }

        ID = id;
        bIsSecret = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bIsSecret = false;

        var DataMgr = DataManager.instance;
        if(DataMgr == null)
        {
            Debug.Log("[GachaCardAnim] Not Find DataManager");
            return;
        }

        Sprite sprite = null;
        switch (Type)
        {
            case ECardType.EffectCard:
                sprite = DataMgr.GetCardSprite(ID);
                break;
            case ECardType.Character:

                break;
        }
    }
}
