Shader "Suimono2/Suimono2_FX_ShorelineObject" {

    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader {
        Pass {
            Tags{"Queue"="Overlay"}
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;
            float4 _Mult;

            fixed4 frag(v2f_img i) : SV_Target {
                return tex2D(_MainTex, i.uv*_Mult.xy+_Mult.zw);
            }
            ENDCG

        }
    }
}
