using UnityEngine;
using System.Collections;



namespace Suimono.Core
{

	public enum suiCausToolType {
		aboveWater, belowWater
	};


	[ExecuteInEditMode]
	public class cameraCausticsHandler : MonoBehaviour {

		public bool isUnderwater = false;
		public Light causticLight;
		public suiCausToolType causticType;

		private bool enableCaustics = true;
		private Suimono.Core.SuimonoModule moduleObject;



		void Start () {
			//get master object
			moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));

			//get caustic light object
			if (moduleObject != null){
				causticLight = moduleObject.suimonoModuleLibrary.causticObjectLight;
			}
		}
		


		void LateUpdate () {
			//turn off caustic light when not playing scene
			if (!Application.isPlaying) causticLight.enabled = false;
		}



		void OnPreCull() {
			if (causticLight != null){

				//enable caustics lighting
				if (moduleObject != null){
					enableCaustics = moduleObject.enableCaustics;

					if (moduleObject.setLight != null){
						if (!moduleObject.setLight.enabled || !moduleObject.setLight.gameObject.activeSelf){
							enableCaustics = false;
						}
					}

				}
				
				//handle light emission
				if (causticType == suiCausToolType.aboveWater){
					causticLight.enabled = false;
				} else if (causticType == suiCausToolType.belowWater){
					causticLight.enabled = enableCaustics;

				} else {
					causticLight.enabled = false;
				}

				if (isUnderwater) causticLight.enabled = false;
				if (!Application.isPlaying) causticLight.enabled = false;
			}
		}


		void OnPostRender() {
			if (causticLight != null){

				if (isUnderwater){
					causticLight.enabled = true;
				} else {
					causticLight.enabled = false;
				}

				if (!Application.isPlaying) causticLight.enabled = false;
			}
		}



	}
}