using DG.Tweening;
using UnityEngine;

public class EffectBase : PoolAbleComponent
{
    private ParticleSystem       _particleSystem = null;

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

    private void OnParticleSystemStopped()
    {
        Debug.Log("자동 재생 종료");
        ReturnToPool();
    }
}
