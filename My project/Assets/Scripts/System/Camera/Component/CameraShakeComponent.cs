using UnityEngine;

public class CameraShakeComponent : MonoBehaviour
{
    #region Enum
    // 카메라 흔들림의 세기 변화 방식
    public enum ECameraShakeKeyword
    {
        // 종료될 때까지 일정한 세기 유지
        Constant,

        // 일정한 속도로 세기 감소
        Linear,

        // 초반에는 세기를 유지하고 후반에 빠르게 감소
        EaseIn,

        // 초반에 빠르게 감소하고 후반에 부드럽게 종료
        EaseOut,

        // 시작과 종료 구간에서 부드럽게 세기 변화
        EaseInOut
    }
    #endregion

    #region Struct
    // 카메라 위치 흔들림 설정 데이터
    [System.Serializable]
    public struct CameraShakeData
    {
        public int ID;
        // 시간에 따른 흔들림 세기 변화 방식
        public ECameraShakeKeyword  Keyword;

        // 흔들림 지속시간(초)
        public float                Duration;

        // 각 축의 최대 위치 이동량
        // 예: (1, 1, 0)은 X축과 Y축만 흔들림
        public Vector3              PositionAmplitude;

        // 위치 흔들림에 적용할 전체 세기 배율
        public float                Magnitude;
    }
    #endregion

    [SerializeField] private string     _CameraKey;

    [SerializeField]  private float           magnitude = 3.0f;
    private float           remainingTime = 0;
    private Vector3         currentOffset;

    private void LateUpdate()
    {
        transform.localPosition -= currentOffset;
        currentOffset = Vector3.zero;

        if (remainingTime <= 0.0f)
            return;

        remainingTime -= Time.deltaTime;

        Vector2 randomOffset = Random.insideUnitCircle * magnitude;
        currentOffset = new Vector3(randomOffset.x, randomOffset.y, 0f);
        transform.localPosition += currentOffset;
    }

    [ContextMenu("StartAction")]
    public void ShakeCamera(CameraShakeData Data)
    {
        remainingTime = 1.0f;
    }

    #region Default
    public void Init()
    {
        var BattleMgr = BattleManager.instance;
        if (BattleMgr != null)
        {
            BattleMgr.RegisterCamera(_CameraKey, this);
        }
    }

    private void OnEnable()
    {
        var BattleMgr = BattleManager.instance;
        if (BattleMgr != null)
        {
            BattleMgr.RegisterCamera(_CameraKey, this);
        }
    }

    private void OnDisable()
    {
        var BattleMgr = BattleManager.instance;
        if (BattleMgr != null)
        {
            BattleMgr.UnRegisterCamera(_CameraKey);
        }
    }
    #endregion


}
