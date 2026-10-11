using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PlayerData;

internal class CollectionCardView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image  TypeIconImage = null;
    [SerializeField] private Image  CardImage = null;
    [SerializeField] private Text   CountText = null;

    private DataManager dataManager = null;
    private int     ID;
    private int     Count;

    private void Awake()
    {
        dataManager = DataManager.instance;
    }

    public void Refresh(DeckEntry Value)
    {
        var cardData = dataManager.GetCardById(Value.CardID);
        ID = Value.CardID;
        Count = Value.Count;

        if (CardImage != null)
            CardImage.sprite = cardData.Icon;

        if (CountText != null)
            CountText.text = Value.Count.ToString();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        var ClientMgr = GameClientManager.instance;
        if (ClientMgr == null)
            return;

        (int, int) Entry = (ID, Count);
        if(ClientMgr._playerData.RemoveCollection(Entry.Item1, Entry.Item2))
            ClientMgr._playerData.ADDDeck(Entry.Item1, Entry.Item2);
    }
}
