using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    Stack<GameObject>       popup_Stack;


    public void Add_UI()
    {
        Debug.Log("Show Setting Menu");
        //popup_Stack.Push();

    }

    static public UIManager instance { get; private set; }
    static UIManager _instance = null;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            instance = _instance;

            DontDestroyOnLoad(_instance);
        }
        else
            Destroy(this);
    }

}
