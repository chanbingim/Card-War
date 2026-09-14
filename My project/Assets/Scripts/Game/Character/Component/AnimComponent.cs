using Spine;
using Spine.Unity;
using System.Collections.Generic;
using TurnCardGame.Data;
using UnityEngine;

public class AnimComponent : MonoBehaviour
{
    [System.Serializable]
    public class AnimationConfig
    {
        public AnimationGroup[] groups;
    }

    [System.Serializable]
    public class AnimationGroup
    {
        public string type;
        public string[] animationNames;
    }

    Dictionary<EFSM_STATE, System.Action>           _AnimationEvents = new();
    Dictionary<EFSM_STATE, List<Spine.Animation>>   _AnimationMap = new();

    SkeletonAnimation _Animation = null;
    EFSM_STATE          eCurState;

    private void Awake()
    {
        var Animation = GetComponent<SkeletonAnimation>();
        Animation.AnimationState.Event += HandleAnimationEvent;
    }

    public void Initialize(CharacterData data, SkeletonAnimation animaton)
    {
        _Animation = animaton;
        var AddressableMgr = AddressableManager.instance;
        if (AddressableMgr == null)
        {
            Debug.Log("[Anim Component] Not Find AddressableManager");
            return;
        }

        if (data.SkeletonDataKey != null)
        {
            _Animation.ClearState();

            _Animation.skeletonDataAsset = AddressableMgr.Get<SkeletonDataAsset>(data.SkeletonDataKey);
            _Animation.Initialize(true);
        }

        var config = JsonUtility.FromJson<AnimationConfig>(data.AnimationDatas.text);
        foreach (var group in config.groups)
        {
            if (System.Enum.TryParse<EFSM_STATE>(group.type, out var type))
            {
                foreach(var AnimName in group.animationNames)
                {
                    var Anim = animaton.Skeleton.Data.FindAnimation(AnimName);
                    if (Anim == null)
                        continue;

                    if (_AnimationMap.TryGetValue(type, out var animations))
                    {
                        animations.Add(Anim);
                    }
                    else
                    {
                        List<Spine.Animation> anims = new();
                        anims.Add(Anim);

                       _AnimationMap.Add(type, anims);
                    }
                        
                }
            }
        }
    }

    public void AddListener(EFSM_STATE eState, System.Action e)
    {
        if(_AnimationEvents.TryGetValue(eState, out var events))
        {
            events = e + events;
        }
        else
        {
            _AnimationEvents.Add(eState, e);
        }
    }

    public void RemoveListener(EFSM_STATE eState, System.Action e)
    {
        if (!_AnimationEvents.TryGetValue(eState, out System.Action current))
            return;

        current -= e;
        if (current == null)
            _AnimationEvents.Remove(eState);
        else
            _AnimationEvents[eState] = current;
    }

    public void ChangeAnim(EFSM_STATE eState, int idx = 0, bool loop = true, int StartFrame = 0)
    {
        if (!_AnimationMap.TryGetValue(eState, out var anims))
            return;

        if (idx < 0 || idx >= anims.Count)
            return;

        Spine.Animation target = anims[idx];

        var current = _Animation.AnimationState.GetCurrent(0);
        if (current?.Animation == target)
            return;

        eCurState = eState;
        var Track = _Animation.AnimationState.SetAnimation(StartFrame, target, loop);
        Track.Event += HandleAnimationEvent;
        Track.Dispose += HandleTrackDisposed;
    }

    private void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if(e.Data.Name == "attack_hit")
        {
            _AnimationEvents[eCurState]?.Invoke();
        }
    }

    private void HandleTrackDisposed(TrackEntry track)
    {
        track.Event -= HandleAnimationEvent;
        track.Dispose -= HandleTrackDisposed;
    }

    private void OnDisable()
    {
        var Animation = GetComponent<SkeletonAnimation>();
        Animation.AnimationState.Event -= HandleAnimationEvent;
    }
}
