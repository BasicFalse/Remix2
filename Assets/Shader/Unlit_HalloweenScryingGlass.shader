Shader "Unlit/HalloweenScryableObjectSRP" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,0.5)
        [Toggle(_ALPHATEST_ON)] _AlphaTestToggle ("Alpha Clipping", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        [HideInInspector] _Cull ("__cull", Float) = 2
    }
    SubShader {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        LOD 200
        
        Stencil {
            Ref 1
            Comp always // Always write to stencil buffer
            Pass replace
        }

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        float _Cutoff;
        int _AlphaTestToggle;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            if (_AlphaTestToggle > 0)
                clip(c.a - _Cutoff);
            else
                o.Alpha = c.a;
        }
        ENDCG
    }
}
