using UnityEngine;
using System.Collections;


namespace Suimono.Core
{

	public class fx_causticModule : MonoBehaviour {

		//PUBLIC VARIABLES
		public bool enableCaustics = true;
		public Light sceneLightObject;
		public bool inheritLightColor = false;
		public bool inheritLightDirection = false;
		public Color causticTint = new Color(1f,1f,1f,1f);
		public float causticIntensity = 2f;
		public float causticScale = 4f;
		public float heightFac = 0f;
		public int causticFPS = 32;
		public Texture2D[] causticFrames;
		public Texture2D useTex;

		//PRIVATE VARIABLES
		private float causticsTime = 0f;
		private Suimono.Core.SuimonoModule moduleObject;
		private GameObject lightObject;
		private int frameIndex = 0;



		void Start () {
			//get master objects
			moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
			lightObject = transform.Find("mainCausticObject").gameObject;
		}
		

		void LateUpdate () {
		
			if (this.enabled){
	
		  		useTex = causticFrames[frameIndex];
		    	causticsTime += Time.deltaTime;
		    	if (causticsTime > (1f/(causticFPS*1f))){
		    		causticsTime = 0f;
		    		frameIndex += 1;
		    	}

		    	if (frameIndex == causticFrames.Length) frameIndex = 0;

		    	if (moduleObject != null){
		    		if (moduleObject.setLight != null){
		    			sceneLightObject = moduleObject.setLight;
		    		}

		    		if (lightObject != null){
		    			lightObject.SetActive(moduleObject.enableCaustics);
		    		}
		    	}
		    }
		}

	}
}