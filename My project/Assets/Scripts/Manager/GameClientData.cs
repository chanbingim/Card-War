using System;
using System.Linq;
using UnityEngine;

public class GameClientManager : MonoBehaviour
{
    public PlayerData playerData { get; private set; }

    #region Collection
    public void RequestADDCollection(int CardID, int CardCount = 1)
    {
        if (CardCount <= 0)
        {
            Debug.Log("Collection Remove Fail From Player (Card Index Out Bound)");
            return;
        }

        if (playerData.Collections.TryGetValue(CardID, out int count))
        {
            playerData.Collections[CardID] = count + CardCount;
        }
        else
        {
            playerData.Collections.Add(CardID, CardCount);
        }
    }

    public bool RequestRemoveCollection(int CardID, int CardCount = 1)
    {
        if (CardCount <= 0)
        {
            Debug.Log("Collection Remove Fail From Player (Card Index Out Bound)");
            return false;
        }

        if (!playerData.Collections.TryGetValue(CardID, out int current))
        {
            Debug.Log("Collection Remove Fail From Player (Not Find ID)");
            return false;
        }

        if (current < CardCount)
            return false;

        if (current == CardCount)
            playerData.Collections.Remove(CardID);
        else
            playerData.Collections[CardID] = current - CardCount;

        return true;
    }
    #endregion

    #region ADD_Card
    public bool AddCardFromDeck(int cardID, int count)
    {
        if (playerData == null || count <= 0)
            return false;

        int ADDCount = Math.Min(GAME_CONST.Const.MAX_DECK - playerData.Decks.Count, count);
        if (ADDCount <= 0)
            return false;

        if (!playerData.Collections.TryGetValue(cardID, out int ownedCount))
            return false;

        if (ownedCount < count)
            return false;

        if (!RequestRemoveCollection(cardID, ADDCount))
            return false;

        playerData.Decks.AddRange(
            Enumerable.Repeat(cardID, ADDCount)
        );

        return true;
    }

    public bool RemoveCardFromDeck(int cardID, int count)
    {
        if (playerData == null || count <= 0)
            return false;

        int removeCount = 0;
        for (int i = playerData.Decks.Count - 1; i >= 0; i--)
        {
            if (playerData.Decks[i] != cardID)
                continue;

            playerData.Decks.RemoveAt(i);
            removeCount++;

            if (removeCount >= count)
                break;
        }

        if (removeCount <= 0)
            return false;

        RequestADDCollection(cardID, removeCount);
        return true;
    }

    #endregion

    #region Defualt
    static public GameClientManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        instance.Initialize();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void Initialize()
    {
        playerData = new PlayerData();
        RequestADDCollection(1, 3);
        RequestADDCollection(2, 4);
        RequestADDCollection(3, 3);
        RequestADDCollection(4, 2);
    }

    #endregion


}
