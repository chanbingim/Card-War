
public class TurnEndButton: UIBase
{
    void Awake()
    {
        EventBus.Subscribe<TurnStartEvent>(View_TurnUI);
        gameObject.SetActive(false);
    }

    private void View_TurnUI(TurnStartEvent turnStartEvent)
    {
        gameObject.SetActive(true);

        var Player = BattleManager.instance.GetLoaclPlayer();
        if (0 == (Player.Name.CompareTo(turnStartEvent.Name)))
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<TurnStartEvent>(View_TurnUI);
        base.OnDestroy();
    }
}
