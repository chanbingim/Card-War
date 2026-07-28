using System.Collections.Generic;

public class PlayerData
{
    public  string                     Name { get; private set; }
    public  List<int>                  Skills;
    public  List<int>                  Decks;
    public  List<int>                  PlayerParty { get; protected set; }

    public  Dictionary<int, int>       Collections = new Dictionary<int, int>();
    private Dictionary<int, StageData> StageDatas = new Dictionary<int, StageData>();

    public PlayerData()
    {
        Skills = new List<int>(GAME_CONST.Const.MAX_SKILL);
        Decks = new List<int>(GAME_CONST.Const.MAX_DECK);

        EventBus.Subscribe<StageClearEvent>(ClearStage);
    }

    public void SetName(string name)
    {
        Name = name;
    }

    void ClearStage(StageClearEvent data)
    {
        if(StageDatas.TryGetValue(data.StageID, out var stage))
            stage.SetData(data);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StageClearEvent>(ClearStage);
    }
}
