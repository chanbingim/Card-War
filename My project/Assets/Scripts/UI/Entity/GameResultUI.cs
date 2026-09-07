using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private SceneMoveComponent _SceneMoveCom = null;
    [SerializeField] private Image _Image = null;

    private bool _IsClear = false;

    void Start()
    {
        if(_SceneMoveCom != null )
        {
            _SceneMoveCom.OnClicked += OnStageClearEvent;
        }

        gameObject.SetActive(false);
        EventBus.Subscribe<BattleEndEvent>(ResultUI);
    }

    private void ResultUI(BattleEndEvent Result)
    {
        if (_sprites.Count <= 1)
            return;

        Color c = _Image.color;
        c.a = 0.0f;
        _Image.color = c;

        gameObject.SetActive(true);
        _Image.DOFade(1, 0.6f);

        if (Result.IsWinner)
        {
            _Image.sprite = _sprites[0];
            _IsClear = true;
        }
        else
        {
            _Image.sprite = _sprites[1];
            _IsClear = false;
        }
    }

    private void OnStageClearEvent()
    {
        var GameMgr = GameManager.instance;
        if(GameMgr == null)
        {
            Debug.Log("[GameresultUI] Not Find GameMgr");
            return;
        }

        if(_IsClear)
        {
            // Stage 접근해서 몇개의 별을 가지고있는지 확인하기
            int StarCount = 3;
            EventBus.Publish<StageClearEvent>(new StageClearEvent(GameMgr.StageIndex, StarCount));
        }
        else
        {
            Debug.Log("[GameresultUI] Clear Fail Stage");
        }
    }

    private void OnDisable()
    {
        _Image.DOKill();
    }

    public void OnDestory()
    {
        _Image.DOKill();

        _SceneMoveCom.OnClicked -= OnStageClearEvent;
        EventBus.Unsubscribe<BattleEndEvent>(ResultUI);
    }
}
