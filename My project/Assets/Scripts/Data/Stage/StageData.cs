
public class StageData
{
    public int      StageID { get; private set; }
    public int      StarCount { get; private set; }

    public bool     IsClear { get; private set; }
    public bool     RewardReceived { get; private set; }

    public StageData(int stageID, 
                     int starCount = 0,
                     bool isClear = false, 
                     bool rewardReceived = false)
    {
        this.StageID = stageID;
        this.StarCount = starCount;
        this.IsClear = isClear;
        this.RewardReceived = rewardReceived;
    }

    public void SetData(StageClearEvent data)
    {
        if (data.StarCount <= 0)
            return;

        StarCount = data.StarCount;
        if (IsClear == false)
            IsClear = true;

       if (RewardReceived == false)
           RewardReceived = true;
    }
}