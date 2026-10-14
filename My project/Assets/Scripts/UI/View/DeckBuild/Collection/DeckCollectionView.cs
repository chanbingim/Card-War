using GamePlay.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using TurnCardGame.Data;
using UnityEngine;

internal class DeckCollectionView : MonoBehaviour
{
    [SerializeField] List<CollectionCardView>   CardViewList = new();
    private GameClientManager                   ClientManager = null;

    private ECardEffectType SelectIndex = ECardEffectType.None;

    // Page 관련 변수
    private int MaxPage = 5;
    private int viewPage = 0;

    private void  Awake()
    {
        ClientManager = GameClientManager.instance;

        ClientManager._playerData.OnChangeCollections += RefreshList;
        ClientManager.RequestCollectionItem += GetCollectionItem;
      
    }

    private void Start()
    {
        RefreshList();
    }

    private void OnDestroy()
    {
        ClientManager._playerData.OnChangeCollections -= RefreshList;
        ClientManager.RequestCollectionItem -= GetCollectionItem;
    }

    public void RefreshItem(ECardEffectType Index)
    {
        SelectIndex = Index;
       
        RefreshList();
    }

    private void RefreshList()
    {
        var Collection = ClientManager.GetCollection(SelectIndex);
        MaxPage = Collection.Count / CardViewList.Count;

        int StartIdx = viewPage * (CardViewList.Count / CardViewList.Count);
        for (int i = 0; i < CardViewList.Count; i++)
        {
            int Idx = StartIdx + i;

            if(Idx >= Collection.Count)
            {
                CardViewList[i].gameObject.SetActive(false);
            }
            else
            {
                CardViewList[i].gameObject.SetActive(true);
                CardViewList[i].Refresh(Collection.ElementAt(Idx));
            }
        }
    }

    public void PrevPage(int value)
    {
        viewPage = Math.Clamp(viewPage - value, 0, MaxPage);
    }

    private (RectTransform Transform, Sprite Sprite) GetCollectionItem(int cardID)
    {
        foreach (CollectionCardView cardView in CardViewList)
        {
            if (cardView.gameObject.activeInHierarchy && cardView.CardID == cardID)
                return (cardView.RectTransform, cardView.CardSprite);
        }

        return default;
    }
}
