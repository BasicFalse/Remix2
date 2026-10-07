Shader "Custom/URP17/DissolveEdgeEmissiveShiny_Lightmap"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Progress ("Dissolve Progress", Range(0,1)) = 0
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _EdgeWidth ("Edge Width", Range(0.001,0.2)) = 0.05
        _EdgeEmission ("Edge Emission", Range(0,5)) = 1
        _Shininess ("Shininess (Smoothness)", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="TransparentCutout"
            "Queue"="AlphaTest"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // --- Lighting variants ---
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                float2 uvLM       : TEXCOORD1; // lightmap UV
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float2 uvLM        : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;

            float _Progress;
            float4 _EdgeColor;
            float _EdgeWidth;
            float _EdgeEmission;
            float _Shininess;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS = TransformWorldToHClip(o.positionWS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvLM = v.uvLM * unity_LightmapST.xy + unity_LightmapST.zw;
                o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float4 baseCol = tex2D(_MainTex, i.uv);
                float noise = tex2D(_NoiseTex, i.uv).r;

                float dissolve = noise - _Progress;

                // Alpha cutout
                clip(dissolve);

                // Edge mask
                float edgeMask = smoothstep(0.0, _EdgeWidth, dissolve);

                float3 albedo = lerp(_EdgeColor.rgb, baseCol.rgb, edgeMask);
                float3 emission = _EdgeColor.rgb * (1.0 - edgeMask) * _EdgeEmission;

                // -------- InputData --------
                InputData inputData;
                inputData.positionWS = i.positionWS;
                inputData.normalWS = normalize(i.normalWS);
                inputData.viewDirectionWS = GetWorldSpaceViewDir(i.positionWS);
                inputData.shadowCoord = i.shadowCoord;
                inputData.fogCoord = 0;
                inputData.vertexLighting = VertexLighting(i.positionWS, inputData.normalWS);

                // ✅ Baked lightmaps
                #if defined(LIGHTMAP_ON)
                    inputData.bakedGI = SampleLightmap(i.uvLM, inputData.normalWS);
                #else
                    inputData.bakedGI = SampleSH(inputData.normalWS);
                #endif

                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
                inputData.shadowMask = SAMPLE_SHADOWMASK(i.uvLM);

                // -------- SurfaceData --------
                SurfaceData surfaceData;
                surfaceData.albedo = albedo;
                surfaceData.metallic = 0;
                surfaceData.specular = 0;
                surfaceData.smoothness = _Shininess;
                surfaceData.normalTS = float3(0,0,1);
                surfaceData.occlusion = 1;
                surfaceData.emission = emission;
                surfaceData.alpha = 1;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;

                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }
    }
}
