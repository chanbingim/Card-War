float4 CalculateRimLight(float3 WSCameraPos, float3 WSPos, float3 WSNormal, float3 Color, float Power)
{
    float3 N = normalize(WSNormal);
    float3 V = normalize(WSCameraPos - WSPos);
    
    float rim = pow(1.0 - saturate(dot(N, V)), Power);
    rim *= smoothstep(0.2, 0.3, rim);
    
    // Store the scalar rim factor in alpha so callers can shape the mask.
    return float4(Color * rim, rim);
}

// Shader Graph Custom Function for Sprite/Quad rendering.
void RimLight2D_float(UnityTexture2D BaseTex, UnityTexture2D NormalTex, float2 UV,
                      float3 WSCameraPos, float3 WSPos, float3 WSNormal,
                      float3 WSTangent, float3 WSBitangent,
                      float3 Color, float Power, float Intensity, out float4 Result)
{
    float4 baseColor = SAMPLE_TEXTURE2D(BaseTex.tex, BaseTex.samplerstate, UV);
    float4 normalSample = SAMPLE_TEXTURE2D(NormalTex.tex, NormalTex.samplerstate, UV);

    // Convert the sampled tangent-space normal with the mesh's real TBN basis.
    float3 normalTS = normalize(UnpackNormal(normalSample));
    float3x3 tangentToWorld = float3x3(
        normalize(WSTangent),
        normalize(WSBitangent),
        normalize(WSNormal));
    float3 mappedWSNormal = normalize(mul(normalTS, tangentToWorld));

    float4 rimLight = CalculateRimLight(
        WSCameraPos,
        WSPos,
        mappedWSNormal,
        Color,
        max(Power, 0.0001));

    float intensityGain = 1.0 - exp(-max(Intensity, 0.0));
    float rimMask = saturate(rimLight.a * intensityGain) * baseColor.a;

    Result = float4(baseColor.rgb + Color * rimMask, baseColor.a);
}
