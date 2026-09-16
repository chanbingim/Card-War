using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GachaCardAnim : MonoBehaviour, IPointerClickHandler
{
    public enum ECardType { EffectCard, Character };
    public event Action OnCompelted;

    #region Orizin Data
    [SerializeField] private Sprite _OrizinImage;
    #endregion

    private Image _image;
    private ECardType Type;

    private int ID;
    private bool bIsSecret = true;

    private void Awake()
    {
        _image = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    public void Initialize(int id, Vector3 AnimTargetPoint, Action OnCompelted)
    {
        var Gachasystem = GachaSystem.instance;
        if (Gachasystem == null)
        {
            Debug.Log("[GachaCardAnim] Not Find GachaSystem");
            return;
        }

        ID = id;
        bIsSecret = true;
        _image.sprite = _OrizinImage;

        transform.position = AnimTargetPoint + Vector3.right * 10;
        transform.DOMove(AnimTargetPoint, 0.6f)
                 .OnComplete(() => OnCompelted?.Invoke());
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
