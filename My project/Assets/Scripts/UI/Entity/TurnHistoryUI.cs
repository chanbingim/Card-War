using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnHistoryUI : UIBase
{
    [SerializeField] private Image  _BackGround = null;
    [SerializeField] private int    _ViewCount = 5;

    private List<GameObject>        _HistoryView;
    private int                     _Percent = 5;

    void Awake()
    {




    }

    public void SettingHistoryData(int ViewPlayerIndex)
    {
        try
        {
            var BattleMgr = BattleManager.instance;
            if (BattleMgr == null)
                throw new ArgumentException("[TrunHistoryUI] Battle Manager NULL");

            var act = BattleMgr.GetPlayerHistoryAction(ViewPlayerIndex);
            if(act == null)
                throw new ArgumentException("[TrunHistoryUI] Not Find Action");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void OnChangeScroll()
    {

    }

    private void OnEnable()
    {

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
