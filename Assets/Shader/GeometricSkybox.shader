Shader "Cubemap/GeometricSkybox"
{
    Properties
    {
        _CubeMap ("Cubemap", CUBE) = "" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Background+1" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _CubeMap;
            fixed4 _TintColor;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldDir = worldPos - _WorldSpaceCameraPos;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.worldDir);
                fixed4 cubeColor = texCUBE(_CubeMap, dir);
                return cubeColor * _TintColor;
            }
            ENDCG
        }
    }
}
