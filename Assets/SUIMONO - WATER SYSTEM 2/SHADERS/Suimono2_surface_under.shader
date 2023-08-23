Shader "Suimono2/surface_under" {

Properties {
	//_MainTex ("Particle Texture", 2D) = "white" {}
	_TintColor1 ("Tint Color 1", Color) = (0.5,0.5,0.5,0.5)
	_TintColor2 ("Tint Color 2", Color) = (0.5,0.5,0.5,1.0)
	_BlendColor ("Blend Color", Color) = (0.0,0.0,0.0,0.0)
	_OverlayColor ("Overlay Color", Color) = (0.0,0.0,0.0,0.0)
	_ReflectionColor ("Reflection Color", Color) = (1.0,1.0,1.0,1.0)
	_SSSColor ("SubSurface Color", Color) = (1.0,1.0,1.0,1.0)
	_FoamColor ("Foam Color", Color) = (1.0,1.0,1.0,1.0)
	_CausticsColor ("Caustics Color", Color) = (1.0,1.0,1.0,1.0)
	_UnderwaterColor ("Caustics Color", Color) = (1.0,1.0,1.0,1.0)

	_NormalTexS ("Shallow Wave Normal Texture", 2D) = "white" {}
	_NormalTexD ("Deep Wave Normal Texture Large", 2D) = "white" {}
	_NormalTexR ("Rolling Wave Normal Texture Large", 2D) = "white" {}
	_ReflectionTex ("reflections", 2D) = "white" {}
	_CubeTex ("Cubemap reflections", CUBE) = "white" {}
	_FoamTex ("Foam Texture", 2D) = "white" {}
	_MaskTex ("Mask Texture", 2D) = "white" {}
	_NormalStrength ("Normal Strength", Range(0.0,1.0)) = 0.5
	_RefractStrength ("Refraction Strength", Range(0.0,1.0)) = 0.5
	_EdgeFade ("Edge Fade", Range(0.01,500.0)) = 1.0
	_EdgeFoamFade ("Edge Foam Fade", Range(5.0,500.0)) = 1.0
	_HeightFoamFade ("Edge Foam Fade", Range(5.0,500.0)) = 1.0
	_DepthFade ("Depth Fade", Range(0.01,5.0)) = 1.0
	_RefractFade ("Refract Fade", Range(0.01,500.0)) = 1.0
	_CausticsFade ("Caustics Fade", Range(0.01,500.0)) = 1.0

	_WaveTex ("Wave Texture", 2D) = "white" {}
	_shorelineScale ("Shoreline scale", Float) = 1.0
	_shorelineFrequency ("Shoreline Frequency", Float) = 1.0
	_shorelineSpeed  ("Shoreline Speed", Float) = 1.0
	_shorelineHeight  ("Shoreline Height", Float) = 1.0

	_Tess ("Tessellation", Float) = 4.0
    _minDist ("TessMin", Range(-180.0, 0.0)) = 10.0
    _maxDist ("TessMax", Range(20.0, 500.0)) = 25.0

    _suimono_uvx ("uvx1", Float) = 0.0
    _suimono_uvy ("uvy1", Float) = 0.0

}





//SURFACE UNDER DX11
SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="SuimonoWater" }
	Cull Front Lighting On ZWrite On

	CGPROGRAM

		#include "UnityCG.cginc"
		#include "SuimonoFunctionLibrary.cginc"

		#pragma target 5.0
		#pragma surface SuimonoSurf SuimonoLightUnder addshadow vertex:SuimonoVert tessellate:SuimonoTess

	ENDCG
}



//SURFACE UNDER DX9
SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="SuimonoWater" }
	Cull Front Lighting On ZWrite On

	CGPROGRAM

		#include "UnityCG.cginc"
		#include "SuimonoFunctionLibrary.cginc"

		#pragma target 3.0
		#pragma surface SuimonoSurf SuimonoLightUnder addshadow vertex:SuimonoVert

	ENDCG
}





fallback "Standard"

}
