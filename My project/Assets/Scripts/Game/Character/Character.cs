using UnityEngine;

public class Character : MonoBehaviour, IPointerHoverEvent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrop(UIBase DragUI)
    {
        Debug.Log("Use Card");
    }

    public void OnHoverEnter()
    {

    }

    public void OnHoverExit()
    {

    }

   
}
