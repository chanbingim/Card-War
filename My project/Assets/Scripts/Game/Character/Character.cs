using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour, IPointerHoverEvent
{
    private Material        _material = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
        if(spriteRender != null )
        {
            _material = spriteRender.material;
        }
    }

    // Update is called once per frame
    void Update()
    {

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
