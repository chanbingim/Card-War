
public class TurnEndButton: UIBase
{
    private DissolveComponent _Dissolve = null;

    void Awake()
    {
        _Dissolve = GetComponent<DissolveComponent>();
        EventBus.Subscribe<ChangeTurnEvent>(View_TurnUI);

        gameObject.SetActive(false);
    }

    private void View_TurnUI(ChangeTurnEvent turnStartEvent)
    {
        if (turnStartEvent._IsLocal)
        {
            _Dissolve.OnDissloveAnim(true);
        }
        else
        {
            _Dissolve.OnDissloveAnim(false);
        }
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<ChangeTurnEvent>(View_TurnUI);
        base.OnDestroy();
    }
}
