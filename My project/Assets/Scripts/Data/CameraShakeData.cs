// CSV 1행(=엔벨로프 포인트 1개)과 1:1 대응. 파싱 직후의 원본 행 형태
using System;
using System.Collections.Generic;

public enum ShakeTechnique
{
    LayeredWave = 0,
    SmoothNoise = 1,
    DampedSpring = 2,
    DirectionalRecoil = 3
}

// 같은 ShakeID 행들을 묶은 런타임 데이터 (실제로 쓰는 타입)
[Serializable]
public struct EnvelopePoint
{
    public float time;    // 0~1
    public float power;   // 0~1
}

[Serializable]
public class CameraShakeProfile
{
    public int id;
    public string name;
    public ShakeTechnique technique;
    public float intensityPx;
    public float durationMs;
    public float frequencyHz;
    public float rotationDeg;
    public float zoomPercent;
    public EnvelopePoint[] envelope;

    public float DurationSec => durationMs * 0.001f;
}

public class ReadCameraShakeJson
{
    public List<CameraShakeProfile> profiles = new();
}