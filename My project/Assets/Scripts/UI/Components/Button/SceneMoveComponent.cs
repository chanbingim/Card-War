using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneMoveComponent : MonoBehaviour, IPointerClickHandler
{
    public event Action                     OnClicked;

    [SerializeField] private bool           _PointerEventEnable = true;
    [SerializeField] private string         _NextScene;
    [SerializeField] private int            _StageIndex = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(_PointerEventEnable)
        {
            OnClicked?.Invoke();
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        GameManager.instance.ChangeScene(_NextScene, _StageIndex);
    }
}
