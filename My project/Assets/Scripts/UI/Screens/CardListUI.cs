using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnCardGame.Data;
using Unity.VisualScripting;
using UnityEngine;

public class CardListUI : MonoBehaviour
{
    [SerializeField] private GameObject         _cardPrefab;
    [SerializeField] private Transform          _CardDeck;
    [SerializeField] private List<CardUI>       _cardList;

    [SerializeField] private float              _spreadAngle = 105f;
    [SerializeField] private int                _maxDrawCount = 5;

    private float   _CardPadding;

    private Queue<int>  _drawRequestQueue = new Queue<int>();
    private Coroutine   _drawRoutine; 

    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        _CardPadding = rectTransform.rect.width / _maxDrawCount;

        PlayerDataManager.instance.UseCardEvent += Remove_Card;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RequestDraw(2);
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

    private void Remove_Card(CardUI card)
    {
        _cardList.Remove(card);
        Destroy(card.gameObject);

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
                var data = PlayerDataManager.instance.Draw_Card();
                if(data != null) 
                    ADD_Card(data);

                yield return new WaitForSeconds(0.5f); // 카드 한 장씩 연출 딜레이
            }
        }

        _drawRoutine = null; // 큐 다 비면 워커 종료
    }

    void OnDisable()
    {
        if (PlayerDataManager.instance != null)
            PlayerDataManager.instance.UseCardEvent -= Remove_Card; // 반드시 해줘야 참조 끊김
    }
}
