Shader "Bakery/Skybox" {
	Properties {
		_Tint ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_Exposure ("Exposure", Float) = 1
		_MatrixRight ("Right", Vector) = (1,0,0,0)
		_MatrixUp ("Up", Vector) = (0,1,0,0)
		_MatrixForward ("Forward", Vector) = (0,0,1,0)
		[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "white" {}
		_NoTexture ("No texture", Float) = 0
		_Hemispherical ("Hemispherical", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}