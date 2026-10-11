using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PlayerData;

internal class DeckViewEntry : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image  CardImage = null;
    [SerializeField] private Text   Nametext = null;
    [SerializeField] private Text   CountText = null;

    private DataManager dataManager = null;
    private int ID;
    private int Count;

    private void Awake()
    {
        dataManager = DataManager.instance;
    }

    public void Refresh(DeckEntry entry)
    {
        var CardData = dataManager.GetCardById(entry.CardID);
        if (CardData == null)
            return;

        ID = entry.CardID;
        Count = entry.Count;

        CardImage.sprite = CardData.Icon;
        Nametext.text = CardData.Name;
        CountText.text = entry.Count.ToString();
    }

    public void ClieckedEvnet()
    {
        // 여기서 데이터를 몇개 뺄지 결정하고 추가

        var ClientMgr = GameClientManager.instance;
        if (ClientMgr == null)
            return;

        (int, int) Entry = (ID, Count);
        if (ClientMgr._playerData.RemoveDeck(Entry.Item1, Entry.Item2))
            ClientMgr._playerData.ADDCollection(Entry.Item1, Entry.Item2);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClieckedEvnet();
    }
}
