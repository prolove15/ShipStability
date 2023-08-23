Shader "Suimono2/Demo/Skin" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 2.0)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_ReflectionTex ("Reflection", CUBE) = "" {}
	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.01,1.0)) = 1.0
    _RimColor2 ("Rim Color2", Color) = (0.26,0.19,0.16,0.0)
    _RimPower2 ("Rim Power2", Range(0.1,8.0)) = 3.0
}

SubShader { 
	Tags {"RenderType"="Opaque" "Queue"= "Geometry"}
	Cull Back

	
CGPROGRAM
#pragma target 3.0
#pragma surface surf BlinnPhong


sampler2D _MainTex;
sampler2D _BumpMap;
samplerCUBE _ReflectionTex;
float4 _Color;
float _Shininess;
float4 _RimColor;
float _RimPower;
float4 _RimColor2;
float _RimPower2;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 viewDir;
	float3 worldRefl;
	//float4 pos;
	INTERNAL_DATA
};


inline fixed4 LightingBlinnPhongSkin2 (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	fixed diff = max (0, dot (s.Normal, lightDir));
	
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular*128.0) * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
	c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
	return c;
}

inline fixed4 LightingBlinnPhongSkin2_PrePass (SurfaceOutput s, half4 light)
{
	fixed spec = light.a * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec);
	c.a = s.Alpha + spec * _SpecColor.a;
	return c;
}

fixed4 LightingBlinnPhongSkin (SurfaceOutput s, fixed3 lightDir, fixed atten)
{
	fixed diff = max (0, dot (s.Normal, lightDir));
	
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
	c.a = s.Alpha;
	return c;
}




void surf (Input IN, inout SurfaceOutput o) {
	half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	half3 reflectionBase = texCUBE(_ReflectionTex, WorldReflectionVector (IN, o.Normal)).rgb; 

	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a * _Shininess;
	reflectionBase *= (_SpecColor*2.0);
	half useSpec = (tex.a * (reflectionBase)) * 2.0;
	if (useSpec <= 0.01) useSpec = 0.01;
	o.Specular = useSpec;
	
	half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
	half rim2 = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
	
	o.Albedo += (reflectionBase * tex.a * _Shininess * 0.25);
	o.Alpha = 1.0;
   	//rim 1
    //o.Emission = _RimColor.rgb * pow(rim, _RimPower) * (0.75) * (1.0-tex.a);
    //o.Emission += _RimColor.rgb * pow(rim, _RimPower) * (0.5) * (tex.a);
    
    //rim 2
    //o.Emission += _RimColor2.rgb * pow(rim2, _RimPower2) * (0.75) * (1.0-tex.a);
    //o.Emission += _RimColor2.rgb * pow(rim2, _RimPower2*0.5) * (1.75) * (tex.a);
    
    
}
ENDCG










}






Fallback Off




}
