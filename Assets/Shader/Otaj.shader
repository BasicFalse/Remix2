Shader "Custom/Otaj"
{
    Properties
    {
        _MainColor ("Blob Color", Color) = (1, 0.3, 0, 1)
        _Background ("Background Color", Color) = (0, 0, 0, 0) // Alpha 0
        _Glow ("Glow Intensity", Float) = 2
        _Speed ("Blob Speed", Float) = 1
        _BlobCount ("Blob Count", Int) = 5
        _LightDir ("Light Direction", Vector) = (0, 1, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _MainColor;
            fixed4 _Background;
            float _Glow;
            float _Speed;
            int _BlobCount;
            float4 _LightDir;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float SphereFalloff(float2 uv, float2 center, float radius)
            {
                float2 offset = uv - center;
                float d = length(offset / radius);
                if (d > 1) return 0;

                float z = sqrt(1 - d * d);
                return z;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y * _Speed;

                float3 finalColor = _Background.rgb;
                float finalAlpha = _Background.a;

                for (int b = 0; b < _BlobCount; b++)
                {
                    float id = float(b);
                    float angle = id * 6.283 / _BlobCount;

                    float2 center = float2(
                        0.5 + 0.25 * sin(t + angle * 1.1),
                        0.5 + 0.3 * sin(t * 1.2 + angle * 1.7)
                    );

                    float radius = 0.2;
                    float falloff = SphereFalloff(uv, center, radius);

                    if (falloff > 0)
                    {
                        float3 normal = normalize(float3(uv - center, falloff));
                        float3 lightDir = normalize(_LightDir.xyz);
                        float ndotl = saturate(dot(normal, lightDir));

                        float3 litColor = _MainColor.rgb * (0.4 + ndotl * 0.6);
                        litColor += pow(1 - ndotl, 6) * 0.5; // Rim light

                        finalColor = lerp(finalColor, litColor, falloff);
                        finalAlpha = max(finalAlpha, falloff);
                    }
                }

                return float4(finalColor * _Glow, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack Off
}
