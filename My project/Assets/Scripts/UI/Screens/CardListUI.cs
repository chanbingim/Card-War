using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CardListUI : MonoBehaviour
{
    [SerializeField] private GameObject         _cardPrefab;
    [SerializeField] private Transform          _CardDeck;
    [SerializeField] private List<CardUI>       _cardList;

    [SerializeField] private float              _spreadAngle = 105f;
    [SerializeField] private int                _maxDrawCount = 5;

    private float   _CardPadding;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        _CardPadding = rectTransform.rect.width / _maxDrawCount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ADD_Card();
    }

    [ContextMenu("Spawn Card")]
    public void ADD_Card()
    {
        if (_cardList.Count >= 5)
            return;

        var obj = GameObject.Instantiate(_cardPrefab, gameObject.transform);
        obj.transform.position = _CardDeck.position;
        _cardList.Add(obj.GetComponent<CardUI>());
        RefreshCardTransform();
    }

    public int Remove_Card(CardUI card)
    {
        _cardList.Remove(card);
        RefreshCardTransform();
        return card._CardID;
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
}
