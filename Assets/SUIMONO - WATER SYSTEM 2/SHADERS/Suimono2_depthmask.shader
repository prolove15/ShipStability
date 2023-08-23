Shader "Suimono2/DepthMask" {
 
	SubShader {

		// Render the mask after regular geometry, but before masked geometry and
		// transparent things.
 		Tags {"Queue" = "Geometry+10"}
 
 		//Stencil{
		//    Ref 1
		 //   Comp Always
		 //   Pass replace
		//}

		// Don't draw in the RGBA channels; just the depth buffer
		ColorMask 0
		ZWrite On
		//ZTest Greater

		// Do nothing specific in the pass: 
		Pass {}
	}
}