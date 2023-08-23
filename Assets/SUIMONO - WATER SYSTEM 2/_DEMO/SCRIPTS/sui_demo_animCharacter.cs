using UnityEngine;
using System.Collections;



public class sui_demo_animCharacter : MonoBehaviour {


	//PUBLIC VARIABLES
	public bool isWalking = false;
	public bool isRunning = false;
	public bool isSprinting = false;
	public bool isInWater = false;
	public bool isInWaterDeep = false;
	public bool isUnderWater = false;
	public bool isAtSurface = false;
	public bool isFloating = false;
	public bool isFalling = false;
	public bool isInBoat = false;
	public float moveSideways = 0.0f;
	public float moveForward = 0.0f;
	public float moveVertical = 0.0f;
	public float wetAmount = 0.0f;
	public float gSlope = 0.0f;
	public float useSlope = 0.0f;


	//PRIVATE VARIABLES
	private GameObject cameraObject;
	private Rigidbody physRigidbody;
	private Animation physAnimation;
	private string currClip;
	private string useClip;
	private string defaultClip;
	private float fadeSpeed = 0.0f;
	private float playSpeed = 1.0f;
	private float animTime = 0.0f;
	private float blinkTime = 0.0f;
	private bool doBlink = false;
	private float eyelidTime = 0.0f;
	private float randBlinkNum = 2.0f;
	private float eyeRand;
	private float headRand;
	private float headTgt;
	private float headTime = 0.0f;
	//private bool doHeadAmb = false;
	private float randHeadNum = 4.0f;
	private float randHeadSpd = 4.0f;
	//private float lastYPos = 0.0f;

	//bone objects
	private Transform boneRoot;
	private Transform boneLEye;
	private Transform boneREye;
	private Transform boneLEyelid;
	private Transform boneREyelid;
	//private Transform boneRHand;
	private Transform boneHead;
	private Transform boneNeck;
	//private Transform boneRFoot;
	//private Transform boneLFoot;



	void Start () {

		//log components
		physRigidbody = this.GetComponent<Rigidbody>();
		physAnimation = this.GetComponent<Animation>();
		
		//start animations
		useClip = "anim_miho_idle_normal";
		defaultClip = useClip;
		
		//set important bones
		SetBoneTransforms();
		
	}







