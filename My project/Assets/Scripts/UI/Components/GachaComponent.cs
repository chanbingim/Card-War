using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GachaComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] UnityEvent OnClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        var Gachasystem = GachaSystem.instance;
        if(Gachasystem == null)
        {
            Debug.Log("[GachaComponent] Not Find Gacha System");
            return;
        }

        // 여기서 연출 호출
        OnClicked?.Invoke();
        //Gachasystem.RequestGachaResult();
    }
}
