using UnityEngine;
using System.Collections;


namespace Suimono.Core
{
	public class fx_causticObject : MonoBehaviour {

		public bool manualPlacement = false;

		private Suimono.Core.SuimonoModule moduleObject;
		private Suimono.Core.fx_causticModule causticObject;
		private Light lightComponent;
		private float heightMult = 1f;

		void Start () {
			//get master objects
			moduleObject = GameObject.Find("SUIMONO_Module").GetComponent<Suimono.Core.SuimonoModule>();
			causticObject = GameObject.Find("_caustic_effects").GetComponent<Suimono.Core.fx_causticModule>();
			lightComponent = GetComponent<Light>();
		}
		

		void LateUpdate () {
		

			if (causticObject.enableCaustics){

				//get the current light texture from Module
				lightComponent.cookie = causticObject.useTex;
				lightComponent.cullingMask = moduleObject.causticLayer;
				lightComponent.color = causticObject.causticTint;

				//Set Height (manual objects only)
				heightMult = 1f;
				if (manualPlacement) heightMult = (1f-causticObject.heightFac);
				lightComponent.intensity = causticObject.causticIntensity * heightMult;

				lightComponent.cookieSize = causticObject.causticScale;

				//get scene lighting
				if (causticObject.sceneLightObject != null ){

					//set caustic color based on scene lighting
					if (causticObject.inheritLightColor){
						lightComponent.color = causticObject.sceneLightObject.color * causticObject.causticTint;
						lightComponent.intensity = lightComponent.intensity * causticObject.sceneLightObject.intensity;
					} else {
						lightComponent.color = causticObject.causticTint;
					}

					//set caustic direction based on scene light direction
					if (causticObject.inheritLightDirection){
						transform.eulerAngles = causticObject.sceneLightObject.transform.eulerAngles;
					} else {
						transform.eulerAngles = new Vector3(90f,0f,0f);
					}

				}
			}


		}
	}
}