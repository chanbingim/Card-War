Shader "Custom/TestShader"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _Count("Count", Float) = 1
        _WorldPosition("World Position", Vector) = (0, 0, 0, 0)
        _Scale("Scale", Vector) = (1, 1, 1, 0)
        _ParentAlpha("Parent Alpha", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ----------------------------------------
            // Vertex / Fragment
            // ----------------------------------------

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
                uint DigitIndex;
            };

            StructuredBuffer<FontData> _InstanceBuffer;

            // ----------------------------------------
            // Global Data
            // ----------------------------------------

            float _Count;
            float4 _WorldPosition;
            float4 _Scale;
            float _ParentAlpha;

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

                FontData data = _InstanceBuffer[instanceID];

                // 1. 전체 Scale을 기준으로 글자 하나의 크기 계산
                float digitWidth = _Scale.x / _Count;
                
                // 2. 현재 글자의 위치 계산
                float centerIndex = (_Count - 1.0) * 0.5;
                float xOffset = (instanceID - centerIndex) * digitWidth;

                // 3. Quad의 Local Position
                // 들어오는 삼각형 영역의 위치를 크기만큼 늘리는 행위
                float4 positionOS = IN.positionOS;
                positionOS.x *= digitWidth;
                positionOS.y *= _Scale.y;
                positionOS.z *= _Scale.z;

                // 4. 글자별 위치를 World Position에 적용
                float3 positionWS = _WorldPosition.xyz;
                positionWS.x += xOffset;
                positionWS += positionOS.xyz;

                // 5. World → Clip
                OUT.positionHCS = TransformWorldToHClip(positionWS);

                // 6. Digit Atlas UV
                float cellWidth = 30.0 / 300.0;
                float uMin = data.DigitIndex * cellWidth;
                float uMax = (data.DigitIndex + 1) * cellWidth;

                OUT.uv.x = lerp(
                    uMin,
                    uMax,
                    IN.uv.x
                );

                OUT.uv.y = IN.uv.y;
                return OUT;
            }

            // ----------------------------------------
            // Fragment
            // ----------------------------------------

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    IN.uv
                );

                if (color.a <= 0.1)// || _ParentAlpha <= 0.1)
                    discard;

                return color;
            }

            ENDHLSL
        }
    }
}