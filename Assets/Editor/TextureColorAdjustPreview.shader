Shader "Hidden/Editor/TextureColorAdjustPreview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Hue ("Hue", Float) = 0
        _Saturation ("Saturation", Float) = 1
        _Contrast ("Contrast", Float) = 1
        _Brightness ("Brightness", Float) = 0
        _Gamma ("Gamma", Float) = 1
        _Invert ("Invert Colors", Float) = 0

        _BlurStrength ("Blur Strength", Float) = 0
        _BlurType ("Blur Type", Float) = 0
        _BlurAngle ("Blur Angle", Float) = 0
    }

    SubShader
    {
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float _Hue, _Saturation, _Contrast, _Brightness, _Gamma, _Invert;
            float _BlurStrength, _BlurType, _BlurAngle;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 RGBToHSV(float3 c)
            {
                float4 K = float4(0., -1./3., 2./3., -1.);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                return float3(abs(q.z + (q.w - q.y) / (6. * d + 1e-10)), d / (q.x + 1e-10), q.x);
            }

            float3 HSVToRGB(float3 c)
            {
                float4 K = float4(1., 2./3., 1./3., 3.);
                float3 p = abs(frac(c.xxx + K.xyz) * 6. - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float4 BoxBlur(float2 uv)
            {
                float2 off = _MainTex_TexelSize.xy * _BlurStrength;
                float4 c = 0;

                for (int y = -1; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                    c += tex2D(_MainTex, uv + off * float2(x, y));

                return c / 9.0;
            }

            float4 GaussianBlur(float2 uv)
            {
                float2 off = _MainTex_TexelSize.xy * _BlurStrength;
                float4 c = tex2D(_MainTex, uv) * 0.4;
                c += tex2D(_MainTex, uv + off) * 0.3;
                c += tex2D(_MainTex, uv - off) * 0.3;
                return c;
            }

            float4 DirectionalBlur(float2 uv)
            {
                float angle = radians(_BlurAngle);
                float2 dir = float2(cos(angle), sin(angle));
                float2 off = dir * _MainTex_TexelSize.xy * _BlurStrength;

                float4 c = tex2D(_MainTex, uv);
                c += tex2D(_MainTex, uv + off);
                c += tex2D(_MainTex, uv - off);
                return c / 3.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col;

                if (_BlurType < 0.5)
                    col = tex2D(_MainTex, i.uv);
                else if (_BlurType < 1.5)
                    col = BoxBlur(i.uv);
                else if (_BlurType < 2.5)
                    col = GaussianBlur(i.uv);
                else
                    col = DirectionalBlur(i.uv);

                float3 hsv = RGBToHSV(col.rgb);
                hsv.x = frac(hsv.x + (_Hue / 360.0));
                hsv.y *= _Saturation;
                col.rgb = HSVToRGB(hsv);

                col.rgb = ((col.rgb - 0.5) * _Contrast) + 0.5;
                col.rgb += _Brightness;
                col.rgb = pow(max(col.rgb, 0), 1.0 / max(_Gamma, 0.0001));

                if (_Invert > 0.5)
                    col.rgb = 1.0 - col.rgb;

                return float4(saturate(col.rgb), col.a);
            }
            ENDCG
        }
    }
}
