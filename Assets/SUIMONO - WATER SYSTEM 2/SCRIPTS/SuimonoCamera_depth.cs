
using UnityEngine;
using System.Collections;


namespace Suimono.Core
{
	[ExecuteInEditMode]
	public class SuimonoCamera_depth : MonoBehaviour {

		//PUBLIC VARIABLES
		[HideInInspector] public float _sceneDepth = 20.0f;
		[HideInInspector] public float _shoreDepth = 45.0f;

		//PRIVATE VARIABLES
		private Material useMat;

		void Start () {
			//setup material
			useMat = new Material(Shader.Find("Suimono2/SuimonoDepth"));
		}
		

		void LateUpdate () {
			
			//clamp values
			_sceneDepth = Mathf.Clamp(_sceneDepth,0.0f,100.0f);
			_shoreDepth = Mathf.Clamp(_shoreDepth,0.0f,100.0f);

			//set material properties
			useMat.SetFloat("_sceneDepth", _sceneDepth);
			useMat.SetFloat("_shoreDepth", _shoreDepth);
		}


		void OnRenderImage (RenderTexture source, RenderTexture destination){
			Graphics.Blit(source,destination,useMat);
		}

	}
}



