using UnityEngine;

public class CameraShakeComponent : MonoBehaviour
{
    [SerializeField] private string _CameraKey;
    [SerializeField] private Camera targetCamera;

    [Header("툴(px) → Unity 단위 변환")]
    [Tooltip("툴의 intensity(px) 기준 화면 높이. 클수록 같은 px 값이 작게 흔들림")]
    [SerializeField] private float referenceHeightPx = 900f;
    [Tooltip("원근 카메라일 때 px 환산에 쓰는 초점 거리")]
    [SerializeField] private float perspectiveFocusDistance = 10f;

    [Header("테스트 (ContextMenu)")]
    [SerializeField] private int _testShakeId = 2002;

    private float remainingTime = 0;
    private CameraShakeProfile ShakeData = null;

    // 쉐이크 시작 시점의 기준 자세 (끝나면 여기로 복구)
    private Vector3 _baseLocalPos;
    private Quaternion _baseLocalRot;
    private float _baseOrthoSize;
    private float _baseFov;
    private float unitsPerPx;

    public struct ShakePoint
    {
        public Vector2 offsetPx;   // 위치 변위 (px)
        public float rotDeg;       // 회전 (도)
        public float scale;        // 확대 배율
        public float power;        // 현재 세기 (0~1)
    }

    private void Awake()
    {
        if (targetCamera == null) targetCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (remainingTime <= 0.0f)
            return;

        remainingTime -= Time.deltaTime;           // 히트스탑(timeScale 0) 중에도 진행
        if (remainingTime <= 0.0f)
        {
            remainingTime = 0.0f;
            Restore();                                     // 원래 자세로 복구
            return;
        }

        float elapsed = ShakeData.DurationSec - remainingTime;   // 초 단위 경과 시간
        ShakePoint pt = GetTechniquePoint(ShakeData.technique, ShakeData, elapsed);

        var offset = new Vector3(-pt.offsetPx.x, pt.offsetPx.y, 0f) * unitsPerPx;
        transform.localPosition = _baseLocalPos + _baseLocalRot * offset;
        transform.localRotation = _baseLocalRot * Quaternion.Euler(0f, 0f, pt.rotDeg);
        ApplyZoom(pt.scale);
    }

    // ── 재생 ────────────────────────────────────────────────────────────
    public void ShakeCamera(int shakeId)
    {
        if (shakeId <= 0) return;   // 0 이하 = 쉐이크 없음
        if (DataManager.instance.TryCameraData(shakeId, out var profile)) ShakeCamera(profile);
        else Debug.LogWarning($"[CameraShakeComponent] ShakeID {shakeId} 없음");
    }

    public void ShakeCamera(CameraShakeProfile Data)
    {
        if (Data == null || Data.durationMs <= 0f) return;

        if (remainingTime <= 0.0f)      // 재생 중이 아닐 때만 기준 자세 저장 (재생 중 재호출 시 유지)
            CaptureBase();

        ShakeData = Data;
        remainingTime = Data.DurationSec;   // ms → 초
    }

    public void Stop()
    {
        if (remainingTime <= 0.0f) return;
        remainingTime = 0.0f;
        Restore();
    }

    [ContextMenu("StartAction")]
    private void TestShake() => ShakeCamera(_testShakeId);   // ContextMenu 는 파라미터가 없어야 함

    // ── 기준 자세 저장 / 복구 ───────────────────────────────────────────
    private void CaptureBase()
    {
        _baseLocalPos = transform.localPosition;
        _baseLocalRot = transform.localRotation;
        _baseOrthoSize = targetCamera.orthographicSize;
        _baseFov = targetCamera.fieldOfView;

        float visibleHeight = targetCamera.orthographic
            ? _baseOrthoSize * 2f
            : 2f * perspectiveFocusDistance * Mathf.Tan(_baseFov * 0.5f * Mathf.Deg2Rad);
        unitsPerPx = visibleHeight / Mathf.Max(1f, referenceHeightPx);
    }

    private void Restore()
    {
        transform.localPosition = _baseLocalPos;
        transform.localRotation = _baseLocalRot;
        ApplyZoom(1f);
    }

    private ShakePoint GetTechniquePoint(ShakeTechnique technique, CameraShakeProfile p, float elapsedSec)
    {
        float progress = Mathf.Min(1f, elapsedSec / p.DurationSec);
        float power = GetPower(p.envelope, progress);

        Vector3 s;   // (x, y, r) 파형 -1 ~ 1
        switch (technique)
        {
            case ShakeTechnique.SmoothNoise: s = SampleNoise(elapsedSec, p.frequencyHz); break;
            case ShakeTechnique.DampedSpring: s = SampleSpring(progress, p.frequencyHz); break;
            case ShakeTechnique.DirectionalRecoil: s = SampleRecoil(progress); break;
            default: s = SampleLayered(elapsedSec, p.frequencyHz); break;
        }

        return new ShakePoint
        {
            offsetPx = new Vector2(s.x * p.intensityPx * power, s.y * p.intensityPx * power),
            rotDeg = s.z * p.rotationDeg * power,
            scale = 1f + p.zoomPercent * 0.01f * power,
            power = power
        };
    }

