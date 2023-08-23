// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suimono2/effect_splash" {
Properties {
	_MaskMap ("MaskMap", 2D) = "" {}
	_HeightMap ("HeightMap", 2D) = "" {}
	_DepthColor ("Depth Color", Color) = (0.5, 0.5, 0.5, 1)
    _Strength ("Refraction Strength", range (0,150)) = 25.0
    _Overlay ("Overlay Strength", range (0.0,1.0)) = 1.0
    _Brightness ("Brightness Strength", range (1.0,1.8)) = 1.0
}



SubShader {





GrabPass {
	"_waterTex1"
	Tags {"Queue" = "Transparent+1" "IgnoreProjector"="True"}
	Name "ScreenGrab"
}


//RENDER REFRACTION
Pass{
	Tags {"Queue"= "Transparent+1" "IgnoreProjector"="True"} //Geometry
	Cull off
	Name "DropDistortion"
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
	ZWrite Off
	ZTest Always
	
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos          : POSITION;
		float4 uvgrab       : TEXCOORD0;
		float2 uv           : TEXCOORD1;
		float4 screenPos    : TEXCOORD2;
		float4 uvs          : TEXCOORD3;
		fixed4 color 		: COLOR;
	};
	
	
	
	v2f vert (appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);   
		#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
		#else
		float scale = 1.0;
		#endif

		o.color = v.color;
		
		o.screenPos = ComputeScreenPos(o.pos);
		
		o.uv = v.texcoord.xy;
		
		o.uvgrab.xy = (float2(o.pos.x, o.pos.y * scale) + o.pos.w) * 0.5;
		o.uvgrab.zw = o.pos.zw;
		
		o.uvs.xy = (float2(o.pos.x, o.pos.y * scale) + (o.pos.w)) * 0.5;
		o.uvs.z = o.pos.z;
		o.uvs.w = o.pos.w;
		
		return o;
	}
	sampler2D _CameraDepthTexture;
	sampler2D _HeightMap;
	sampler2D _MaskMap;
    float _EffectSpeed, _EffectSpeed2, _Strength;
	float4 _DepthColor;
	sampler2D _waterTex1;
	float4 _GrabTexture_TexelSize;
	float _Overlay;
	float _Brightness;
	
	
	half4 frag( v2f i ) : COLOR
	{
	

		float2 effectUVs = i.uv;
		float3 normal1 = UnpackNormal(tex2D(_HeightMap, effectUVs));

		effectUVs = i.uv;
		float3 normal2 = UnpackNormal(tex2D(_HeightMap, effectUVs));
   
		normal2 = normal2 * float3(21, 11, 0.5);
		
		//grab original background
		half4 oCol = half4(tex2Dproj(_waterTex1, UNITY_PROJ_COORD(i.uvgrab)).rgb, 1);
		
		//distort
		float3 combinedNormal = normalize(normal1 * normal2);
		//float2 offset = combinedNormal.xy * _RefrStrength * _GrabTexture_TexelSize.xy;// * _MasterScale;
		float2 offset = combinedNormal.xy * _Strength * _GrabTexture_TexelSize.xy * 1.25;  //5
		i.uvgrab.xy = (offset * i.uvgrab.z) + i.uvgrab.xy;
		half4 rCol = half4(tex2Dproj(_waterTex1, UNITY_PROJ_COORD(i.uvgrab)).rgb, 1.0);
		
                    
        half3 MTex = tex2D(_MaskMap, effectUVs);
                    
		//calculate second UVs
		float3 combinedNormal2 = normalize(normal1 * normal2);
		float2 offset2 = combinedNormal2.xy * _Strength * _GrabTexture_TexelSize.xy * 1.25;  //5
		i.uvs.xy = (offset2 * i.uvs.z) + i.uvs.xy;
		
		//Sample Depth
		half drefr = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.uvs)).r);//i.ref
		half dref = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r); //uv2

		//If Depth doesn't match, switch in refracted background
		half colMask = 1.0-((dref*10.0)-(drefr*10.0));
		//if (colMask >= 0.999)
			oCol = rCol;

		oCol.rgb = oCol.rgb * ((1.0 + (0.05 * _Overlay)) + (_DepthColor.rgb * MTex.g * 0.25));
		oCol -= (((1.0-MTex.r)*0.1)*_Overlay);
		oCol.a = MTex.r * i.color.a * _DepthColor.a;// * 1.65;//(MTex.r*1.5);
		
		if (colMask >= 0.999)
			oCol.a * 0.05;
		
		return oCol*(_Brightness);
	}

	ENDCG
}





    
        
// TOP EFFECT RENDER 
Tags {"Queue"= "Transparent+1" "IgnoreProjector"="True"}
Cull Off
Blend SrcAlpha OneMinusSrcAlpha
Lighting Off
ColorMask RGB
ZWrite Off
ZTest Always


CGPROGRAM
#pragma surface surf Lambert


struct Input {
	float2 uv_HeightMap;
};



sampler2D _HeightMap;
float4 _DepthColor;



void surf (Input IN, inout SurfaceOutput o) {

	o.Gloss = 1.0;
	o.Specular = 1.0;
	o.Albedo = half3(0.0,0.0,0.0);//_DepthColor.rgb;//float3(0.0,0.0,0.0);
	o.Alpha = 0.0;//_DepthColor.a;// * maskTex.r;//0.0;
	
}
ENDCG


      
                

}

//FallBack ""
}
