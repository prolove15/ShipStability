Shader "Suimono2/particle_AlphaOverlay" {

Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
	Tags { "Queue"="Overlay+13" "IgnoreProjector"="True"}
	//Tags { "Queue"="Transparent-102" "IgnoreProjector"="True"}
	Blend SrcAlpha OneMinusSrcAlpha
	//AlphaTest Greater .01
	//ColorMask RGB
	//ZTest LEqual
	Cull Back Lighting Off ZWrite Off Fog{Mode Off}
	//BindChannels {
	//	Bind "Color", color
	//	Bind "Vertex", vertex
	//	Bind "TexCoord", texcoord
	//}
	

	SubShader {

		CGPROGRAM
		#pragma surface surf SuimonoNoLight vertex:vert alpha
		sampler2D _MainTex;
		float4 _TintColor;


	void vert (inout appdata_full v){
		//UNITY_INITIALIZE_OUTPUT(Input,o);
		//o.customColor = abs(v.normal);
		v.vertex = v.vertex;
	}
			
			
	struct Input {
		float2 uv_MainTex;
	};

				
	fixed4 LightingSuimonoNoLight (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
	{
		fixed4 col;
		col.rgb = (s.Albedo+s.Alpha)*_LightColor0.rgb*2.0*atten;
		col.a = lerp((fixed)0.0, (fixed)2.0, s.Alpha);
		col.a = saturate(col.a);
		
		return col;
	}

	void surf (Input IN, inout SurfaceOutput o) {
		half4 col = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = col.rgb * _TintColor.rgb;
		o.Alpha = col.a;
	}
			
			
			
	ENDCG 
			
	} 	
	
	
	
	
	
}
}
