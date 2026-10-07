Shader "Custom/URP/LitVectorDrivenTextureSpin"
{
    Properties
    {
        [MainTexture] _BaseMap ("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor ("Base Color", Color) = (1,1,1,1)

        _SpinVector ("Spin Vector (XY)", Vector) = (0,1,0,0)
        _SpinStrength ("Spin Strength", Float) = 1

        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
            };

            // 🔑 REQUIRED in Unity 6 / URP
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _SpinVector;
                float  _SpinStrength;
                float  _Cutoff;
                float  _Metallic;
                float  _Smoothness;
            CBUFFER_END

            float2 RotateUV(float2 uv, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);

                uv -= 0.5;
                uv = float2(
                    uv.x * c - uv.y * s,
                    uv.x * s + uv.y * c
                );
                uv += 0.5;

                return uv;
            }

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS  = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS    = TransformObjectToWorldNormal(v.normalOS);

                // ✅ Now _BaseMap_ST exists
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 spinDir = normalize(_SpinVector.xy);
                float speed = length(_SpinVector.xy) * _SpinStrength;
                float angle = _Time.y * speed * (spinDir.x + spinDir.y);

                float2 uv = RotateUV(i.uv, angle);

                half4 albedo = SampleAlbedoAlpha(
                    uv,
                    TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)
                );
                albedo *= _BaseColor;

                clip(albedo.a - _Cutoff);

                InputData inputData;
                inputData.positionWS = i.positionWS;
                inputData.normalWS = normalize(i.normalWS);
                inputData.viewDirectionWS = GetWorldSpaceViewDir(i.positionWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                inputData.vertexLighting = 0;
                inputData.bakedGI = SampleSH(inputData.normalWS);
                inputData.fogCoord = 0;
                
                SurfaceData surfaceData = (SurfaceData)0;

                surfaceData.albedo = albedo.rgb;
                surfaceData.alpha = albedo.a;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = half3(0,0,1);
                surfaceData.emission = 0;
                surfaceData.occlusion = 1;
                
                // Optional but explicit (future-proof)
                surfaceData.specular = 0;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;


                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }
    }
}
