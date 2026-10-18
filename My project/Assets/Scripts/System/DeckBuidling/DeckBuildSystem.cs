using System;
using UnityEngine;

public class DeckBuildSystem : MonoBehaviour
{
    public event Func<int, (RectTransform Transform, Sprite Sprite)> RequestCollectionItem;
    public event Func<int, RectTransform> RequestDeckItem;

    public event Action<RectTransform, RectTransform, Sprite> DeckCardTransferRequested;

    public bool RequestAddDeck(int cardID, int count)
    {
        var GameClient = GameClientManager.instance;
        if(GameClient == null)
        {
            Debug.Log("[Deck Build System] GameClient Not Find");
            return false;
        }

        if (GameClient._playerData == null || RequestCollectionItem == null)
            return false;

        var source = RequestCollectionItem.Invoke(cardID);
        if (source.Transform == null || source.Sprite == null)
            return false;

        if (!GameClient._playerData.RemoveCollection(cardID, count))
            return false;

        GameClient._playerData.ADDDeck(cardID, count);
        RectTransform destination = RequestDeckItem?.Invoke(cardID);
        if (destination != null)
            DeckCardTransferRequested?.Invoke(source.Transform, destination, source.Sprite);

        return true;
    }

    public bool RequestRemoveDeck(int cardID, int count)
    {
        var GameClient = GameClientManager.instance;
        if (GameClient == null)
        {
            Debug.Log("[Deck Build System] GameClient Not Find");
            return false;
        }

        if (GameClient._playerData == null || !GameClient._playerData.RemoveDeck(cardID, count))
            return false;

        GameClient._playerData.ADDCollection(cardID, count);
        return true;
    }

    #region Defualt
    static public DeckBuildSystem instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

  

    #endregion
}
