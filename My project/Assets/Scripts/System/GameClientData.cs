using GamePlay.Enum;
using System;
using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;
using static PlayerData;

public class GameClientManager : MonoBehaviour
{
    public PlayerData _playerData;

    public event Func<int, (RectTransform Transform, Sprite Sprite)> RequestCollectionItem;
    public event Func<int, RectTransform> RequestDeckItem;

    public event Action<RectTransform, RectTransform, Sprite> DeckCardTransferRequested;

    public BattlePlayerData                     GetBattleData()         { return new BattlePlayerData(_playerData, true); }
    public IReadOnlyDictionary<int, StageData>  GetPlayerStages()       { return _playerData?.StageDatas; }
    public IReadOnlyDictionary<int, int>        GetCollection()         { return _playerData?.Collections; }
    public IReadOnlyList<int>                   GetPlayerSkill()        { return _playerData?.Skills; }
    public IReadOnlyList<int>                   GetPlayerPartyList()    { return _playerData?.PlayerParty; }
    public IReadOnlyList<DeckEntry>             GetPlayerDeck()         { return _playerData?.Decks; }
    public IReadOnlyList<DeckEntry>             GetCollection(ECardEffectType type) { return _playerData?.FindCollectionList(type); }

    public void SubscribeCurrencyEvent(Action<ECurrency, int> action)
    {
        _playerData.OnChangeCurrencyValue += action;
    }

    public void UnSubscribeCurrencyEvent(Action<ECurrency, int> action)
    {
        _playerData.OnChangeCurrencyValue -= action;
    }

    public int GetCurrency(ECurrency Type) { return _playerData?.GetCurrency(Type) ?? 0; }

    // 재화습득에 성공하면 해당하는 재화의 값을 알려준다.
    public int ADDCurrency(ECurrency Type, int Value, int ErrorCode) { return _playerData?.ADDCurrency(Type, Value, ErrorCode) ?? 0; }

    public bool HasEnoughCurrency(ECurrency Type, int Value) { return _playerData?.HasEnoughCurrency(Type, Value) ?? false;  }

    public bool RequestAddDeck(int cardID, int count)
    {
        if (_playerData == null || RequestCollectionItem == null)
            return false;

        var source = RequestCollectionItem.Invoke(cardID);
        if (source.Transform == null || source.Sprite == null)
            return false;

        if (!_playerData.RemoveCollection(cardID, count))
            return false;

        _playerData.ADDDeck(cardID, count);

        RectTransform destination = RequestDeckItem?.Invoke(cardID);
        if (destination != null)
            DeckCardTransferRequested?.Invoke(source.Transform, destination, source.Sprite);

        return true;
    }

    public bool RequestRemoveDeck(int cardID, int count)
    {
        if (_playerData == null || !_playerData.RemoveDeck(cardID, count))
            return false;

        _playerData.ADDCollection(cardID, count);
        return true;
    }

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
       _playerData = new PlayerData();
       _playerData.ADDCollection(1001, 3);
       _playerData.ADDCollection(1002, 4);
       _playerData.ADDCollection(1003, 3);
       _playerData.ADDCollection(1004, 2);

        _playerData.ReName("Client A");
    }

    #endregion
}
