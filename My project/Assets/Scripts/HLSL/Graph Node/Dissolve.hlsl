float DissolveEdge(float noiseValue, float Amount, float EdgeWidth)
{
    float safeWidth = max(EdgeWidth, 0.00001);
    float edgeGradient = 1.0 - smoothstep(Amount, Amount + safeWidth, noiseValue);
    return edgeGradient * step(Amount, noiseValue);
}

bool CalculateDissolve(UnityTexture2D MainTexture,
                    UnityTexture2D NoiseTexture,
                    float2 UV, float Amount, float EdgeWidth,
                    float4 EdgeColor,
                    out float4 Result)
{
    float4 Color = SAMPLE_TEXTURE2D(MainTexture.tex, MainTexture.samplerstate, UV);
    float n = SAMPLE_TEXTURE2D(NoiseTexture.tex, NoiseTexture.samplerstate, UV).r;

    float Edge = DissolveEdge(n, Amount, EdgeWidth);
    float3 edgeColor = lerp(Color.rgb, EdgeColor.rgb, Edge);
    
    if (n <= Amount)
        return false;
    
    Result = Amount <= 0.f ? Color : float4(edgeColor, Color.a);
    return true;
}

void Dissolve_float(UnityTexture2D MainTexture,
                    UnityTexture2D NoiseTexture,
                    float2 UV, float Amount, float EdgeWidth,
                    float4 EdgeColor,
                    out float4 Result)
{
    if (!CalculateDissolve(MainTexture, NoiseTexture, UV, Amount, EdgeWidth, EdgeColor, Result))
        discard;
}

void GachaDissolve_float(UnityTexture2D ItemTexture,
                    UnityTexture2D MainTexture,
                    UnityTexture2D NoiseTexture,
                    float2 UV, float Amount, float EdgeWidth,
                    float4 EdgeColor,
                    out float4 Result)
{
    if (!CalculateDissolve(MainTexture, NoiseTexture, UV, Amount, EdgeWidth, EdgeColor, Result))
        Result = SAMPLE_TEXTURE2D(ItemTexture.tex, ItemTexture.samplerstate, UV);
}