using System;
using UI.Enum;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneMoveComponent : MonoBehaviour, IPointerClickHandler
{
    public event Action                     OnClicked;

    [SerializeField] private bool           _PointerEventEnable = true;
    [SerializeField] private string         _NextScene;
    [SerializeField] private int            _StageIndex = 0;

    public async void OnPointerClick(PointerEventData eventData)
    {
        if(_PointerEventEnable)
        {
            OnClicked?.Invoke();
            await UIManager.instance.ShowAsync(UIID.Fade, new FadeUIDesc(false, (System.Action)ChangeScene));
        }
    }

    public void ChangeScene()
    {
        GameManager.instance.ChangeScene(_NextScene, _StageIndex);
    }
}
