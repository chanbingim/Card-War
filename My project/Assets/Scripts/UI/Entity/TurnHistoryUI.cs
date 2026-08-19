using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class TurnHistoryUI : RecycleScrollView<CharacterAction>
{
    [Header("애니메이션 위치")]
    public Transform    _AnimTransform = null;

    [Header("미리 캐싱")]
    private List<TurnActionSlot> _TurnActionList = null;

    private void OnCardActionAdd(ActionRecordedEvent data)
    {
        ComputeRectSize();
        if (_prevStartIndex == -1)
            _prevStartIndex = 0;

        if (_prevStartIndex == 0)
        {
            RefreshView();
        }
    }

    public void SettingHistoryData()
    {
        try
        {
            var BattleMgr = BattleManager.instance;
            if (BattleMgr == null)
                throw new ArgumentException("[TrunHistoryUI] Battle Manager NULL");

            _datas = BattleMgr.GetAllHistory();
            if (_datas == null)
                throw new ArgumentException("[TrunHistoryUI] Not Find Action");

            Init(_datas);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    protected override void RefreshView()
    {
        var LastItem = _TurnActionList.Last();
        _TurnActionList.Remove(LastItem);

        LastItem.transform.position = _AnimTransform.transform.position;
        _TurnActionList.Insert(0, LastItem);
        _TurnActionList[0].transform.SetAsLastSibling();

        var HistoryActions = BattleManager.instance.GetAllHistory();
        float Startsize = (_itemHeight + _spacing) * 0.5f;

        for (int i = 0; i < _pooledItems.Count; i++)
        {
            int dataIndex = _prevStartIndex + i;
            var go = _TurnActionList[i];

            //dataIndex가 유효 범위에 있는지 확인
            if (dataIndex >= 0 && dataIndex < _datas.Count)
            {
                go.gameObject.SetActive(true);

                float anchoredY = -dataIndex * (_itemHeight + _spacing) - Startsize;
                go.SetData(HistoryActions[(HistoryActions.Count - 1) - dataIndex], new Vector2(0, anchoredY) + _Offset, true);
            }
            else //데이터 범위 초과한 gameObject들은 active false.
            {
                go.gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        SettingHistoryData();

        _TurnActionList = new List<TurnActionSlot>();
        _TurnActionList.Capacity = _pooledItems.Count;
        foreach (var item in _pooledItems)
            _TurnActionList.Add(item.GetComponent<TurnActionSlot>());

        EventBus.Subscribe<ActionRecordedEvent>(OnCardActionAdd);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ActionRecordedEvent>(OnCardActionAdd);
    }
}
