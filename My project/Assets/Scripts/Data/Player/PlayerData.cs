using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerData
{
    public string Name { get; private set; }
    public IReadOnlyDictionary<int, StageData>  StageDatas => _stageDatas;
    public IReadOnlyDictionary<int, int>        Collections => _Collections;
    public IReadOnlyList<int>                   Skills => _Skills;
    public IReadOnlyList<int>                   Decks => _Decks;
    public IReadOnlyList<int>                   PlayerParty => _PlayerParty;

    private List<int>   _Skills;
    private List<int>   _Decks;
    private List<int>   _PlayerParty;

    private Dictionary<int, int>        _Collections = new Dictionary<int, int>();
    private Dictionary<int, StageData>  _stageDatas = new Dictionary<int, StageData>();

    public PlayerData()
    {
        _Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);
        _Decks = new List<int>(GAME_CONST.Const.MAX_DECK);

        EventBus.Subscribe<StageClearEvent>(ClearStage);
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

        _Decks.AddRange(
            Enumerable.Repeat(cardID, ADDCount)
        );

        return true;
    }

    public bool RemoveCardFromDeck(int cardID, int count)
    {
        if (count <= 0)
            return false;

        int removeCount = 0;
        for (int i = _Decks.Count - 1; i >= 0; i--)
        {
            if (_Decks[i] != cardID)
                continue;

            _Decks.RemoveAt(i);
            removeCount++;

            if (removeCount >= count)
                break;
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
            stage.SetData(data);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StageClearEvent>(ClearStage);
    }
}
