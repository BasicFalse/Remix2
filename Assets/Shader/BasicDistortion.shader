Shader "Distortion/Basic"
{
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Name "Distortion"
            Tags { "LightMode"="UniversalForward" }

            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;
            float _WaveMinDistance;
            float _WaveMaxDistance;

            float4 _WaveOrigin;
            float4 _TargetRight;
            float4 _TargetUp;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
                float4 screenPos  : TEXCOORD1;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS = TransformWorldToHClip(o.worldPos);
                o.screenPos = ComputeScreenPos(o.positionCS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 toPos = i.worldPos - _WaveOrigin.xyz;

                float x = dot(toPos, normalize(_TargetRight.xyz));
                float y = dot(toPos, normalize(_TargetUp.xyz));

                float dist = length(toPos);

                float distMask = 1.0 - smoothstep(
                    _WaveMinDistance,
                    _WaveMaxDistance,
                    dist
                );

                float wave =
                    sin(x * _WaveFrequency + _Time.y * _WaveSpeed) *
                    sin(y * _WaveFrequency + _Time.y * _WaveSpeed);

                float2 offset = wave * _WaveAmplitude * distMask;

                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                screenUV += offset;

                half4 col = SAMPLE_TEXTURE2D(
                    _CameraOpaqueTexture,
                    sampler_CameraOpaqueTexture,
                    screenUV
                );

                col.a = distMask;
                return col;
            }
            ENDHLSL
        }
    }
}
