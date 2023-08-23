// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suimono2/EffectWakeMask" {


Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent+10" "IgnoreProjector"="True" "RenderType"="SuimonoWakeMask"}

	ZWrite Off
	Blend One One
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0,0,0,0);
				fixed4 tex = tex2D(_MainTex, i.texcoord);

				col.r = lerp((fixed)0.0,(fixed)tex.r*(fixed)_Color.r,i.color.r);
				col.g = lerp((fixed)0.0,(fixed)tex.g*(fixed)_Color.g,i.color.g);
				col.b = lerp((fixed)0.0,(fixed)tex.b*(fixed)_Color.b,i.color.b);
				col.a = tex.a * _Color.a * i.color.a;

				return col;
			}
		ENDCG
	}
}

}
