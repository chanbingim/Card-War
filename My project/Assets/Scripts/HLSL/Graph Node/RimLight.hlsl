float4 CalculateRimLight(float3 WSCameraPos, float3 WSPos, float3 WSNormal, float3 Color, float Power, float Width)
{
    float3 N = normalize(WSNormal);
    float3 V = normalize(WSCameraPos - WSPos);

    float rim = 1.0f - saturate(dot(N, V));
    rim = smoothstep(1.0 - max(Width, 1e-4), 1.0, rim);
    
    // Power: 영역 안에서의 falloff
    rim = pow(rim, max(Power, 1e-4));
    
    // Store the scalar rim factor in alpha so callers can shape the mask.
    return float4(Color * rim, rim);
}

// Sprite/Quad를 위한 Shader Graph Custom Function.
// 처리 흐름:
// 1. 기본 텍스처와 노멀맵을 샘플링한다.
// 2. 현재 픽셀 주변의 월드 위치 및 UV 변화량으로 TBN 기저를 복원한다.
// 3. Tangent Space 노멀을 World Space 노멀로 변환한다.
// 4. 변환된 노멀로 RimLight를 계산하여 기본 색상에 더한다.
void RimLight2D_float(UnityTexture2D BaseTex, UnityTexture2D NormalTex, float2 UV,
                      float3 WSCameraPos, float3 WSPos, float3 WSNormal,
                      float3 WSTangent, float3 WSBitangent,
                      float3 Color, float Power, float Intensity, out float4 Result)
{
    // 현재 UV에서 원본 색상과 노멀맵 값을 가져온다.
    float4 baseColor = SAMPLE_TEXTURE2D(BaseTex.tex, BaseTex.samplerstate, UV);
    float4 normalSample = SAMPLE_TEXTURE2D(NormalTex.tex, NormalTex.samplerstate, UV);

    // 노멀맵에 0~1 범위로 저장된 값을 -1~1 범위의 Tangent Space 방향으로 복원한다.
    float3 normalTS = normalize(UnpackNormal(normalSample));

    // 화면에서 인접한 픽셀 사이의 월드 위치와 UV 변화량을 구한다.
    // Spine 메시가 Bone Animation으로 변형되면 positionDx/positionDy도 함께 변하므로,
    // 별도의 Mesh Normal/Tangent가 없어도 현재 변형 상태를 반영할 수 있다.
    float3 positionDx = ddx(WSPos);
    float3 positionDy = ddy(WSPos);
    float2 uvDx = ddx(UV);
    float2 uvDy = ddy(UV);

    // UV 변화량 행렬의 행렬식이다.
    // 0에 가까우면 UV 면적이 없는 퇴화 삼각형이므로 정상적인 TBN 복원이 불가능하다.
    float uvDeterminant = uvDx.x * uvDy.y - uvDx.y * uvDy.x;

    float3 tangentWS;
    float3 bitangentWS;
    float3 surfaceNormalWS;

    if (abs(uvDeterminant) > 1e-8)
    {
        // 월드 위치 변화량과 UV 변화량의 관계를 역산하여
        // U축 방향(Tangent)과 V축 방향(Bitangent)을 World Space로 복원한다.
        float inverseDeterminant = rcp(uvDeterminant);
        tangentWS = normalize((positionDx * uvDy.y - positionDy * uvDx.y) * inverseDeterminant);
        bitangentWS = normalize((-positionDx * uvDy.x + positionDy * uvDx.x) * inverseDeterminant);

        // Tangent와 Bitangent에 모두 수직인 실제 표면 방향을 계산한다.
        surfaceNormalWS = normalize(cross(tangentWS, bitangentWS));

        // 미러링된 Attachment UV 또는 음수 Skeleton Scale에서는 외적 방향이 뒤집힐 수 있다.
        // 표면 노멀이 항상 카메라를 향하도록 방향을 보정한다.
        float3 viewDirectionWS = normalize(WSCameraPos - WSPos);
        float facingSign = dot(surfaceNormalWS, viewDirectionWS) < 0.0 ? -1.0 : 1.0;
        surfaceNormalWS *= facingSign;
    }
    else
    {
        // UV 면적이 없는 퇴화 삼각형에서는 고정된 화면 정렬 기저를 대신 사용한다.
        tangentWS = float3(1.0, 0.0, 0.0);
        bitangentWS = float3(0.0, 1.0, 0.0);
        surfaceNormalWS = normalize(WSCameraPos - WSPos);
    }

    // 복원한 TBN 기저를 이용하여 Tangent Space 노멀을 World Space로 변환한다.
    // 아래 벡터 조합은 mul(normalTS, TBN) 행렬 연산과 같은 의미다.
    float3 mappedWSNormal = normalize(
        tangentWS * normalTS.x +
        bitangentWS * normalTS.y +
        surfaceNormalWS * normalTS.z);

    // 카메라 방향과 변환된 World Normal의 각도로 RimLight를 계산한다.
    float4 rimLight = CalculateRimLight(WSCameraPos, WSPos, mappedWSNormal, Color, max(Power, 0.0001), 0.1f);

    // 투명 영역에는 Rim이 생기지 않도록 Base Alpha를 곱하고 원본 색상에 합성한다.
    float3 rimColor = rimLight.rgb * Intensity * baseColor.a;
    Result = float4(baseColor.rgb + rimColor, baseColor.a);
}
