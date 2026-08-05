using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

public class TurnHistoryUI : RecycleScrollView<CardAction>
{
    [Header("애니메이션 위치")]
    public Transform    _AnimTransform;

    private void OnCardActionAdd(ActionRecordedEvent data)
    {
        ComputeRectSize();
        if (_prevStartIndex == 0)
        {
            PlayAnim();
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

            RefreshView();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void PlayAnim()
    {
        var LastItem = _pooledItems.Last();
        _pooledItems.Remove(LastItem);

        LastItem.transform.position = _AnimTransform.transform.position;
        _pooledItems.Insert(0, LastItem);

        float Startsize = (_itemHeight + _spacing) * 0.5f;
        for (int i = 0; i < _pooledItems.Count; i++)
        {
            int dataIndex = _prevStartIndex + i;
            var go = _pooledItems[i];

            //dataIndex가 유효 범위에 있는지 확인
            if (dataIndex >= 0 && dataIndex < _datas.Count)
            {
                go.SetActive(true);
                var rt = go.GetComponent<RectTransform>();
                float anchoredY = -dataIndex * (_itemHeight + _spacing) - Startsize;
                rt.DOAnchorPos(new Vector2(0, anchoredY) + _Offset, 0.6f);
            }
            else //데이터 범위 초과한 gameObject들은 active false.
            {
                go.SetActive(false);
            }
        }
    }

    void Start()
    {
        SettingHistoryData();
        Init();

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
