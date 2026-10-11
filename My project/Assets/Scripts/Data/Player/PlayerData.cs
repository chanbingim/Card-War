using GamePlay.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using TurnCardGame.Data;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;

[System.Serializable]
public enum ECurrency
{
    Gold,          // 기본 골드
    Cash,          // 유료 재화(캐시, 보석 등)
    Energy,        // 행동력 / 스태미나
    Ticket,        // 뽑기권
    Key,           // 던전 입장 키
    Token,         // 이벤트 토큰
    END,
}

public class PlayerData
{
    #region struct 
    [Serializable]
    public struct DeckEntry 
    {
        public int CardID;
        public int Count;

        public DeckEntry(int id, int count)
        {
            CardID = id;
            Count = count;
        }

        public static bool operator ==(DeckEntry left, DeckEntry right)
        {
            return left.CardID == right.CardID;
        }

        public static bool operator !=(DeckEntry left, DeckEntry right)
        {
            return left.CardID != right.CardID;
        }

        public bool Equals(DeckEntry other)
        {
            return CardID == other.CardID;
        }

        public override bool Equals(object obj)
        {
            return obj is DeckEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return CardID.GetHashCode();
        }

    }
    #endregion

    public event Action<ECurrency, int>             OnChangeCurrencyValue;
    public event Action<IReadOnlyList<DeckEntry>>   OnChangeDeck;
    public event Action                             OnChangeCollections;

    public string Name { get; private set; }
    public IReadOnlyDictionary<int, StageData>  StageDatas => _stageDatas;
    public IReadOnlyDictionary<int, int>        Collections => _Collections;
    public IReadOnlyList<int>                   Skills => _Skills;
    public IReadOnlyList<DeckEntry>             Decks => _Decks;
    public IReadOnlyList<int>                   PlayerParty => _PlayerParty;

    private int[]               _Currency = null;
    private List<int>           _Skills = null;
    private List<DeckEntry>     _Decks = null;
    private List<int>           _PlayerParty = null;

    // 플레이어가 습득한 카드의 종류 및 개수
    private Dictionary<int, int>        _Collections = new Dictionary<int, int>();

    // 스테이지 클리어 정보
    private Dictionary<int, StageData>  _stageDatas = new Dictionary<int, StageData>();

    public PlayerData()
    {
        _Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);
        _Decks = new List<DeckEntry>(GAME_CONST.Const.MAX_DECK);

        _Currency = new int[(int)ECurrency.END];
        Array.Fill(_Currency, 0);

