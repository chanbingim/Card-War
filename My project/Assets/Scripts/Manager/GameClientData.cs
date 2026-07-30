using UnityEngine;
using System.Collections.Generic;

public class GameClientManager : MonoBehaviour
{
    private PlayerData _playerData;

    public BattlePlayerData                     GetBattleData()         { return new BattlePlayerData(_playerData); }
    public IReadOnlyDictionary<int, StageData>  GetPlayerStages()       { return _playerData?.StageDatas; }
    public IReadOnlyDictionary<int, int>        GetCollection()         { return _playerData?.Collections; }
    public IReadOnlyList<int>                   GetPlayerSkill()        { return _playerData?.Skills; }
    public IReadOnlyList<int>                   GetPlayerPartyList()    { return _playerData?.PlayerParty; }
    public IReadOnlyList<int>                   GetPlayerDeck()         { return _playerData?.Decks; }

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
    }

    #endregion
}
