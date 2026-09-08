
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnEndButton: UIBase, IPointerClickHandler
{
    private DissolveComponent _Dissolve = null;

    void Awake()
    {
        _Dissolve = GetComponent<DissolveComponent>();
        EventBus.Subscribe<ChangeTurnEvent>(View_TurnUI);

        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var BattleMgr = BattleManager.instance;
        if(BattleMgr == null)
        {
            Debug.Log("[Turn End Button] Not Find BattleManager");
            return;
        }

        var Player = BattleMgr.GetLoaclPlayer();
        BattleMgr.RequestEndTurn(Player.Name);
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
