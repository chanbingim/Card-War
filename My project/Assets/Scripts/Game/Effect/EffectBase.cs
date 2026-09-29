using DG.Tweening;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    private ParticleSystem       _particleSystem = null;
    private PoolAbleComponent    _PoolableComponent = null;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if(_particleSystem == null)
            Debug.LogWarning($"[EffectBase] Name : {name} Not find ParticleSystem");

        // Callback 설정
        var Particle = _particleSystem.main;
        Particle.stopAction = ParticleSystemStopAction.Callback;

        _PoolableComponent = GetComponent<PoolAbleComponent>();
    }

    public void Play()
    {
        // 자식까지 시작은 해야함
        if( _particleSystem == null )
        {
            Debug.LogWarning($"[EffectBase] Name : {name} Not find ParticleSystem");
            return;
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
        _PoolableComponent?.ReturnToPool();
    }
}
