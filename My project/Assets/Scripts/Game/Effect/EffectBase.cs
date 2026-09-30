using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EffectBase : PoolAbleComponent
{
    private ParticleSystem       _particleSystem = null;
    private Coroutine            _returnCoroutine = null;

    void Awake()
    {
    
    }

    public void Play()
    {
        // 자식까지 시작은 해야함
        if( _particleSystem == null )
        {
            _particleSystem = GetComponent<ParticleSystem>();

        }

        if (_particleSystem.isPlaying)
            _particleSystem.DORestart();
        else
            _particleSystem.DOPlay();

        if (_returnCoroutine != null)
            StopCoroutine(_returnCoroutine);

        _returnCoroutine = StartCoroutine(WaitUntilFinished());
    }

    public void Stop()
    {
        // 자식까지 시작은 해야함
        if (_particleSystem == null)
        {
            Debug.LogWarning($"[EffectBase] Name : {name} Not find ParticleSystem");
            return;
        }

        _particleSystem.Stop();
    }

    private IEnumerator WaitUntilFinished()
    {
        // 매 프레임 확인할 필요가 없으면 간격을 둘 수 있음
        var particleMain = _particleSystem.main;

        float Time = particleMain.startLifetime.constantMax + particleMain.duration;
        yield return new WaitForSeconds(Time);

        _returnCoroutine = null;
        ReturnToPool();
    }
}
