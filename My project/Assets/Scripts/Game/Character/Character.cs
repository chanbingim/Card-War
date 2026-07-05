using UnityEngine;
using TurnCardGame.Data;

public class Character : MonoBehaviour, IPointerHoverEvent
{
    int                     ID = 0;
    CharacterRuntimeData    Data = null;

    private Material        _material = null;
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
            PlayerDataManager.instance.Use_Card(CardUI);

        OnHoverExit();
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
