using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnHistoryUI : UIBase
{
    [SerializeField] private Image          _BackGround = null;
    [SerializeField] private List<UIBase>   _HistoryView;
    [SerializeField] private int            _ViewCount = 5;

    private Queue<CardAction>       _act = null;
  

    private int                     _maxPage = 0;
    private int                     _CurPage = 1;

    private int                     _Percent = 5;

    void Start()
    {
        _ViewCount = _HistoryView.Count;
     
    }

    public void SettingHistoryData(int ViewPlayerIndex)
    {
        try
        {
            var BattleMgr = BattleManager.instance;
            if (BattleMgr == null)
                throw new ArgumentException("[TrunHistoryUI] Battle Manager NULL");

            _act = BattleMgr.GetPlayerHistoryAction(ViewPlayerIndex);
            if(_act == null)
                throw new ArgumentException("[TrunHistoryUI] Not Find Action");

            _maxPage = (_act.Count / _ViewCount) + 1;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    public void ChangeValue(int Value)
    {
        _CurPage = Math.Clamp(_CurPage + Value, 1, _maxPage);

        for(int i = 0; i < _ViewCount; i++)
        {
            int iIndex = (_CurPage * _ViewCount) + i;

            if (_act.Count <= iIndex)
                _HistoryView[i].Close();
            else
            {
                _HistoryView[i].Open();
                //여기서 값 세팅
            }

        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
