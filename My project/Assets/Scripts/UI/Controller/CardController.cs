using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnCardGame.Data;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private GameObject         _cardPrefab;
    [SerializeField] private Transform          _CardDeck;
    [SerializeField] private List<CardUI>       _cardList;

    [SerializeField] private float              _spreadAngle = 105f;
    [SerializeField] private int                _maxDrawCount = 5;
    [SerializeField] private int                _DrawCardCount = 2;

    private float       _CardPadding;
    private Queue<int>  _drawRequestQueue = new Queue<int>();
    private Coroutine   _drawRoutine;

    private void Awake()
    {
        EventBus.Subscribe<TurnStartEvent>(TurnStartEvent);
    }

    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        _CardPadding = rectTransform.rect.width / _maxDrawCount;

        EventBus.Subscribe<UseCardEvent>(Remove_Card);
    }

    public void RequestDraw(int count)
    {
        _drawRequestQueue.Enqueue(count);

        if (_drawRoutine == null)
            _drawRoutine = StartCoroutine(DrawWorker());
    }

    private void ADD_Card(UI_CardData data)
    {
        if (_cardList.Count >= 5)
            return;

        var obj = GameObject.Instantiate(_cardPrefab, gameObject.transform);
        obj.transform.position = _CardDeck.position;

        _cardList.Add(obj.GetComponent<CardUI>());
        _cardList.Last().SettingData(data);
        RefreshCardTransform();
    }

    private void TurnStartEvent(TurnStartEvent turnStartEvent)
    {
        var Player = BattleManager.instance.GetLoaclPlayer();
        if (0 == (Player.Name.CompareTo(turnStartEvent.Name)))
            RequestDraw(_DrawCardCount);
    }

    private void Remove_Card(UseCardEvent card)
    {
        _cardList.Remove(card.UseCard);

        Destroy(card.UseCard);
        RefreshCardTransform();
    }

    private void RefreshCardTransform()
    {
        float center = (_cardList.Count - 1) * 0.5f;

        for (int i = 0; i < _cardList.Count; i++)
        {
            float numIdx = i - center;

            float x = numIdx * _CardPadding;
            float angle = numIdx * _spreadAngle;

            _cardList[i].DrawAnimation(transform.position + new Vector3(x, 0, 0));
        }
    }

    IEnumerator DrawWorker()
    {
        while (_drawRequestQueue.Count > 0)
        {
            int count = _drawRequestQueue.Dequeue();
            for (int i = 0; i < count; i++)
            {
                var data = BattleManager.instance.DrawCard();
                if(data != null) 
                    ADD_Card(data);

                yield return new WaitForSeconds(0.5f); // 카드 한 장씩 연출 딜레이
            }
        }

        _drawRoutine = null; // 큐 다 비면 워커 종료
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<TurnStartEvent>(TurnStartEvent);
        EventBus.Unsubscribe<UseCardEvent>(Remove_Card);
    }
}
