//
//  This Utility will change the far culling of your scene camera based on the height elevation of the camera.
// 	This is useful  to expand the viewable game area as the camera gets higher (where you most need longer view distances.)
//


using UnityEngine;
using System.Collections;

public class utility_AdjustCulling : MonoBehaviour {

	public float nearCullAtBase = 0.3f;
	public float nearCullAtAltitude = 5.0f;
	public float altitudeLowerThreshold = 50f;
	public float altitudeUpperThreshold = 250f;

	private Camera cam;
	private float useThreshold;

	void Start () {
		cam = this.gameObject.GetComponent<Camera>() as Camera;
	}
	
	void LateUpdate () {
		if (cam != null){
			if (transform.position.y > altitudeLowerThreshold){
				useThreshold = Mathf.Clamp01(
					(transform.position.y-altitudeLowerThreshold) / (altitudeUpperThreshold-altitudeLowerThreshold)
					);
			} else {
				useThreshold = 0f;
			}
			cam.nearClipPlane = Mathf.Lerp(nearCullAtBase, nearCullAtAltitude, useThreshold);
		}
	}
}
