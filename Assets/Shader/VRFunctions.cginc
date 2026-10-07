bool IsLeftEye(float3 WorldCamPos, float3 WorldCamRight)
{
#if defined(USING_STEREO_MATRICES)
	return (unity_StereoEyeIndex == 0);
#elif defined (UNITY_DECLARE_MULTIVIEW)
	return (UNITY_VIEWID == 0);
#else

#if defined(UNITY_MATRIX_I_V)
	float3 renderCameraPos = UNITY_MATRIX_I_V._m03_m13_m23;
#else
	float3 renderCameraPos = _WorldSpaceCameraPos.xyz;
#endif

	float fL = distance(WorldCamPos - WorldCamRight, renderCameraPos);
	float fR = distance(WorldCamPos + WorldCamRight, renderCameraPos);
	return (fL < fR);
#endif
}


void StereoSwap(float3 WorldCameraPosition, float3 WorldCameraRight, float Force, float4 Left, float4 Right, out float4 Out) {
	Out = Left;
	if (IsStereoEyeLeft(WorldCameraPosition, WorldCameraRight, Force)){
	  Out = Left;
	}
	else {
	  Out = Right;
	}
}