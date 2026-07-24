using UnityEngine;
using TurnCardGame.Data;

public class Character : MonoBehaviour, IPointerHoverEvent
{
    int                     ID = 0;
    CharacterRuntimeData    Data = null;

    private Material        _material = null;

    public delegate void     FinishedAction();
    public event FinishedAction     OnFinishedAct;

    void Start()
    {
        SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
        if(spriteRender != null )
        {
            _material = spriteRender.material;
        }
    }

    public void Initialize(int CharacterID)
    {
        ID = CharacterID;
        // 데이터 찾기
        CharacterData CharacterSO = new CharacterData();
        Data = new CharacterRuntimeData(CharacterSO);
    }

    public void OnDrop(UIBase DragUI)
    {
        Debug.Log("Use Card");

        var CardUI = DragUI as CardUI;
        if( CardUI != null )
        {
            EventBus.Publish<UseCardEvent>(new UseCardEvent(this, CardUI));
        }

        OnHoverExit();
    }

    public void Update_Action(CardAction action)
    {
        // 상대 받아서 현재 타입에 맞는 행동을 진행한다.
        Debug.Log($"Action {action.ActType}");
        Action_End();
    }

    public void Action_End()
    {
        OnFinishedAct.Invoke();
    }

    public void OnHoverEnter()
    {
        _material.SetFloat("_Enable", 1);
    }

    public void OnHoverExit()
    {
        _material.SetFloat("_Enable", 0);
    }
}
