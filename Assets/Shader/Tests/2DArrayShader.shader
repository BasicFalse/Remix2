Shader "Custom/2DArrayShader"
{
    Properties
    {
        _TextureArray ("Texture Array", 2DArray) = "" {}
        _SliceIndex ("Slice Index", Range(0, 10)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Uniform properties
            UNITY_DECLARE_TEX2DARRAY(_TextureArray);
            float _SliceIndex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the 2D texture array at the given slice
                float3 texCoord = float3(i.uv, _SliceIndex);
                fixed4 color = UNITY_SAMPLE_TEX2DARRAY(_TextureArray, texCoord);
                return color;
            }
            ENDCG
        }
    }
}