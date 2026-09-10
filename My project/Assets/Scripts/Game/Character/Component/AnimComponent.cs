using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;

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


    Dictionary<EFSM_STATE, List<Spine.Animation>> _AnimationMap = new();
    SkeletonAnimation _Animation = null;

    public void Initialize(TextAsset json, SkeletonAnimation animaton)
    {
        var config = JsonUtility.FromJson<AnimationConfig>(json.text);

        foreach (var group in config.groups)
        {
            if (System.Enum.TryParse<EFSM_STATE>(group.type, out var type))
            {
                foreach(var AnimName in group.animationNames)
                {
                    var Anim = animaton.Skeleton.Data.FindAnimation(AnimName);
                    if (Anim == null)
                        continue;

                    if(_AnimationMap.TryGetValue(type, out var animations))
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

        _Animation = animaton;
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

        _Animation.AnimationState.SetAnimation(StartFrame, target, loop);
    }

}
