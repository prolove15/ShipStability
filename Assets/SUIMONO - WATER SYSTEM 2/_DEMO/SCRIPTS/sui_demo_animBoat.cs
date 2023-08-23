using UnityEngine;
using System.Collections;



public class sui_demo_animBoat : MonoBehaviour {


	public GameObject propObject;
	public GameObject rudderObject;
	public float propellerSpeed = 0.0f;
	public float engineRotation = 0.0f;
	public Transform playerPosition;
	public Transform playerExit;
	public AudioClip audioEngineStart;
	public AudioClip audioEngineStop;
	public AudioClip audioEngineIdle;
	public AudioClip audioEngineRev;
	public AudioClip audioEngineRevHigh;
	public AudioClip audioEngineRevAbove;
	public bool behaviorIsOn = false;
	public bool behaviorIsInWater = false;
	public bool behaviorIsRevving = false;
	public bool behaviorIsRevvingBack = false;
	public bool behaviorIsRevvingHigh = false;

	private AudioSource audioObjectA;
	private AudioSource audioObjectB;
	private AudioClip useClip;
	private AudioClip currentClip;
	private float engineRot = 90.0f;
	private bool isOn = false;
	private float onTime = 0.0f;
	private float propSpd = 0.0f;



	void Awake () {
		
		//create audio objects
		GameObject audioObja = new GameObject();
		audioObja.name = "BoatAudioObjectA";
		audioObja.AddComponent<AudioSource>();
		audioObja.transform.position = this.transform.position;
		audioObja.transform.parent = this.transform;
		audioObjectA = audioObja.GetComponent<AudioSource>() as AudioSource;

		GameObject audioObjb = new GameObject();
		audioObjb.name = "BoatAudioObjectB";
		audioObjb.AddComponent<AudioSource>();
		audioObjb.transform.position = this.transform.position;
		audioObjb.transform.parent = this.transform;
		audioObjectB = audioObjb.GetComponent<AudioSource>() as AudioSource;
	}



	void LateUpdate () {
			
			//Handle Rudder Rotation
			if (rudderObject != null){
				if (engineRotation == 0.0f){
					engineRot = Mathf.Lerp(engineRot,90.0f,Time.deltaTime*2.5f);
				} else {
					engineRot = Mathf.Lerp(engineRot,(90.0f-(60.0f*engineRotation)),Time.deltaTime);
				}
				rudderObject.transform.localEulerAngles = new Vector3(
					rudderObject.transform.localEulerAngles.x,
					engineRot,
					rudderObject.transform.localEulerAngles.z
					);
			}
			
		
			//Handle Propeller Rotation
			if (propObject != null){
				propSpd = 0.0f;
				if (behaviorIsOn){
					propSpd = 200.0f;
					if (behaviorIsRevving) propSpd = 1200.0f;
					if (behaviorIsRevvingHigh) propSpd = 3000.0f;
					if (behaviorIsRevvingBack) propSpd = -800.0f;
				}
				propellerSpeed = Mathf.Lerp(propellerSpeed,propSpd,Time.deltaTime);
				propObject.transform.localEulerAngles = new Vector3(
					propObject.transform.localEulerAngles.x,
					propObject.transform.localEulerAngles.y,
					(propObject.transform.localEulerAngles.z + Time.deltaTime * propellerSpeed)
				);
			}
			
			
			//Handle Audio
			if (audioObjectA != null && audioObjectB != null){
			
				//setup audio systems
				float fadeSpeed = 1.0f;
				audioObjectA.minDistance = 10.0f;
				audioObjectA.maxDistance = 30.0f;
				audioObjectB.minDistance = 10.0f;
				audioObjectB.maxDistance = 30.0f;
				
				//HANDLE AUDIO CLIPS
				if (behaviorIsOn){
					
					//Select Clips based on behavior
					audioObjectA.loop = true;
					audioObjectB.loop = true;
					
					if (isOn){
						
						useClip = audioEngineIdle;
						
						if (behaviorIsRevving){
							
							fadeSpeed = 10.0f;
							if (currentClip == audioEngineRevAbove) fadeSpeed = 10.0f;
							if (currentClip == audioEngineRevHigh) fadeSpeed = 10.0f;
							useClip = audioEngineRev;
							
							if (behaviorIsRevvingHigh){
								fadeSpeed = 10.0f;
								useClip = audioEngineRevHigh;
							}
						}
					}
					if (!isOn){
						//handle turn on sequence
						onTime += Time.deltaTime;
						if (onTime >= 1.0f) isOn = true;
						fadeSpeed = 10.0f;
						useClip = audioEngineStart;
					}
					
				} else {
				
					//handle turn off sequence
					audioObjectA.loop = false;
					audioObjectB.loop = false;
					if (isOn){
						
						onTime -= Time.deltaTime;
						if (onTime <= -0.5f) isOn = false;
						
						fadeSpeed = 10.0f;
						useClip = audioEngineStop;
					} else {
						onTime = 0.0f;
						isOn = false;
						if (audioObjectA.isPlaying) audioObjectA.Stop();
						if (audioObjectB.isPlaying) audioObjectB.Stop();
					}
				}

				//switch clips clip
				if (currentClip != useClip){
					audioObjectA.Stop();
					audioObjectA.clip = useClip;
					audioObjectA.volume = 0.0f;
					audioObjectB.Stop();
					audioObjectB.clip = currentClip;
					audioObjectB.volume = 1.0f;
					currentClip = useClip;
				}
				
				//fade clips
				audioObjectA.volume = Mathf.Lerp(audioObjectA.volume,1.0f,Time.deltaTime * fadeSpeed);
				audioObjectB.volume = Mathf.Lerp(audioObjectB.volume,0.0f,Time.deltaTime * fadeSpeed);
				
				//play clips
				if (behaviorIsOn || isOn){
					if (!audioObjectA.isPlaying) audioObjectA.Play();
					if (!audioObjectB.isPlaying) audioObjectB.Play();
				}
			}
	}



}