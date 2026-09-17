using System;
using System.Collections.Generic;
using UnityEngine;
using static PlayerData;
using static UnityEngine.Rendering.DebugUI;

public class GameClientManager : MonoBehaviour
{
    public PlayerData _playerData;

    public BattlePlayerData                     GetBattleData()         { return new BattlePlayerData(_playerData, true); }
    public IReadOnlyDictionary<int, StageData>  GetPlayerStages()       { return _playerData?.StageDatas; }
    public IReadOnlyDictionary<int, int>        GetCollection()         { return _playerData?.Collections; }
    public IReadOnlyList<int>                   GetPlayerSkill()        { return _playerData?.Skills; }
    public IReadOnlyList<int>                   GetPlayerPartyList()    { return _playerData?.PlayerParty; }
    public IReadOnlyList<DeckEntry>             GetPlayerDeck()         { return _playerData?.Decks; }

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
       _playerData.ADDCollection(1, 3);
       _playerData.ADDCollection(2, 4);
       _playerData.ADDCollection(3, 3);
       _playerData.ADDCollection(4, 2);

        _playerData.ReName("Client A");
    }

    #endregion
}
