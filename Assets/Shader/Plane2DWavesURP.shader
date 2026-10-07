Shader "Distortion/TargetRotatedWaveDistortionURP_XR"
{
    Properties
    {
        _WaveOrigin     ("Wave Origin (World)", Vector) = (0,0,0,0)
        _TargetRight    ("Target Right", Vector)        = (1,0,0,0)
        _TargetUp       ("Target Up", Vector)           = (0,1,0,0)

        _WaveAmplitude  ("Wave Amplitude", Float) = 0.05
        _WaveFrequency  ("Wave Frequency", Float) = 6
        _WaveSpeed      ("Wave Speed", Float)     = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Required for XR SPI
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            half3 _WaveOrigin;
            half3 _TargetRight;
            half3 _TargetUp;

            half _WaveAmplitude;
            half _WaveFrequency;
            half _WaveSpeed;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 screenPos  : TEXCOORD0;
                half3 positionWS  : TEXCOORD1;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.screenPos   = ComputeScreenPos(o.positionHCS);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // XR-correct screen UVs
                half2 screenUV = i.screenPos.xy / i.screenPos.w;
                screenUV = UnityStereoTransformScreenSpaceTex(screenUV);

                // World-space offset
                half3 toPoint = i.positionWS - _WaveOrigin;

                // Normalize safely
                half3 right = normalize(_TargetRight + 1e-4);
                half3 up    = normalize(_TargetUp    + 1e-4);

                // Project into rotated 2D space
                half2 waveUV;
                waveUV.x = dot(toPoint, right);
                waveUV.y = dot(toPoint, up);

                half dist = sqrt(dot(waveUV, waveUV));

                half wave =
                    sin(dist * _WaveFrequency - _Time.y * _WaveSpeed)
                    * _WaveAmplitude;

                half2 distortionDir = normalize(waveUV + 1e-4);

                half2 distortedUV = screenUV + distortionDir * wave;

                half4 col = SAMPLE_TEXTURE2D(
                    _CameraOpaqueTexture,
                    sampler_CameraOpaqueTexture,
                    distortedUV
                );

                col.a = 1;
                return col;
            }
            ENDHLSL
        }
    }
}
