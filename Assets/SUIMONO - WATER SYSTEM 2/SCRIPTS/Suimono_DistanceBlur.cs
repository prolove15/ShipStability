using UnityEngine;
using System.Collections;


namespace Suimono.Core
{

	public class Suimono_DistanceBlur : MonoBehaviour {

		//public variables
		public float blurAmt = 0.0f;
		public int iterations = 3;
		public float blurSpread = 0.6f;
		public Shader blurShader = null;
		public Material material = null;

		//private variables
		private float offc;
		private float off;
		private int rtW;
		private int rtH;
		private int i;
		private RenderTexture buffer;
		private RenderTexture buffer2;


        [Range(0, 2)]
        public int downsample = 1;

        [Range(0.0f, 10.0f)]
        public float blurSize = 3.0f;



        void Start () {
            //get material reference
            CreateMaterial();
        }


		void CreateMaterial() {
			//get material reference
		    if (material == null) {
		    	material = new Material(blurShader);
		        material.hideFlags = HideFlags.DontSave;
		    }
		}
		

        public void OnDisable () {
            if (material)
                DestroyImmediate (material);
        }



		// Called by the camera to apply the image effect
		void OnRenderImage (RenderTexture source, RenderTexture destination) {

            if (material ==null) CreateMaterial();
            
			iterations = Mathf.FloorToInt(Mathf.Lerp(0,2,blurAmt));
		    downsample = Mathf.FloorToInt(Mathf.Lerp(0,2,blurAmt));
		    blurSpread = Mathf.Lerp(0.0f,2.0f,blurAmt);


            float widthMod = 1.0f / (1.0f * (1<<downsample));

            material.SetVector ("_Parameter", new Vector4 (blurSpread * widthMod, -blurSpread * widthMod, 0.0f, 0.0f));
            source.filterMode = FilterMode.Bilinear;

            int rtW = source.width >> downsample;
            int rtH = source.height >> downsample;

            // downsample
            RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);

            rt.filterMode = FilterMode.Bilinear;
            Graphics.Blit (source, rt, material, 0);

            var passOffs = 0;

            for(int i = 0; i < iterations; i++) {
                float iterationOffs = (i*1.0f);
                material.SetVector ("_Parameter", new Vector4 (blurAmt * widthMod + iterationOffs, -blurAmt * widthMod - iterationOffs, 0.0f, 0.0f));

                // vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit (rt, rt2, material, 1 + passOffs);
                RenderTexture.ReleaseTemporary (rt);
                rt = rt2;

                // horizontal blur
                rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit (rt, rt2, material, 2 + passOffs);
                RenderTexture.ReleaseTemporary (rt);
                rt = rt2;
            }

            Graphics.Blit (rt, destination);
            RenderTexture.ReleaseTemporary (rt);


		}




	}
}
