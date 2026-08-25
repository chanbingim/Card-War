using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneMoveComponent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool           _PointerEventEnable = true;

    [SerializeField] private string         _NextScene;
    [SerializeField] private int            _StageIndex = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(_PointerEventEnable)
            ChangeScene();
    }

    public void ChangeScene()
    {
        GameManager.instance.ChangeScene(_NextScene, _StageIndex);
    }
}
