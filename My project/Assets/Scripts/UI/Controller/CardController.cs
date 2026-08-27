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

    private float       _CardPadding;

    private void Awake()
    {
       
    }

    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        _CardPadding = rectTransform.rect.width / GAME_CONST.Const.MAX_HAND;

        EventBus.Subscribe<UseCardEvent>(Remove_Card);
        EventBus.Subscribe<CardDrawEvent>(ADD_Card);
    }

    private void ADD_Card(CardDrawEvent data)
    {
        if (_cardList.Count >= GAME_CONST.Const.MAX_HAND)
            return;

        var obj = GameObject.Instantiate(_cardPrefab, gameObject.transform);
        obj.transform.position = _CardDeck.position;

        _cardList.Add(obj.GetComponent<CardUI>());
        _cardList.Last().SettingData(data._CardData);
        RefreshCardTransform();
    }

    private void Remove_Card(UseCardEvent card)
    {
        _cardList.Remove(card.UseCard);

        card.UseCard.Close();
        Destroy(card.UseCard.gameObject);

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

    void OnDisable()
    {
        EventBus.Unsubscribe<CardDrawEvent>(ADD_Card);
        EventBus.Unsubscribe<UseCardEvent>(Remove_Card);
    }
}
