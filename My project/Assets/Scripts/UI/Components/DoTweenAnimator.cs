using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DoTweenAnimator : MonoBehaviour
{
    [SerializeReference] private List<UIAnimData>    _AnimList;
    private List<UIAnimation>                       _AnimationList;
    private BitArray                                _AnimFlag;

    [SerializeField] private float     _TotalPlayTime = 1f;
    private float     _AnimRate = 1f;
    [SerializeField] private bool      _bIsLoop = false;

    private float           _CurAnimTime = 0f;
    public bool             _AnimationPasue { get; private set; }
    private int             _AnimFrame = 0;
    private int             _TotalFrame = 0;

    public void Initialize()
    {
        _AnimationList = new List<UIAnimation>();
        _AnimationList.Capacity = _AnimList.Count;
        _AnimFlag = new BitArray(_AnimList.Count);
        _AnimFlag.SetAll(false);

        if (_AnimList.Count > 0)
        {
            _AnimList = _AnimList
                .OrderBy(x => x.EndFrame)
                .ThenBy(x => x.StartFrame)
                .ToList();

            foreach(var anim in _AnimList)
            {
                _AnimationList.Add(anim.Create());
            }

            _TotalFrame =  _AnimList.Last().EndFrame;
        }
    }

    public void Update()
    {
        if (_AnimationPasue)
        {
            // 전체 누적 시간
            _CurAnimTime += Time.deltaTime * _AnimRate;

            // 현재 프레임 계산
            float rate = Mathf.Clamp01(_CurAnimTime / _TotalPlayTime);
            _AnimFrame = Mathf.FloorToInt(rate * (_TotalFrame - 1));

            // 아직 실행되지 않았고, 시작 프레임에 도달했다면 실행
            for (int i = 0; i < _AnimationList.Count; i++)
            {
                if (!_AnimFlag[i] &&
                    _AnimFrame >= _AnimationList[i]._startFrame)
                {
                    _AnimationList[i].Play_Animation(transform, (float)_AnimationList[i]._endFrame / _TotalFrame);
                    _AnimFlag[i] = true;
                }
            }

            // 재생 종료
            if (_CurAnimTime >= _TotalPlayTime)
            {
                _CurAnimTime = 0f;
                if (_bIsLoop)
                {
                    _AnimFrame = 0;
                    _AnimFlag.SetAll(false);
                }
                else
                {
                    _AnimationPasue = false;
                }
            }
        }
    }

    public void Play_Animation() { _AnimationPasue = true; }
    public void Pause_Animation() { _AnimationPasue = false; }

    private void Stop_Animation()
    {
        foreach (var anim in _AnimationList)
        {
            anim.Release();
        }
    }

    private void OnEnable()
    {
        _AnimationPasue = true;
        _AnimFlag.SetAll(false);
    }

    private void OnDisable()
    {
        Stop_Animation();
    }

    private void OnDestroy()
    {
        Stop_Animation();
    }

}
