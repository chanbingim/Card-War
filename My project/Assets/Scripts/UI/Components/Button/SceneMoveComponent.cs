using UnityEngine;
using UnityEngine.EventSystems;

public class SceneMoveComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string         _NextScene;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.Change_Scene(_NextScene);
    }
}
