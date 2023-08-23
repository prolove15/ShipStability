using UnityEngine;
using System.Collections;


namespace Suimono.Core
{
	public class SuimonoDepth : MonoBehaviour {


		//PUBLIC VARIABLES
		public Shader useShader;

		//PRIVATE VARIABLES
		private Material useMat;

		void Start () {
			//setup material
			useMat = new Material(useShader);
		}
		
		void OnRenderImage (RenderTexture source, RenderTexture destination){
			if (useMat != null) Graphics.Blit(source,destination,useMat);
		}

	}
}