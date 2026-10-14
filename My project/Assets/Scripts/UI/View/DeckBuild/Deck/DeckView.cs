using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

internal class DeckView : MonoBehaviour
{
    [SerializeField] List<DeckViewEntry> ViewList = new();
    [SerializeField] Text                DeckCountText = null;

    private GameClientManager            ClientManager = null;

    private void Awake()
    {
        ClientManager = GameClientManager.instance;
        ClientManager._playerData.OnChangeDeck += RefreshItem;
        ClientManager.RequestDeckItem += GetDeckItem;
    }

    private void Start()
    {
        RefreshItem(ClientManager.GetPlayerDeck());
    }

    private void OnDestroy()
    {
        ClientManager._playerData.OnChangeDeck -= RefreshItem;
        ClientManager.RequestDeckItem -= GetDeckItem;
    }

    public void RefreshItem(IReadOnlyList<DeckEntry> Decks)
    {
        int total = 0;

        for (int i = 0; i < ViewList.Count; i++)
        {
            if(i >= Decks.Count)
            {
                ViewList[i].gameObject.SetActive(false);
            }
            else
            {
                ViewList[i].gameObject.SetActive(true);
                ViewList[i].Refresh(Decks[i]);

                total += Decks[i].Count;
            }
        }

        DeckCountText.text = total.ToString();
    }

    private RectTransform GetDeckItem(int cardID)
    {
        foreach (DeckViewEntry entry in ViewList)
        {
            if (entry.gameObject.activeInHierarchy && entry.CardID == cardID)
                return entry.RectTransform;
        }

        return null;
    }
}
