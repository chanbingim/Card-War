public readonly struct TurnUIEvent
{
    public string Name { get; }
    public TurnManager.ETurnType eTurnType { get; }

    public TurnUIEvent(string name, TurnManager.ETurnType eturnType)
    {
        Name = name;
        eTurnType = eturnType;
    }
}
