using TurnCardGame.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static PlayerData;

public class DeckTypeEventInvoker : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ECardEffectType value;
    [SerializeField] private UnityEvent<ECardEffectType> onInvoke;

    public void OnPointerClick(PointerEventData eventData)
    {
        onInvoke?.Invoke(value);
    }
}
