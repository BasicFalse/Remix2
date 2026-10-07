Shader "Hidden/CameraGlitchBlockJumpBleed"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlockSize ("Block Size", Float) = 0.05
        _JumpChance ("Jump Chance", Float) = 0.5
        _JumpStrength ("Jump Strength", Float) = 0.4
        _BleedStrength ("Bleed Strength", Float) = 0.4
        _BleedRadius ("Bleed Radius", Float) = 0.005
        _Seed ("Seed", Float) = 0
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _BlockSize;
            float _JumpChance;
            float _JumpStrength;
            float _BleedStrength;
            float _BleedRadius;
            float _Seed;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            // Deterministic hash
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Identify block
                float2 blockID = floor(uv / _BlockSize);

                // Stable per-block random
                float r = hash(blockID + _Seed);

                // Decide if THIS block jumps
                float jump = step(r, _JumpChance);

                // Random source block
                float2 sourceBlock = blockID +
                    (float2(
                        hash(blockID + _Seed + 3.1),
                        hash(blockID + _Seed + 7.9)
                    ) - 0.5) * _JumpStrength * 10;

                float2 sourceUV =
                    (sourceBlock + frac(uv / _BlockSize)) * _BlockSize;

                sourceUV = saturate(sourceUV);

                fixed4 baseCol = tex2D(_MainTex, uv);
                fixed4 jumpCol = tex2D(_MainTex, sourceUV);

                // Base result
                fixed4 col = lerp(baseCol, jumpCol, jump);

                // ---- REAL COLOR BLEED (spatial smear) ----
                float2 b = float2(_BleedRadius, 0);
                fixed4 bleed =
                    tex2D(_MainTex, uv + b) +
                    tex2D(_MainTex, uv - b) +
                    tex2D(_MainTex, uv + b.yx) +
                    tex2D(_MainTex, uv - b.yx);

                bleed *= 0.25;

                col = lerp(col, bleed, _BleedStrength * jump);

                return col;
            }
            ENDCG
        }
    }
}
