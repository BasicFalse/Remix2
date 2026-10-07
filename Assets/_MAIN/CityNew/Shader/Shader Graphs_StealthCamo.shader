Shader "Shader Graphs/StealthCamo" {
	Properties {
		_Base_Color ("Base Color", Vector) = (0.2866233,0.4609547,0.5377358,0.2784314)
		_Noise_Size ("Noise Size", Float) = 20
		_Noise_Strength ("Noise Strength", Float) = 0.5
		[NoScaleOffset] _MainTex ("_MainTex", 2D) = "white" {}
		[NoScaleOffset] _MouthMap ("_MouthMap", 2D) = "white" {}
		[NoScaleOffset] _MouthMap_Atlas ("_MouthMap_Atlas", 2DArray) = "" {}
		_Color ("_Color", Vector) = (0,0,0,0)
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}