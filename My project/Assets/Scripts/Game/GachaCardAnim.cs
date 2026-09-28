using DG.Tweening;
using GAME_CONST;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class GachaCardAnim : MonoBehaviour, IPointerClickHandler
{
    public enum ECardType { EffectCard, Character };
    public event Action OnCompelted;

    public bool         bIsSecret { get; private set; } = true;

    #region Orizin Data
    [SerializeField] private DissolveComponent _dissolveComponent = null;
    #endregion
    
    private int         ID;
    private ECardType   Type;

    private void Awake()
    {
        _dissolveComponent = GetComponent<DissolveComponent>();
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
        if (!SettingSpriteImage())
            return;

        bIsSecret = true;
        transform.position = AnimTargetPoint + Vector3.right * 10;
        transform.DOMove(AnimTargetPoint, 0.6f)
                 .OnComplete(() => OnCompelted?.Invoke());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(bIsSecret)
        {
            bIsSecret = false;

            var DataMgr = DataManager.instance;
            if (DataMgr == null)
            {
                Debug.Log("[GachaCardAnim] Not Find DataManager");
                return;
            }

            _dissolveComponent?.OnDissloveAnim(true);
        }
    }

    public void SetSpriteTexture()
    {
        bIsSecret = false;
        _dissolveComponent.Material.SetFloat("_DissovleHeight", 1);
    }

    private bool SettingSpriteImage()
    {
        var DataMgr = DataManager.instance;
        if(DataMgr == null)
        {
            Debug.Log("[GachaCardAnim] Not Find DataManager");
            return false;
        }

        var CardData = DataMgr.GetCardById(ID);
        if (CardData == null)
            return false;

        var AddressableMgr = AddressableManager.instance;
        if (AddressableMgr == null)
        {
            Debug.Log("[GachaCardAnim] Not Find AddressableManager");
            return false;
        }

        var Atlas = AddressableMgr.Get<SpriteAtlas>(Const.CharacterIconAddress);
        if (Atlas == null)
            return false;

        _dissolveComponent.Material.SetTexture("_ItemTexture", Atlas.GetSprite(CardData.TextureKey).texture);
        return true;
    }
}
