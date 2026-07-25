using UnityEngine;

public readonly struct StageClearEvent
{
    public readonly int     StageID;
    public readonly int     StarCount;

    public StageClearEvent(int id, int starCount)
    {
        StageID = id;
        StarCount = starCount;
    }
}