	void LateUpdate () {


		//UNDERWATER status get's sent to this
		//component by the camera controller component.
		if (!isInWater){
			wetAmount -= (Time.deltaTime * 0.05f);
			wetAmount = Mathf.Clamp(wetAmount,0.0f,1.0f);
		} else {
			wetAmount = 1.0f;
		}
		
		
		//set animation clips
		useClip = defaultClip;
		playSpeed = 1.0f;
		
		if (!isInBoat){
		
			useClip = "anim_miho_idle_normal";
			fadeSpeed = 1.2f;
			playSpeed = 1.0f;

			if (isWalking){
				useClip = "anim_miho_walk_normal";
				fadeSpeed = 0.5f;
				playSpeed = 1.1f;
				if (moveForward != 0.0f && moveSideways != 0.0f){
					fadeSpeed = 0.5f;
					playSpeed = 1.1f;
				}
			}
			
			if (isRunning){
				useClip = "anim_miho_run_normal";
				fadeSpeed = 0.8f;
				playSpeed = 0.9f;
				if (moveForward != 0.0f && moveSideways != 0.0f){
					fadeSpeed = 0.8f;
					playSpeed = 0.9f;
				}
			}

			if (isSprinting){
				useClip = "anim_miho_sprint_normal";
				fadeSpeed = 1.3f;
				playSpeed = 1.1f;
				if (moveForward != 0.0f && moveSideways != 0.0f){
					fadeSpeed = 0.3f;
					playSpeed = 1.1f;
				}
			}
			
			if (isInWater){
				wetAmount = 1.0f;
			}
			
			if (isInWaterDeep){
				wetAmount = 1.0f;
				if (isWalking){
					useClip = "anim_miho_walk_water";
					fadeSpeed = 0.8f;
					playSpeed = 0.8f;
				}
			}	
			
			if (isUnderWater){
				wetAmount = 1.0f;
				useClip = "anim_miho_swim_idle";
				fadeSpeed = 1.2f;
				playSpeed = 1.0f;
				if (isWalking || isRunning){
					useClip = "anim_miho_swim_forward";
					fadeSpeed = 1.8f;
					playSpeed = 1.0f;
					if (isRunning) playSpeed = 1.4f;
				}
				if (physRigidbody != null) physRigidbody.useGravity = false;
				if (physRigidbody != null) physRigidbody.Sleep();
			}


			if (isAtSurface){
				useClip = "anim_miho_swim_surface_idle";
				fadeSpeed = 0.8f;
				playSpeed = 1.0f;
				if (physRigidbody != null) physRigidbody.useGravity = true;
			}
		
		
		} else if (isInBoat){
			useClip = "anim_miho_boat_sit_idle";
			fadeSpeed = 0.4f;
			playSpeed = 1.0f;
		}	
		
		//useClip = "anim_miho_swim_idle";
		animTime += Time.deltaTime;
		
		
		//play animations

			//normalize animation clips
			if (physAnimation[useClip] != null && physAnimation[currClip] != null){
				physAnimation[useClip].time = physAnimation[currClip].time;
			}
			
			//set new clip
			currClip = useClip;
			animTime = 0.0f;
			if (physAnimation[currClip] != null){
				physAnimation.CrossFade(currClip,fadeSpeed);
				physAnimation[currClip].speed = playSpeed;


				//ANIMATION BLENDS

				//cross fade slope walk
				if (gSlope > 0.0f){
					if (useSlope > 15.0f && useSlope < 90.0f && (isWalking || isRunning || isSprinting)){
						physAnimation.Blend("anim_miho_walk_water",((useSlope / 90.0f))*2.0f,0.1f);
					}
				}

				//falling animation
				if (isFalling){
					physAnimation.Blend("anim_miho_fall_normal",1.0f,0.1f);
				}
					

			} else {
				Debug.Log("animation "+currClip+" cannot be found!");
			}




		//BLINK
		//check for blinking eyes
		if (!doBlink){
			blinkTime += Time.smoothDeltaTime;
			if (blinkTime > randBlinkNum){
				blinkTime=0.0f;
				randBlinkNum = Random.Range(2.0f,4.0f);
				doBlink = true;
			}
		}


		//HEAD AMBIENT
		//check for blinking eyes
		headTime += Time.smoothDeltaTime;
		if (headTime > randHeadNum){
			headTime=0.0f;
			float checkHeadMove = Random.Range(0.0f,5.0f);
			if (checkHeadMove > 0.3f){
				headTgt = 0.0f;
			} else {
				headTgt = Random.Range(-80.0f,80.0f);
			}
			randHeadNum = Random.Range(2.0f,7.0f);
			randHeadSpd = Random.Range(1.0f,5.0f);
			
			
			//doHeadAmb = true;
		}
		
		if (isRunning || isSprinting){
			headTgt = 0.0f;
			randHeadSpd = 5.0f;
		}
		
		headRand = Mathf.SmoothStep(headRand, headTgt, Time.deltaTime*randHeadSpd);
		eyeRand = Mathf.SmoothStep(eyeRand, (headTgt*0.75f), Time.deltaTime*(randHeadSpd*2.0f));
		if (eyeRand >= 35.0f) eyeRand = 35.0f;
		if (eyeRand <= -35.0f) eyeRand = -35.0f;


	//}





	//function LateUpdate(){

		//----------------------------
		// PROCEDURAL ANIMATIONS
		//----------------------------
		
		//blink
		if (doBlink){
		float eyeAnimTime = 0.5f;
			eyelidTime += Time.deltaTime; 
			if (eyelidTime <= eyeAnimTime){

				boneLEyelid.transform.localEulerAngles = new Vector3(
					boneLEyelid.transform.localEulerAngles.x,
					boneLEyelid.transform.localEulerAngles.y,
					Mathf.SmoothStep(265.0f,295.0f,eyelidTime*5.0f)
					);

				boneREyelid.transform.localEulerAngles = new Vector3(
					boneREyelid.transform.localEulerAngles.x,
					boneREyelid.transform.localEulerAngles.y,
					Mathf.SmoothStep(265.0f,295.0f,eyelidTime*5.0f)
					);

			}
			if (eyelidTime > eyeAnimTime){
				eyelidTime = 0.0f;
				doBlink = false;
			}
		} else {
			boneLEyelid.transform.localEulerAngles = new Vector3(
				boneLEyelid.transform.localEulerAngles.x,
				boneLEyelid.transform.localEulerAngles.y,
				295.0f
				);
			boneREyelid.transform.localEulerAngles = new Vector3(
				boneREyelid.transform.localEulerAngles.x,
				boneREyelid.transform.localEulerAngles.y,
				295.0f
				);
		}



		//head ambient movement
		boneHead.transform.localEulerAngles = new Vector3(headRand,boneHead.transform.localEulerAngles.y,boneHead.transform.localEulerAngles.z);
		boneNeck.transform.localEulerAngles = new Vector3((headRand * 0.5f),boneNeck.transform.localEulerAngles.y,boneNeck.transform.localEulerAngles.z);
		boneLEye.transform.localEulerAngles = new Vector3(eyeRand,boneLEye.transform.localEulerAngles.y,boneLEye.transform.localEulerAngles.z);
		boneREye.transform.localEulerAngles = new Vector3(eyeRand,boneREye.transform.localEulerAngles.y,boneREye.transform.localEulerAngles.z);
		

	}



	void resetPos(){
		float saveY = this.transform.position.y;
		this.transform.position = new Vector3(boneRoot.transform.position.x, saveY, boneRoot.transform.position.z);
	}




	void SetBoneTransforms(){

		// Storing reference to some specific bone
		// objects so we can use them later in the code.
		boneRoot = transform.Find("Bip01");
		boneNeck = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Spine3/Bip01 Neck");
		boneHead = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Spine3/Bip01 Neck/Bip01 Head");
		boneLEye = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Spine3/Bip01 Neck/Bip01 Head/Bip01 EyeLeft");
		boneREye = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Spine3/Bip01 Neck/Bip01 Head/Bip01 EyeRight");
		boneLEyelid = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Spine3/Bip01 Neck/Bip01 Head/Bip01 EyeLidLeft");
		boneREyelid = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Spine3/Bip01 Neck/Bip01 Head/Bip01 EyeLidRight");
		//boneRHand = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Spine3/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand");

		//boneRFoot = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 R Thigh/Bip01 R Calf/Bip01 R Foot");
		//boneLFoot = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 L Thigh/Bip01 L Calf/Bip01 L Foot");
		
		//head beginning position
		boneHead.transform.localEulerAngles = new Vector3(
			headRand,
			boneHead.transform.localEulerAngles.y,
			boneHead.transform.localEulerAngles.z
			);

	}


}