using UnityEngine;
using GamePlay.Enum;
using System.Collections.Generic;

public class SettingMenu : UIBase
{
    [SerializeField] private List<GameObject> _menus;

    private int             _CurActiveMenuID = -1;

    void Start()
    {
        gameObject.SetActive(false);
        if(_menus.Count > 0)
        {
            Debug.Log("LOG : Not Bind Setting Menu List");
            for (int i = 0; i < _menus.Count; i++)
                _menus[i].SetActive(false);

            Change_SettingMenu(0);
        }
    }

    public void Change_SettingMenu(int MenuID)
    {
        if(_CurActiveMenuID != MenuID)
        {
            if(_CurActiveMenuID >= 0)
                _menus[_CurActiveMenuID].SetActive(false);

            _CurActiveMenuID = MenuID;
            _menus[_CurActiveMenuID].SetActive(true);
        }
    }

    void Change_Sound(SOUND_TYPE Type, float value)
    {

    }
}
