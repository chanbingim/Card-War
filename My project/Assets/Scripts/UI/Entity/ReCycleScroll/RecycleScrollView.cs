using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RecycleScrollView<T> : UIBase
{
    [Header("UI 연결")]
    public ScrollRect           _ScrollRect;        //ScrollView에 있는 ScrollRect
    public RectTransform        _Content;           // view아래의 Content. 
    public GameObject           _Reuseable_Item;    // 아이템 prefab

    [Header("세팅 값")]
    public int _itemHeight = 100;                   //아이템 높이
    public int _spacing = 20;                       //아이템 간 간격.
    public int _visibleCount = 10;                  // 재사용 스크롤뷰에 쓸 프리팹 수.
    public Vector2 _Offset = Vector2.zero;

    protected int _prevStartIndex = -1; //스크롤 시 이전 Index에서 변화가 있는지 체크용.
    protected List<GameObject> _pooledItems = new List<GameObject>(); //프리팹 풀링용도
    protected Queue<T> _datas = new();

    public void ChangeValue(Vector2 Value)
    {
        RefreshView();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected void ComputeRectSize()
    {
        //Content 총 높이 계산.
        float contentHeight = _datas.Count * (_itemHeight + _spacing);
        _Content.sizeDelta = new Vector2(_Content.sizeDelta.x, contentHeight);
    }

    protected virtual void Init()
    {
        for (int i = 0; i < _visibleCount; i++)
        {
            var item = GameObject.Instantiate(_Reuseable_Item, _Content.transform);
            var rt = item.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.rect.width, _itemHeight);

            item.SetActive(false);
            _pooledItems.Add(item);
        }

        ComputeRectSize();
        _ScrollRect.onValueChanged.AddListener(ChangeValue);
        RefreshView();
    }

    protected void RefreshView()
    {
        float scrollY = _Content.anchoredPosition.y;
        int startIndex = Mathf.FloorToInt(scrollY / (_itemHeight + _spacing));

        //스크롤 하는데 이전 인덱스랑 차이가 없으면 return;
        if (_prevStartIndex == startIndex)
            return;

        float Startsize = (_itemHeight + _spacing) * 0.5f;

        _prevStartIndex = startIndex;
        for (int i = 0; i < _pooledItems.Count; i++)
        {
            int dataIndex = startIndex + i;
            var go = _pooledItems[i];

            //dataIndex가 유효 범위에 있는지 확인
            if (dataIndex >= 0 && dataIndex < _datas.Count)
            {
                go.SetActive(true);
                var rt = go.GetComponent<RectTransform>();
                float anchoredY = -dataIndex * (_itemHeight + _spacing) - Startsize;
                rt.anchoredPosition = new Vector2(0, anchoredY) + _Offset;
            }
            else //데이터 범위 초과한 gameObject들은 active false.
            {
                go.SetActive(false);
            }
        }
    }
}
