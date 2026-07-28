using UnityEngine;
using UnityEngine.EventSystems;

public class SceneMoveComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string         _NextScene;
    [SerializeField] private int            _StageIndex = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.ChangeScene(_NextScene, _StageIndex);
    }
}