    // ── Power: 이전 점 / 현재 점 index 로 smoothstep 보간 ────────────────────
    private static float GetPower(EnvelopePoint[] env, float progress)
    {
        if (env == null || env.Length == 0) return 0f;
        if (progress <= env[0].time) return env[0].power;

        for (int curr = 1; curr < env.Length; curr++)
        {
            if (progress > env[curr].time) continue;

            int prev = curr - 1;
            float raw = (progress - env[prev].time) / Mathf.Max(0.001f, env[curr].time - env[prev].time);
            float smooth = raw * raw * (3f - 2f * raw);
            return env[prev].power + (env[curr].power - env[prev].power) * smooth;
        }
        return 0f;   // 마지막 점 이후
    }

    // ── 기법별 파형: 반환값 (x, y, r) ───────────────────────────────────────
    private static Vector3 SampleLayered(float t, float f)
    {
        return new Vector3(
            Wave(t, f, 0.72f, 1f),
            Wave(t, f, 2.31f, 0.83f) * 0.68f,
            Wave(t, f, 4.18f, 0.39f) * 0.39f);
    }

    private static Vector3 SampleNoise(float t, float f)
    {
        return new Vector3(
            ValueNoise(t * f, 0.37),
            ValueNoise(t * f * 0.78, 8.1) * 0.72f,
            ValueNoise(t * f * 0.42, 17.7) * 0.5f);
    }

    private static Vector3 SampleSpring(float progress, float f)
    {
        float damping = Mathf.Exp(-progress * 4.8f);
        float osc = Mathf.Sin(progress * Mathf.PI * 2f * Mathf.Max(2f, f / 5f));
        return new Vector3(osc * damping, -osc * damping * 0.28f, osc * damping * 0.42f);
    }

    private static Vector3 SampleRecoil(float progress)
    {
        const float kickEnd = 0.16f;
        float kick = progress < kickEnd
            ? -Mathf.Sin(progress / kickEnd * Mathf.PI / 2f)
            : Mathf.Exp(-(progress - kickEnd) * 6f) * Mathf.Sin((progress - kickEnd) * Mathf.PI * 4f) * 0.48f;

        return new Vector3(kick, kick * 0.16f + Mathf.Sin(progress * Mathf.PI * 5f) * 0.05f, -kick * 0.32f);
    }

    // ── 기본 수식 ──────────────────────────────────────────────────────────
    // 주파수 비율 1 / 1.73 / 0.47 사인파 3개 합성
    private static float Wave(float time, float frequency, float phase, float ratio)
    {
        float w = time * frequency * Mathf.PI * 2f * ratio;
        return Mathf.Sin(w + phase) * 0.58f
             + Mathf.Sin(w * 1.73f + phase * 1.9f) * 0.29f
             + Mathf.Sin(w * 0.47f + phase * 0.63f) * 0.13f;
    }

    // 격자 난수 사이를 smoothstep 으로 보간
    private static float ValueNoise(double position, double seed)
    {
        double b = System.Math.Floor(position);
        double fr = position - b;
        double smooth = fr * fr * (3.0 - 2.0 * fr);
        double h0 = Hash((int)b, seed);
        double h1 = Hash((int)b + 1, seed);
        return (float)(h0 + (h1 - h0) * smooth);
    }

    private static double Hash(int index, double seed)
    {
        double raw = System.Math.Sin((index + seed) * 12.9898) * 43758.5453;
        return (raw - System.Math.Floor(raw)) * 2.0 - 1.0;
    }

    // pt.scale = 1 + zoom% × Power  (1보다 크면 화면이 확대됨)
    private void ApplyZoom(float scale)
    {
        if (targetCamera.orthographic)
            targetCamera.orthographicSize = _baseOrthoSize / scale;
        else
            targetCamera.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(_baseFov * 0.5f * Mathf.Deg2Rad) / scale) * Mathf.Rad2Deg;
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
        Stop();   // 쉐이크 도중 비활성화되면 카메라를 원래 자세로 복구

        var BattleMgr = BattleManager.instance;
        if (BattleMgr != null)
        {
            BattleMgr.UnRegisterCamera(_CameraKey);
        }
    }
    #endregion
}