float4 CalculateRimLight(float3 WSCameraPos, float3 WSPos, float3 WSNormal, float3 Color, float Power)
{
    float3 N = normalize(WSNormal);
    float3 V = normalize(WSCameraPos - WSPos);
    
    float rim = pow(1.0 - saturate(dot(N, V)), Power);
    return float4(Color * rim, 1);
}