        EventBus.Subscribe<StageClearEvent>(ClearStage);
        var stageData = new StageData(1, 3, true);
    }

    public void ReName(string name)
    {
        Name = name;
    }

    #region Collection
    public void ADDCollection(int CardID, int CardCount = 1)
    {
        if (CardCount <= 0)
        {
            Console.WriteLine("Collection Remove Fail From Player (Card Index Out Bound)");
            return;
        }

        if (_Collections.TryGetValue(CardID, out int count))
        {
            _Collections[CardID] = count + CardCount;
        }
        else
        {
            _Collections.Add(CardID, CardCount);
        }

        OnChangeCollections?.Invoke();
    }

    public bool RemoveCollection(int CardID, int CardCount = 1)
    {
        if (CardCount <= 0)
        {
            Console.WriteLine("Collection Remove Fail From Player (Card Index Out Bound)");
            return false;
        }

        if (!_Collections.TryGetValue(CardID, out int current))
        {
            Console.WriteLine("Collection Remove Fail From Player (Not Find ID)");
            return false;
        }

        if (current < CardCount)
            return false;

        if (current == CardCount)
            _Collections.Remove(CardID);
        else
            _Collections[CardID] = current - CardCount;

        OnChangeCollections?.Invoke();
        return true;
    }

    public IReadOnlyList<DeckEntry> FindCollectionList(ECardEffectType type)
    {
        List<DeckEntry> newList = new List<DeckEntry>();

        var DataMgr = DataManager.instance;
        if (DataMgr == null)
            return null;

        if(type == ECardEffectType.None)
        {
            newList = Collections.Select(item => new DeckEntry(item.Key, item.Value))
                                 .ToList();
        }
        else
        {
            foreach (var iter in _Collections)
            {
                var data = DataMgr.GetCardById(iter.Key);

                if (data.eEffectType == type)
                    newList.Add(new DeckEntry(iter.Key, iter.Value));
            }
        }
        
        return newList;
    }

    #endregion

    #region Deck
    public void ADDDeck(int CardID, int CardCount = 1)
    {
        if (CardCount <= 0)
        {
            Debug.Log($"[Player Data] ADD Fail (CardCount Out Bound {CardCount})");
            return;
        }

        var Data = _Decks.FirstOrDefault(item => item.CardID == CardID);
        if (Data == default)
        {
            _Decks.Add(new DeckEntry(CardID, CardCount));
        }
        else
        {
            Data.Count += CardCount;
        }

        OnChangeDeck?.Invoke(_Decks);
    }

    public bool RemoveDeck(int CardID, int CardCount = 1)
    {
        if (CardCount <= 0)
        {
            Debug.Log($"[Player Data]  Remove Count  (CardCount Out Bound {CardCount})");
            return false;
        }

        var Data = _Decks.FirstOrDefault(item => item.CardID == CardID);
        if (Data == default)
        {
            Debug.Log($"[Player Data]  Remove Data Not Find)");
            return false;
        }
        else
        {
            Data.Count -= CardCount;
            if(Data.Count < 0)
            {
                Debug.Log($"[Player Data] Remove Count (CardCount Out Bound {CardCount})");
                Data.Count += CardCount;
                return false;
            }

            if(Data.Count == 0)
            {
                _Decks.Remove(Data);
            }
        }

        OnChangeDeck?.Invoke(_Decks);
        return true;
    }

    #endregion

    #region Currency Func
    // 해당하는 재화의 보유량을 가져온다.
    public int GetCurrency(ECurrency Type) 
    {
        int idx = (int)Type;
        if(0 > idx || idx >= _Currency.Length)
            return -1;

        return _Currency[idx];
    }

    // 재화습득에 성공하면 해당하는 재화의 값을 알려준다.
    public int ADDCurrency(ECurrency Type, int Value, int ErrorCode)
    {
        if(ErrorCode != 0)
        {
            Debug.Log("[PlayerData] Check Error Code");
            return 0;
        }

        int idx = (int)Type;
        if (idx < 0 || _Currency.Length <= idx)
            return 0;

        _Currency[idx] += Value;
        if (_Currency[idx] < 0)
            _Currency[idx] = 0;

        OnChangeCurrencyValue?.Invoke(Type, _Currency[idx]);
        return _Currency[idx];
    }

    public bool HasEnoughCurrency(ECurrency Type, int Value)
    {
        int idx = (int)Type;
        if (idx < 0 || _Currency.Length <= idx)
            return false;

        return _Currency[idx] >= Value;
    }
    #endregion

    #region ADD_Card
    public bool AddCardFromDeck(int cardID, int count)
    {
        if (count <= 0)
            return false;

        int ADDCount = Math.Min(GAME_CONST.Const.MAX_DECK - _Decks.Count, count);
        if (ADDCount <= 0)
            return false;

        if (!_Collections.TryGetValue(cardID, out int ownedCount))
            return false;

        if (ownedCount < count)
            return false;

        if (!RemoveCollection(cardID, ADDCount))
            return false;

        _Decks.Add(new DeckEntry{ CardID = cardID, Count = ADDCount });

        return true;
    }

    public bool RemoveCardFromDeck(int cardID, int count)
    {
        if (count <= 0)
            return false;

        int removeCount = 0;
        for (int i = _Decks.Count - 1; i >= 0; i--)
        {
            if (_Decks[i].CardID != cardID)
                continue;

            if (_Decks[i].Count - count > 0)
            {
                var Entry = _Decks[i];
                Entry.Count -= count;
                _Decks[i] = Entry;
            }
            else
            {
                _Decks.RemoveAt(i);
            }
        }

        if (removeCount <= 0)
            return false;

        ADDCollection(cardID, removeCount);
        return true;
    }
    #endregion

    #region Stage Func
    void ClearStage(StageClearEvent data)
    {
        if (_stageDatas.TryGetValue(data.StageID, out var stage))
        {
            if(stage.StarCount < data.StarCount)
                stage.SetData(data);
        }
        else
        {
            var stageData = new StageData(data.StageID, data.StarCount);
            stageData.SetData(data);

            _stageDatas.Add(data.StageID, stageData);
        }
    }
    #endregion

    void OnDisable()
    {
        EventBus.Unsubscribe<StageClearEvent>(ClearStage);
    }
}
