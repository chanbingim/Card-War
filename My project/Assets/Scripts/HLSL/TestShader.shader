Shader "Custom/TestShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // ----------------------------------------
            // Instance Data
            // ----------------------------------------

            struct FontData
            {
                uint TexIndex;
                float4x4 WorldMatrix;
            };

            StructuredBuffer<FontData> _InstanceBuffer;


            // ----------------------------------------
            // Texture
            // ----------------------------------------

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)

                half4 _BaseColor;
                float4 _BaseMap_ST;

            CBUFFER_END


            // ----------------------------------------
            // Vertex
            // ----------------------------------------

            Varyings vert(
                Attributes IN,
                uint instanceID : SV_InstanceID
            )
            {
                Varyings OUT;

                // 현재 Instance의 데이터 가져오기
                FontData data = _InstanceBuffer[instanceID];

                // 기존 Object Space 위치
                float4 positionOS = IN.positionOS;

                // Instance별 World Matrix 적용
                float4 positionWS = mul(
                    data.WorldMatrix,
                    positionOS
                );

                // World → Clip
                OUT.positionHCS = mul(
                    UNITY_MATRIX_VP,
                    positionWS
                );

                OUT.uv = TRANSFORM_TEX(
                    IN.uv,
                    _BaseMap
                );

                return OUT;
            }


            // ----------------------------------------
            // Fragment
            // ----------------------------------------

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color =
                    SAMPLE_TEXTURE2D(
                        _BaseMap,
                        sampler_BaseMap,
                        IN.uv
                    ) * _BaseColor;

                return _BaseColor;
            }

            ENDHLSL
        }
    }
}