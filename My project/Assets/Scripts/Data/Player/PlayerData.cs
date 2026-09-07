using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct DeckEntry
{
    public int CardID;
    public int Count;
}

public class PlayerData
{
    public string Name { get; private set; }
    public IReadOnlyDictionary<int, StageData>  StageDatas => _stageDatas;
    public IReadOnlyDictionary<int, int>        Collections => _Collections;
    public IReadOnlyList<int>                   Skills => _Skills;
    public IReadOnlyList<DeckEntry>             Decks => _Decks;
    public IReadOnlyList<int>                   PlayerParty => _PlayerParty;

    private List<int>           _Skills;
    private List<DeckEntry>     _Decks;
    private List<int>           _PlayerParty;

    // 플레이어가 습득한 카드의 종류 및 개수
    private Dictionary<int, int>        _Collections = new Dictionary<int, int>();

    // 스테이지 클리어 정보
    private Dictionary<int, StageData>  _stageDatas = new Dictionary<int, StageData>();

    public PlayerData()
    {
        _Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);
        _Decks = new List<DeckEntry>(GAME_CONST.Const.MAX_DECK);

        EventBus.Subscribe<StageClearEvent>(ClearStage);

        var stageData = new StageData(1, 3);

        _stageDatas.Add(1, stageData);
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

        return true;
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

    void OnDisable()
    {
        EventBus.Unsubscribe<StageClearEvent>(ClearStage);
    }
}
