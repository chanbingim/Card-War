using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GachaComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEvent OnClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
       

        // 여기서 연출 호출
        OnClicked?.Invoke();
    }
}
