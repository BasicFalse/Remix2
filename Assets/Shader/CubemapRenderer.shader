Shader "Cubemap/CubemapRenderer"
{
    Properties
    {
        _CubeMap ("Cubemap", CUBE) = "" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", Float) = 1
        _NearbyFlatten ("Nearby Flatten", Float) = 0
        _RotationShift ("Rotation Shift (Degrees)", Float) = 0
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

            samplerCUBE _CubeMap;
            fixed4 _TintColor;
            float _Intensity;
            float _NearbyFlatten;
            float _RotationShift;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldDir = worldPos - _WorldSpaceCameraPos;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.worldDir + float3(0, _NearbyFlatten, 0));

                // Convert degrees to radians
                float angle = radians(_RotationShift);

                // Rotate around Y axis
                float s = sin(angle);
                float c = cos(angle);
                float3x3 rotY = float3x3(
                    c, 0, -s,
                    0, 1,  0,
                    s, 0,  c
                );

                dir = mul(rotY, dir);

                fixed4 cubeColor = texCUBE(_CubeMap, dir);
                return cubeColor * _TintColor * _Intensity;
            }
            ENDCG
        }
    }
}
