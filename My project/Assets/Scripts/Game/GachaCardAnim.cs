using DG.Tweening;
using GAME_CONST;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GachaCardAnim : MonoBehaviour, IPointerClickHandler
{
    public enum ECardType { EffectCard, Character };
    public event Action OnCompelted;

    public bool         bIsSecret { get; private set; } = true;

    #region Orizin Data
    [SerializeField] private DissolveComponent _dissolveComponent = null;
    //private Renderer _renderer;
    #endregion

    private int         ID;

   // private MaterialPropertyBlock materialPropertyBlock;

    private void Awake()
    {
        _dissolveComponent = GetComponent<DissolveComponent>();
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
       

      // _renderer = GetComponent<Renderer>();
      //
      // materialPropertyBlock = new MaterialPropertyBlock();
      // _renderer.GetPropertyBlock(materialPropertyBlock);
      //
        if (!SettingSpriteImage())
            return;

        _dissolveComponent.Material.SetFloat("_DissovleHeight", 0);
        transform.position = AnimTargetPoint + Vector3.right * 900f;
        transform.DOMove(AnimTargetPoint, 0.6f)
                 .OnComplete(() => OnCompelted?.Invoke());

        ApplyMaterialData();
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
        ApplyMaterialData();
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

        _dissolveComponent.Material.SetTexture("_ItemImage", CardData.Icon.texture);
        _dissolveComponent.Material.SetVector("_ItemImageUVRect", CardData.SpriteUV);

        ApplyMaterialData();

        return true;
    }

    private void ApplyMaterialData()
    {
        //_renderer.SetPropertyBlock(materialPropertyBlock);
    }
}

