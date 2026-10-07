Shader "Gorilla/Halloween22/CandleFlameWobble_URP" {
    Properties {
        _MainTex("MainTex", 2D) = "white" {}
        _FlameWobbleNoise("FlameWobbleNoise", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType" = "Transparent" }
        Pass {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_FlameWobbleNoise);
            SAMPLER(sampler_FlameWobbleNoise);

            Varyings vert(Attributes v) {
                Varyings o;
                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz);
                
                // Simpler wobble using sin and vertex position
                float wobble = sin(_Time.y * 3.0 + worldPos.y * 2.0) * 0.05;
                worldPos.x += wobble;
                
                o.positionCS = TransformWorldToHClip(worldPos);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                col.a *= 0.8; // Slight transparency
                return col;
            }
            ENDHLSL
        }
    }
}