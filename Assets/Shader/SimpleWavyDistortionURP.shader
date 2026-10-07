Shader "Distortion/WavyDistortionVisibleURP_XR"
{
    Properties
    {
        _DistortionStrength ("Distortion Strength", Range(0,0.2)) = 0.08
        _WaveScale ("Wave Scale", Float) = 15
        _WaveSpeed ("Wave Speed", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // XR support
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            half _DistortionStrength;
            half _WaveScale;
            half _WaveSpeed;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 screenPos  : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.screenPos   = ComputeScreenPos(o.positionHCS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // XR-correct screen UVs
                half2 uv = i.screenPos.xy / i.screenPos.w;
                uv = UnityStereoTransformScreenSpaceTex(uv);

                half t = _Time.y * _WaveSpeed;

                half2 wave;
                wave.x = sin((uv.y + t) * _WaveScale);
                wave.y = cos((uv.x + t) * _WaveScale);

                uv += wave * _DistortionStrength;

                // Prevent sampling outside camera texture
                uv = saturate(uv);

                half4 col = SAMPLE_TEXTURE2D(
                    _CameraOpaqueTexture,
                    sampler_CameraOpaqueTexture,
                    uv
                );

                col.a = 1;
                return col;
            }
            ENDHLSL
        }
    }
}
