Shader "Spiral/SpiralTwoColor"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,0,0,1)
        _ColorB ("Color B", Color) = (0,0,1,1)

        _SpiralDensity ("Spiral Density", Float) = 8
        _SpiralsPerColor ("Spirals Per Color", Float) = 1
        _Sharpness ("Sharpness", Range(0.1, 10)) = 2

        _RotationSpeed ("Rotation Speed", Float) = 1
        _RadialScale ("Radial Scale", Float) = 1
        _Softness ("Blend Softness", Range(0.001, 1)) = 0.1
        _AngleOffset ("Angle Offset", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
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

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;

                float _SpiralDensity;
                float _SpiralsPerColor;
                float _Sharpness;

                float _RotationSpeed;
                float _RadialScale;
                float _Softness;
                float _AngleOffset;
            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                // Center UVs
                float2 uv = i.uv * 2.0 - 1.0;

                float radius = length(uv) * _RadialScale;
                float angle = atan2(uv.y, uv.x);

                // Animate rotation
                angle += _Time.y * _RotationSpeed + _AngleOffset;

                // Spiral value
                float spiral = angle + radius * _SpiralDensity;

                // Control how many spirals per color
                spiral *= _SpiralsPerColor * 2.0;

                // Base sine wave
                float wave = sin(spiral);

                // Sharpen the wave (contrast boost)
                wave = sign(wave) * pow(abs(wave), _Sharpness);

                // Smooth blend
                float mask = smoothstep(-_Softness, _Softness, wave);

                return lerp(_ColorA, _ColorB, mask);
            }
            ENDHLSL
        }
    }
}
