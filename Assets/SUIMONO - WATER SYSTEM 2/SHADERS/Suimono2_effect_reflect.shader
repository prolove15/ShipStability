// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suimono2/effect_reflect" {


Properties {

	_DynReflColor ("Reflection Dynamic", Color) = (1.0, 1.0, 1.0, 0.5)
	_ReflDist ("Reflection Distance", Float) = 1000
	_ReflBlend ("Reflection Blend", Range(0.003,0.5)) = 0.01
	_ReflectionTex ("Reflection", 2D) = "" {}

}


Subshader 
{ 


// ------------------------
//    MIRROR REFLECTION
//-------------------------
Tags { "RenderType"="Transparent" "Queue"= "Transparent-301"}
Cull Back
Blend SrcAlpha OneMinusSrcAlpha
ZWrite Off
ZTest Never
ColorMask Off

CGPROGRAM
#pragma target 3.0
#pragma surface surf Lambert vertex:vert noambient alpha



struct Input {
    float4 pos;
};
     

void vert (inout appdata_full v, out Input o) {
	o.pos = -UnityObjectToClipPos (v.vertex);   
}


sampler2D _ReflectionTex;

void surf (Input IN, inout SurfaceOutput o) {

    o.Gloss = 0.0;
    o.Specular = 0.0;
    o.Emission = 0.0;
    o.Albedo = half3(0,0,0);
    o.Alpha = 0.0;
}
ENDCG



}
FallBack ""
}



































