Shader "Custom/ProximityBasedEye"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _TargPos ("Target Position", Vector) = (0,0,0,0)
        _OpenDistance ("Open Distance", Float) = 3.0
        _ScaleAmount ("Scale Amount", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _TargPos;
            float _OpenDistance;
            float _ScaleAmount;
            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 worldPos : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // Transform vertex to world space
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Distance from target
                float dist = distance(worldPos, _TargPos.xyz);

                // Get absolute open range and direction sign
                float dir = sign(_OpenDistance);
                float range = abs(_OpenDistance);

                // Compute open factor (0 = closed, 1 = fully open)
                float openFactor = saturate(1 - dist / range);

                // Apply direction: if OpenDistance < 0, invert open factor
                openFactor = (dir > 0) ? openFactor : 1 - openFactor;

                // Scale door vertically (Y axis)
                float scale = lerp(1.0, _ScaleAmount, openFactor);
                worldPos.y *= scale;

                // Convert back to object space for clipping transform
                o.pos = UnityObjectToClipPos(mul(unity_WorldToObject, float4(worldPos, 1.0)));
                o.worldPos = worldPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}