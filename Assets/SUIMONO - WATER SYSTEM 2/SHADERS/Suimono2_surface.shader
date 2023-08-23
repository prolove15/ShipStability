Shader "Suimono2/surface" {


Properties {
	//_MainTex ("Particle Texture", 2D) = "white" {}
	_shallowColor ("Tint Color 1", Color) = (0.5,0.5,0.5,0.5)
	_depthColor ("Tint Color 2", Color) = (0.5,0.5,0.5,1.0)
	_BlendColor ("Blend Color", Color) = (0.0,0.0,0.0,0.0)
	_OverlayColor ("Overlay Color", Color) = (0.0,0.0,0.0,0.0)
	_ReflectionColor ("Reflection Color", Color) = (1.0,1.0,1.0,1.0)
	_SpecularColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
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
	_EdgeFoamFade ("Edge Foam Amt", Range(5.0,500.0)) = 1.0
	_foamScale ("Foam Scale", Range(5.0,500.0)) = 1.0
	_ShallowFoamAmt ("Shallow Foam Amt", Range(5.0,500.0)) = 1.0
	_HeightFoamAmt ("Edge Foam Amt", Range(5.0,500.0)) = 1.0
	_HeightFoamHeight ("Height Foam Height", Range(5.0,500.0)) = 1.0
	_HeightFoamSpread ("Height Foam Spread", Range(5.0,500.0)) = 1.0

	_DepthFade ("Depth Fade", Range(0.01,5.0)) = 1.0
	_ShallowFade ("Shallow Fade", Range(0.01,5.0)) = 1.0
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



//SURFACE DX11
SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="SuimonoWater"}
	Cull Back Lighting On ZWrite On

	CGPROGRAM
		#include "UnityCG.cginc"
		#include "SuimonoFunctionLibrary.cginc"

		#pragma target 5.0
		#pragma surface SuimonoSurf SuimonoLight addshadow vertex:SuimonoVert tessellate:SuimonoTess

	ENDCG

}





//SURFACE DX9
SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="SuimonoWater" }
	Cull Back Lighting On ZWrite On

	CGPROGRAM
		#include "UnityCG.cginc"
		#include "SuimonoFunctionLibrary.cginc"

		#pragma target 3.0
		#pragma surface SuimonoSurf SuimonoLight addshadow vertex:SuimonoVert
	ENDCG

}



fallback "Diffuse"

}
