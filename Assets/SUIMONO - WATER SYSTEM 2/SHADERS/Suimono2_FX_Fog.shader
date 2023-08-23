// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Suimono2/SuimonoFog" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "black" {}
}

CGINCLUDE

	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D_float _CameraDepthTexture;
	uniform float4 _HeightParams;
	uniform float4 _DistanceParams;

	float4 _SceneFogParams;

	uniform float4 _MainTex_TexelSize;
	uniform float4x4 _FrustumCornersWS;
	uniform float4 _CameraWS;

	half4 _FogColor,_FogColorHigh;
	float _fogSkybox;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv_depth : TEXCOORD1;
		float4 interpolatedRay : TEXCOORD2;
	};
	
	v2f vert (appdata_img v)
	{
		v2f o;
		half index = v.vertex.z;
		v.vertex.z = 0.1;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv_depth = v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif				
		
		o.interpolatedRay = _FrustumCornersWS[(int)index];
		o.interpolatedRay.w = index;
		
		return o;
	}
	

	// Distance-based fog
	float ComputeDistance (float3 camDir, float zdepth)
	{
		return length(camDir);
	}

	// Linear half-space fog, from https://www.terathon.com/lengyel/Lengyel-UnifiedFog.pdf
	float ComputeHalfSpace (float3 wsDir)
	{
		float3 wpos = _CameraWS + wsDir;
		float FH = _HeightParams.x;
		float3 C = _CameraWS;
		float3 V = wsDir;
		float3 P = wpos;
		float3 aV = _HeightParams.w * V;
		float FdotC = _HeightParams.y;
		float k = _HeightParams.z;
		float FdotP = P.y-FH;
		float FdotV = wsDir.y;
		float c1 = k * (FdotP + FdotC);
		float c2 = (1-2*k) * FdotP;
		float g = min(c2, 0.0);
		g = -length(aV) * (c1 - g * g / abs(FdotV+1.0e-5f));
		return g;
	}

	half4 ComputeFog (v2f i, bool distance, bool height) : SV_Target
	{
		half4 sceneColor = tex2D(_MainTex, i.uv);
		
		// Reconstruct world space position & direction
		// towards this screen pixel.
		float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv_depth);
		float dpth = Linear01Depth(rawDepth);
		float4 wsDir = dpth * i.interpolatedRay;
		float4 wsPos = _CameraWS + wsDir;

		// Compute fog distance
		float g = _DistanceParams.x;
		//distance
		g += ComputeDistance (wsDir, dpth);
		//height
		g += ComputeHalfSpace (wsDir);


		//CALCULATE FOG
		half usedpth = _DistanceParams.z + ComputeDistance(wsDir, dpth) + ComputeHalfSpace (wsDir);
		half fogFac = saturate(max(0.0,usedpth) * _SceneFogParams.z + _SceneFogParams.w);

		// Do not fog skybox
		if (_fogSkybox == 1.0){
			if (rawDepth >= 0.999999){
				fogFac = 1.0;
			}
		}

		_FogColor = lerp(_FogColor,_FogColorHigh,fogFac * _FogColorHigh.a);

		// Lerp between fog color & original scene color
		half4 fCol = lerp (lerp((half4)sceneColor, (half4)_FogColor* half4(0.4646,0.4646,0.4646,0.4646), _FogColor.a), (half4)sceneColor, fogFac);

		//lerp color to only depth value
		fCol.rgb = sceneColor.rgb;
		fCol.a = lerp (lerp((half)0.0, (half)1.0, _FogColor.a), (half)0.0, fogFac);

		return fCol;
	}

ENDCG

SubShader
{
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	// 0: distance + height
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, true, true); }
		ENDCG
	}
	// 1: distance
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, true, false); }
		ENDCG
	}
	// 2: height
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, false, true); }
		ENDCG
	}
}

Fallback off

}
