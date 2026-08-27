using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : UIBase
{
    [SerializeField] List<GameObject>   _CardActIcon;
    [SerializeField] List<Image>        _ArrowActIcon;

    void Awake()
    {
        EventBus.Subscribe<ChangeTurnEvent>(View_TurnUI);
        EventBus.Subscribe<ChangeTurnActEvent>(TurnActionChangeEvent);

        gameObject.SetActive(false);
    }

    private void View_TurnUI(ChangeTurnEvent turnStartEvent)
    {
        gameObject.SetActive(true);

        if (turnStartEvent._IsLocal)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    private void TurnActionChangeEvent(ChangeTurnActEvent turnStartEvent)
    {
        var IconTransform = _CardActIcon[(int)turnStartEvent.eTurnType].transform;

        if (turnStartEvent.eTurnType == TurnManager.ETurnType.USE_CARDTRUN)
        {
            IconTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.6f);
        }
        else
        {
            int preActIndex = (int)turnStartEvent.eTurnType - 1;
            var PreIconTransform = _CardActIcon[preActIndex].transform;

            Sequence seq = DOTween.Sequence()
                .Append(PreIconTransform.DOScale(Vector3.one, 0.5f))
                .Append(_ArrowActIcon[preActIndex].DOColor(Color.yellowNice, 0.3f))
                .Append(IconTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f))
                .Append(_ArrowActIcon[preActIndex].DOColor(Color.white, 0.3f))
                .OnComplete(() =>
                {
                    turnStartEvent._OnCompleted?.Invoke();
                });
        }
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<ChangeTurnEvent>(View_TurnUI);
        EventBus.Unsubscribe<ChangeTurnActEvent>(TurnActionChangeEvent);
        base.OnDestroy();
    }
}
