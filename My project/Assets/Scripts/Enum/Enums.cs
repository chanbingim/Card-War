using UnityEngine;

namespace GamePlay.Enum
{
    enum SOUND_TYPE
    {
        BGM, EFFECT, VOICE,END
    }

    public enum EAddressLabel
    {
        Sprtie,
        Altas,
        Prefabs,
        END,
    }

    public enum EPoolType
    {
        Obejct,
        UI,
        Effect,
        END
    }

    [System.Serializable]
    public enum ETargetType
    {
        FriendlySelf,
        FriendlyAll,
        EnemySelf,
        EnemyAll,
        None
    }
}