using UnityEngine;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private SceneMoveComponent _SceneMoveCom = null;
    
    private Animator _Animator = null;
    private bool _IsClear = false;

    void Start()
    {
        if(_SceneMoveCom != null )
        {
            _SceneMoveCom.OnClicked += OnStageClearEvent;
        }

        _Animator = gameObject.GetComponent<Animator>();
        gameObject.SetActive(false);
        EventBus.Subscribe<BattleEndEvent>(ResultUI);
    }

    private void ResultUI(BattleEndEvent Result)
    {
        _Animator.SetBool("IsWin", Result.IsWinner);
        gameObject.SetActive(true);
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
    }

    public void OnDestory()
    {
        _SceneMoveCom.OnClicked -= OnStageClearEvent;
        EventBus.Unsubscribe<BattleEndEvent>(ResultUI);
    }
}
