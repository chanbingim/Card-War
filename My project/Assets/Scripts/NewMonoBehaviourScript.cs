using Spine;
using Spine.Unity;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private SkeletonAnimation skeletonAnimation;

    // 실제로 이벤트를 구독한 AnimationState
    private Spine.AnimationState _subscribedState;

    private void Reset()
    {
        skeletonAnimation =
            GetComponent<SkeletonAnimation>();
    }

    private void Awake()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation =
                GetComponent<SkeletonAnimation>();
        }

        if (skeletonAnimation == null)
        {
            Debug.LogError(
                "[SpineEventReceiver] " +
                "SkeletonAnimation이 없습니다.",
                this
            );

            enabled = false;
            return;
        }

        BindEvents();
    }

    private void OnEnable()
    {
        // Awake 이후 재활성화되는 경우를 처리
        if (skeletonAnimation != null)
            BindEvents();
    }

    private void OnDisable()
    {
        UnbindEvents();
    }

    private void OnDestroy()
    {
        UnbindEvents();
    }

    public void BindEvents()
    {
        if (skeletonAnimation == null)
            return;

        /*
         * false:
         * 이미 초기화됐다면 기존 Skeleton과
         * AnimationState를 유지합니다.
         */
        skeletonAnimation.Initialize(false);

        Spine.AnimationState currentState =
            skeletonAnimation.AnimationState;

        if (currentState == null)
        {
            Debug.LogError(
                "[SpineEventReceiver] " +
                "AnimationState 초기화 실패",
                this
            );

            return;
        }

        // 동일 State에 이미 구독한 경우
        if (ReferenceEquals(
                _subscribedState,
                currentState))
        {
            return;
        }

        // 이전 State 구독 제거
        UnbindEvents();

        _subscribedState = currentState;
        _subscribedState.Event += OnSpineEvent;

        Debug.Log(
            $"[SpineEventReceiver] 이벤트 구독 완료: " +
            $"{gameObject.name}",
            this
        );
    }

    public void UnbindEvents()
    {
        if (_subscribedState == null)
            return;

        _subscribedState.Event -= OnSpineEvent;
        _subscribedState = null;
    }

    private void OnSpineEvent(TrackEntry entry, Spine.Event spineEvent)
    {
        if (entry == null ||
            spineEvent == null ||
            spineEvent.Data == null)
        {
            return;
        }

        string animationName =
            entry.Animation?.Name ?? "Unknown";

        string eventName =
            spineEvent.Data.Name;

        Debug.Log(
            $"[Spine Event]\n" +
            $"Object={gameObject.name}\n" +
            $"Track={entry.TrackIndex}\n" +
            $"Animation={animationName}\n" +
            $"Event={eventName}\n" +
            $"EventTime={spineEvent.Time:0.###}\n" +
            $"TrackTime={entry.TrackTime:0.###}\n" +
            $"Int={spineEvent.Int}\n" +
            $"Float={spineEvent.Float:0.###}\n" +
            $"String={spineEvent.String}",
            this
        );
    }

}
