Shader "Suimono2/Demo/Flora_SingleSided" {
	Properties {
		_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_SpecTex ("Specular", 2D) = "white" {}
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_WaveAndDistance ("Wave, distance and speed", Vector) = (12, 3.6, 1, 1)
		_Cutoff ("Cutoff", float) = 0.5
		_Cutoff2 ("Cutoff2", float) = 0.5
	}
	 
	SubShader {
		Tags {
			//"Queue" = "AlphaTest"
			"Queue" = "Geometry"
			//"IgnoreProjector"="True"
			"RenderType"="TransparentCutout"
		}
		Cull Back
		//LOD 200
		//ColorMask RGB
		 
		CGPROGRAM
		#pragma surface surf Standard addshadow alphatest:_Cutoff2
		//#pragma exclude_renderers flash
		//#include "TerrainEngine.cginc"
		 
		sampler2D _MainTex;
		sampler2D _SpecTex;
		sampler2D _BumpMap;
		half _Shininess;
		fixed _Cutoff;
		fixed trunkMask;
		 
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			fixed4 color : COLOR;
		};

inline fixed4 LightingBlinnPhongTree (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	//half3 h = normalize (lightDir + viewDir);
	
	//fixed diff = max (0, dot (s.Normal, lightDir));
	
	//float nh = max (0, dot (s.Normal, h));
	//float spec = pow (nh, s.Specular*128.0) * s.Gloss;
	
		half NdotL = max(0,dot(lightDir,s.Normal));
	
	fixed4 c;
	//c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
	//c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
	
		c.rgb = s.Albedo * _LightColor0.rgb * lerp(1.0,4.0,NdotL);// * (atten);
		c.rgb = lerp(c.rgb,c.rgb*0.8*lerp(-1.0,1.0,NdotL),trunkMask);
		c.rgb = saturate(c.rgb);
		c.a = s.Alpha;
		
	return c;
}

inline fixed4 LightingBlinnPhongTree_PrePass (SurfaceOutput s, half4 light)
{
	fixed spec = light.a * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec);
	c.a = s.Alpha + spec * _SpecColor.a;
	c.a = s.Alpha;
	return c;
}

		inline fixed4 LightingTreeLight (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
		{
			//calculate final color
			fixed4 c;
			half NdotL = max(0,dot(lightDir,s.Normal));
			c.rgb = s.Albedo * _LightColor0.rgb * lerp(1.0,4.0,NdotL);// * (atten);
			c.rgb = lerp(c.rgb,c.rgb*0.8*lerp(-1.0,1.0,NdotL),trunkMask);
			c.rgb = saturate(c.rgb);
			c.a = 1.0;
			return c;
		}
	 	inline fixed4 LightingTreeLight_Prepass (SurfaceOutput s, half4 light){
			//calculate final color
			fixed4 c;
			//half NdotL = max(0,dot(lightDir,s.Normal));
			c.rgb = s.Albedo * light.rgb;// * lerp(1.0,4.0,NdotL);// * (atten);
			c.rgb = lerp(c.rgb,c.rgb*0.8,trunkMask);
			c.rgb = saturate(c.rgb);
			c.a = 1.0;
			return c;
		}
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 s = tex2D(_SpecTex, IN.uv_MainTex);
			o.Albedo = c.rgb*2.0;// * IN.color;
			o.Alpha = c.a;
			clip (o.Alpha - _Cutoff);
			//o.Gloss = s.r;
			trunkMask = s.g;
			//o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Metallic = 0.0;
			o.Smoothness = 0.2;
		}
		ENDCG
	}
 
 

	 
	Fallback Off
}