using UnityEngine;
using System.Collections;



public class sui_demo_LightHandler : MonoBehaviour {


	public float dayIntensity = 1.0f;
	public float nightIntensity = 0.01f;
	public float sunsetDegrees = 20.0f;
	public float lightDegrees = 10.0f;
	public Color dayColor = new Color(1.0f,0.968f,0.933f,1.0f);
	public Color sunsetColor = new Color(0.77f,0.33f,0.0f,1.0f);

	private Light lightObject;
	private float lightFac;
	private float sunsetFac;


	void Start () {
		lightObject = gameObject.GetComponent<Light>() as Light;
	}


	void LateUpdate () {

		if (lightObject != null){

			//clamp values
			sunsetDegrees = Mathf.Clamp(sunsetDegrees,0.0f,90.0f);

			//find the light factor based on the rotation of the light
			lightFac = transform.eulerAngles.x;
			if (lightFac > 90.0f) lightFac = 0.0f;
			sunsetFac = Mathf.Clamp01(lightFac / sunsetDegrees);
			lightFac = Mathf.Clamp01(lightFac / lightDegrees);

			//set the light intensity
			lightObject.intensity = Mathf.Lerp(nightIntensity,dayIntensity,lightFac);

			//clamp the intensity just in case (having a 0.0 intensity can cause un-anticipated lighting problems in Unity)
			if (lightObject.intensity < 0.01f) lightObject.intensity = 0.01f;

			//modulate the light color
			lightObject.color = Color.Lerp(sunsetColor,dayColor,sunsetFac);

		}
	}


}