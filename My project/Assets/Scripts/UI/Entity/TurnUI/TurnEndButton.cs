
public class TurnEndButton: UIBase
{
    void Awake()
    {
        EventBus.Subscribe<ChangeTurnEvent>(View_TurnUI);
        gameObject.SetActive(false);
    }

    private void View_TurnUI(ChangeTurnEvent turnStartEvent)
    {
        if (turnStartEvent._IsLocal)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<ChangeTurnEvent>(View_TurnUI);
        base.OnDestroy();
    }
}
