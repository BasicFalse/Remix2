Shader "Monster/SparkleSwarm"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _HueSpeed ("Hue Speed", Float) = 1
        _AdditiveTint ("Additive Tint", Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _HueSpeed;
                float4 _AdditiveTint;
            CBUFFER_END

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _InstanceOffset)
            UNITY_INSTANCING_BUFFER_END(Props)

            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1., 2./3., 1./3., 3.);
                float3 p = abs(frac(c.xxx + K.xyz) * 6. - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float offset = UNITY_ACCESS_INSTANCED_PROP(Props, _InstanceOffset);

                float hue = frac(_Time.y * _HueSpeed + offset);
                float3 rgb = HSVtoRGB(float3(hue, 1, 1));

                float3 finalColor = rgb * _BaseColor.rgb;
                finalColor += _AdditiveTint.rgb;

                return float4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}
