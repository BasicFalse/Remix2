Shader "Custom/EdgeAndFaceShader"
{
    Properties
    {
        _FaceColor ("Face Color", Color) = (1, 1, 1, 1)
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _EdgeThickness ("Edge Thickness", Range(0.001, 0.1)) = 0.02
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

            struct appdata
            {
                float4 vertex : POSITION;
                float3 bary : TEXCOORD0; // custom barycentric coordinates
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 bary : TEXCOORD0;
            };

            fixed4 _FaceColor;
            fixed4 _EdgeColor;
            float _EdgeThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.bary = v.bary;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float edgeFactor = min(min(i.bary.x, i.bary.y), i.bary.z);
                float edge = smoothstep(_EdgeThickness, 0.0, edgeFactor);
                return lerp(_EdgeColor, _FaceColor, edge);
            }
            ENDCG
        }
    }
}
