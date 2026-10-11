using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerData;

internal class DeckView : MonoBehaviour
{
    [SerializeField] List<DeckViewEntry> ViewList = new();
    [SerializeField] Text                DeckCountText = null;

    private GameClientManager            ClientManager = null;
    private int                          TotalDeckCount = 0;

    private void Awake()
    {
        ClientManager = GameClientManager.instance;
        ClientManager._playerData.OnChangeDeck += RefreshItem;
    }

    private void Start()
    {
        RefreshItem(ClientManager.GetPlayerDeck());
    }

    private void Destory()
    {
        ClientManager._playerData.OnChangeDeck -= RefreshItem;
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

        TotalDeckCount = total;
        DeckCountText.text = total.ToString();
    }
}