using UnityEngine;
using UnityEngine.Playables;

public class TimeLineCoponent : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director = null;

    public void Play()
    {
        _director?.Play();
    }
}

