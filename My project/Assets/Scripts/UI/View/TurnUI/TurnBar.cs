using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : UIBase
{
    [SerializeField] private List<GameObject>   _CardActIcon;
    [SerializeField] private List<Image>        _ArrowActIcon;

    [SerializeField] private Text               _TimeText;
    [SerializeField] private Slider             _TimeSlider;

    private TweenerCore<float, float, FloatOptions>     _TimerTween = null;
    private Tweener                                     _timeScaleTween = null;

    float       CurrentTime = 0;
    int         _PreSecond = 0;

    void Awake()
    {
        EventBus.Subscribe<ChangeTurnEvent>(View_TurnUI);
        EventBus.Subscribe<ChangeTurnActEvent>(TurnActionChangeEvent);

        gameObject.SetActive(false);
    }

    private void View_TurnUI(ChangeTurnEvent turnStartEvent)
    {
        if (turnStartEvent._IsLocal)
        {
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<ChangeTurnEvent>(View_TurnUI);
        EventBus.Unsubscribe<ChangeTurnActEvent>(TurnActionChangeEvent);
        base.OnDestroy();
    }

    protected override void OnDisable()
    {
        _TimeSlider.DOKill();
        base.OnDisable();
    }

    private void TurnActionChangeEvent(ChangeTurnActEvent turnStartEvent)
    {
        var IconTransform = _CardActIcon[(int)turnStartEvent.eTurnType].transform;

        if (turnStartEvent.eTurnType == ETurnType.USE_CARDTRUN)
        {
            IconTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.6f);
            TurnTimerSetting(turnStartEvent.MaxTurnTime, turnStartEvent._OnTimerCompleted);
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
                    if(turnStartEvent.eTurnType != ETurnType.END)
                        TurnTimerSetting(turnStartEvent.MaxTurnTime, turnStartEvent._OnTimerCompleted);

                    turnStartEvent._OnCompleted?.Invoke();
                });
        }
    }

    private void TurnTimerSetting(float MaxTurnTime, Action OnCompleted)
    {
        if (_TimerTween != null)
        {
            _TimerTween.Kill();
            _TimerTween = null;
        }

        _PreSecond = -1;
        CurrentTime = MaxTurnTime;
        _TimeText.color = Color.white;
        _TimerTween = DOTween.To(() => CurrentTime, value =>
        {
            CurrentTime = value;
            _TimeSlider.value = CurrentTime / MaxTurnTime;

            int second = Mathf.Max(0, Mathf.CeilToInt(CurrentTime));
            if (second == _PreSecond)
                return;

            _PreSecond = second;
            _TimeText.text = second.ToString();

            if (CurrentTime <= 10.0f)
            {
                if (_TimeText.color == Color.white)
                    _TimeText.color = Color.red;

                _timeScaleTween?.Kill();
                _TimeText.transform.localScale = Vector3.one;
                _timeScaleTween = _TimeText.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f, 1, 0);
            }

        }, 0, MaxTurnTime).SetEase(Ease.Linear)
            .OnComplete(() => {
                _timeScaleTween = null;
                _TimerTween = null;
                OnCompleted?.Invoke();
            });
    }
}
