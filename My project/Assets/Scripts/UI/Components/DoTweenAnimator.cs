using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class DoTweenAnimator : MonoBehaviour
{
    public bool             _AnimationPasue { get; private set; }
    public event Action     OnCompleted;

    [SerializeReference] private List<UIAnimData>   _AnimList;
    [SerializeField]    private float               _TotalPlayTime = 1f;
    [SerializeField]    private bool                _bIsLoop = false;

    private List<UIAnimation>                       _AnimationList;
    private BitArray                                _AnimFlag;

    private bool        _bIsReverse = false;
    private int         _TrackIndex = 0;

    private float       _AnimRate = 1f;
    private float       _CurAnimTime = 0f;

    private int         _AnimFrame = 0;
    private int         _TotalFrame = 0;

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
                _AnimationList.Last().Initialize(transform);
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
            if (_bIsReverse)
                _AnimFrame = _TotalFrame - 1 - _AnimFrame;

            // 아직 실행되지 않았고, 시작 프레임에 도달했다면 실행
            if (!_AnimFlag[_TrackIndex])
            {
                float duration = (_AnimationList[_TrackIndex]._endFrame - _AnimationList[_TrackIndex]._startFrame) / (float)(_TotalFrame) * _TotalPlayTime;
                bool bIsPlay = false;
                if (_bIsReverse)
                {
                    if (_AnimFrame <= _AnimationList[_TrackIndex]._endFrame)
                        bIsPlay = true;
                }
                else
                {
                    if (_AnimFrame >= _AnimationList[_TrackIndex]._startFrame)
                        bIsPlay = true;
                }

                if(bIsPlay)
                {
                    _AnimationList[_TrackIndex].Play_Animation(duration, _bIsReverse);
                    _AnimFlag[_TrackIndex] = true;
                    _TrackIndex = Math.Clamp((_bIsReverse == true ? _TrackIndex - 1 : _TrackIndex + 1), 0, _AnimationList.Count - 1);
                }
            }

            // 재생 종료
            if (_CurAnimTime >= _TotalPlayTime)
            {
                _CurAnimTime = 0f;
                _AnimFlag.SetAll(false);
                if (_bIsLoop)
                {
                    _AnimFrame = 0;
                    _TrackIndex = _bIsReverse == false ? 0 : _AnimationList.Count - 1;
                }
                else
                {
                    _AnimationPasue = false;
                }

                OnCompleted?.Invoke();
            }
        }
    }

    public Task AsyncOnCompleted()
    {
        var tcs = new TaskCompletionSource<bool>();

        void Complete()
        {
            OnCompleted -= Complete;
            tcs.TrySetResult(true);
        }

        OnCompleted += Complete;
        return tcs.Task;
    }

    public float GetToatalAnimTime() { return _TotalPlayTime; }
    public void Play_Animation(bool IsReverse = false) 
    {
        _AnimationPasue = true;
        _AnimFlag.SetAll(false);
        _bIsReverse = IsReverse;

        _TrackIndex = (_bIsReverse == false ? 0 : _AnimationList.Count - 1);
    }

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
        if (_AnimationList == null)
            Initialize();

        Play_Animation();
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
