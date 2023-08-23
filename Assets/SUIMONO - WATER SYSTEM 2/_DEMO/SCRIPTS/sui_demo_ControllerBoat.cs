using UnityEngine;
using System.Collections;



public class sui_demo_ControllerBoat : MonoBehaviour {



	//PUBLIC VARIABLES
	public bool isActive = false;
	public bool isControllable = true;
	public bool isExtraZoom = false;
	public bool keepAboveSurface = true;
	public bool handleObjectOcclusion = true;

	public Transform cameraTarget;
	public bool reverseYAxis = true;
	public bool reverseXAxis = false;
	public Vector2 mouseSensitivity = new Vector2(4.0f,4.0f);
	public float cameraFOV = 35.0f;
	public Vector2 cameraOffset = new Vector2(0.0f,0.0f);
	public float cameraLean = 0.0f;
	public float walkSpeed = 0.02f;
	public float runSpeed = 0.4f;
	public Vector3 rotationLimits = new Vector3(0.0f,0.0f,-30.0f);
	public float minZoomAmount = 1.25f;
	public float maxZoomAmount = 8.0f;
	public sui_demo_animBoat targetAnimator;


	//PRIVATE VARIABLES
	private Transform cameraObject;
	private Vector2 axisSensitivity = new Vector2(4.0f,4.0f);
	private float followDistance = 5.0f;
	private float followHeight = 1.0f;
	private float followLat = 0.0f;
	private float camFOV = 35.0f;
	private float camRotation = 0.0f;
	private Vector3 camRot;
	private float camHeight = 0.0f;
	private bool isInWater = false;
	private bool isInWaterDeep = false;
	private bool isUnderWater = false;
	//private bool isAtSurface = false;
	//private bool isFloating = false;
	//private bool isFalling = false;
	private Vector3 targetPosition;
	private float MouseRotationDistance = 0.0f;
	private float MouseVerticalDistance = 0.0f;
	private GameObject suimonoGameObject;
	private Suimono.Core.SuimonoModule suimonoModuleObject;
	private float followTgtDistance = 0.0f;
	//private bool orbitView = false;
	private Quaternion targetRotation;
	private float MouseScrollDistance = 0.0f;
	//private Transform playerObject;
	//private float projEmitTimer = 0.0f;
	//private float camVRotation = 0.0f;
	//private float firingTime = 0.0f;
	//private float sightingTime = 0.0f;
	private float setFOV = 1.0f;
	//private float targetUseLean = 0.0f;
	private float useSpeed = 0.0f;
	private float useSideSpeed = 0.0f;
	private float moveSpeed = 0.05f;
	private float moveForward = 0.0f;
	private float moveSideways = 0.0f;
	private bool isRunning = false;
	//private bool isMouseMove = false;
	//private float lastYPos = 0.0f;
	//private float propSpd = 0.0f;
	//private float engPos = 0.5f;
	//private Transform vehiclePosition;
	//private Transform vehicleExitPosition;
	//private float forwardAmt = 0.0f;
	//private float sidewaysAmt = 0.0f;
	//private float editorSensitivity = 1.0f;
	//private float button3time = 0.0f;
	private Vector3 savePos;
	private float oldMouseRotation;
	//private float oldMouseVRotation;
	private float xMove;
	private float zMove;
	private sui_demo_ControllerMaster MC;
	private sui_demo_InputController IC;
	private float waterLevel;



	void Awake() {

		//get Suimono Specific Objects
		suimonoGameObject = GameObject.Find("SUIMONO_Module");
		if (suimonoGameObject != null) suimonoModuleObject = suimonoGameObject.GetComponent<Suimono.Core.SuimonoModule>();
		
		targetPosition = cameraTarget.position;
		targetRotation = cameraTarget.rotation;
		
		if (cameraTarget != null){
			targetAnimator = cameraTarget.gameObject.GetComponent<sui_demo_animBoat>();
		}
		
		MC = this.gameObject.GetComponent<sui_demo_ControllerMaster>() as sui_demo_ControllerMaster;
		IC = this.gameObject.GetComponent<sui_demo_InputController>() as sui_demo_InputController;
	}




	void LateUpdate() {

		//clamp rotations
		if (rotationLimits.x != 0.0f){
			if (cameraTarget.transform.eulerAngles.x < 360.0f-rotationLimits.x && cameraTarget.transform.eulerAngles.x > rotationLimits.x+10f){
				cameraTarget.transform.eulerAngles = new Vector3(
					360.0f-rotationLimits.x,
					cameraTarget.transform.eulerAngles.y,
					cameraTarget.transform.eulerAngles.z
					);
			} else if (cameraTarget.transform.eulerAngles.x > rotationLimits.x && cameraTarget.transform.eulerAngles.x < 360.0f-rotationLimits.x){
				cameraTarget.transform.eulerAngles = new Vector3(
					rotationLimits.x,
					cameraTarget.transform.eulerAngles.y,
					cameraTarget.transform.eulerAngles.z
					);
			}
		}	
		if (rotationLimits.y != 0.0f){
			if (cameraTarget.transform.eulerAngles.y < 360.0f-rotationLimits.y && cameraTarget.transform.eulerAngles.y > rotationLimits.y+10f){
				cameraTarget.transform.eulerAngles = new Vector3(
					cameraTarget.transform.eulerAngles.x,
					360.0f-rotationLimits.y,
					cameraTarget.transform.eulerAngles.z
					);
			} else if (cameraTarget.transform.eulerAngles.y > rotationLimits.y && cameraTarget.transform.eulerAngles.y < 360.0f-rotationLimits.y){
				cameraTarget.transform.eulerAngles = new Vector3(
					cameraTarget.transform.eulerAngles.x,
					rotationLimits.y,
					cameraTarget.transform.eulerAngles.z
					);
			}
		}	
		if (rotationLimits.z != 0.0f){
			if (cameraTarget.transform.eulerAngles.z < 360.0f-rotationLimits.z && cameraTarget.transform.eulerAngles.z > rotationLimits.z+10f){
				cameraTarget.transform.eulerAngles = new Vector3(
					cameraTarget.transform.eulerAngles.x,
					cameraTarget.transform.eulerAngles.y,
					360.0f-rotationLimits.z
					);
			} else if (cameraTarget.transform.eulerAngles.z > rotationLimits.z && cameraTarget.transform.eulerAngles.z < 360.0f-rotationLimits.z){
				cameraTarget.transform.eulerAngles = new Vector3(
					cameraTarget.transform.eulerAngles.x,
					cameraTarget.transform.eulerAngles.y,
					rotationLimits.z
					);
			}
		}	

	}




	void FixedUpdate () {

	if (isActive){

		//------------------------------------
		//  GET DATA FROM MASTER CONTROLLER
		//------------------------------------
		cameraObject = MC.cameraObject;
		
		
		//---------------------------------
		//  GET KEYBOARD AND MOUSE INPUTS
		//---------------------------------
		
		if (isControllable){

			//"WASD" MOVEMENT KEYS
			moveForward = 0.0f;
			moveSideways = 0.0f;
			if (IC.inputKeyW) moveForward = 1.0f;
			if (IC.inputKeyS) moveForward = -1.0f;
			if (IC.inputKeyA) moveSideways = -1.0f;
			if (IC.inputKeyD) moveSideways = 1.0f;
			
			//MOUSE BUTTON 0
			//isMouseMove = IC.inputMouseKey0;
			
			//MOUSE BUTTON 1
			isExtraZoom = IC.inputMouseKey1;
			if (isExtraZoom){
				setFOV = 0.5f;
			} else {
				setFOV = 1.0f;
			}
			
			//SHIFT
			isRunning = IC.inputKeySHIFTL;
			if (moveForward == -1.0f) isRunning = false;
			
			//SPACE
			//orbitView = IC.inputKeySPACE;


		}


			//CHECK FOR MOUSE INPUT
			targetPosition = cameraTarget.position;
			oldMouseRotation = MouseRotationDistance;
			//oldMouseVRotation = MouseVerticalDistance;
			
			//GET MOUSE MOVEMENT
			MouseRotationDistance = IC.inputMouseX;
			MouseVerticalDistance = IC.inputMouseY;
			MouseScrollDistance = IC.inputMouseWheel;
			if (reverseXAxis) MouseRotationDistance = -IC.inputMouseX;
			if (reverseYAxis) MouseVerticalDistance = -IC.inputMouseY;
		
		


		//---------------------------------
		//  HANDLE CAMERA VIEWS
		//---------------------------------
		if (!isControllable){
			//Zoom Settings used for the intro screen
			camFOV = 63.2f;
			followLat = Mathf.Lerp(followLat,-0.85f,Time.deltaTime*4.0f);
			followHeight = Mathf.Lerp(followHeight,1.8f,Time.deltaTime*4.0f);
			followDistance = Mathf.Lerp(followDistance,5.0f,Time.deltaTime*4.0f);

			axisSensitivity = new Vector2(
				Mathf.Lerp(axisSensitivity.x,mouseSensitivity.x,Time.deltaTime*4.0f),
				Mathf.Lerp(axisSensitivity.y,mouseSensitivity.y,Time.deltaTime*4.0f)
				);

			cameraObject.GetComponent<Camera>().fieldOfView = camFOV;
		}
		
		//IDLE SETTINGS lerp camera back
		camFOV = Mathf.Lerp(camFOV,cameraFOV*setFOV,Time.deltaTime*4.0f);
		followLat = Mathf.Lerp(followLat,-0.4f,Time.deltaTime*4.0f);
		followHeight = Mathf.Lerp(followHeight,1.4f,Time.deltaTime*2.0f);
		axisSensitivity = new Vector2(Mathf.Lerp(axisSensitivity.x,mouseSensitivity.x,Time.deltaTime*4.0f),axisSensitivity.y);
		axisSensitivity = new Vector2(axisSensitivity.x, Mathf.Lerp(axisSensitivity.y,mouseSensitivity.y,Time.deltaTime*4.0f));

		//LOCK CURSOR
		Cursor.lockState = CursorLockMode.None;

		
		//---------------------------------
		//  SUIMONO SPECIFIC HANDLING
		//---------------------------------
		// we use this to get the current Suimono plane water level (if applicable) from the
		// main Suimono Module object, then translate this into different walk / run speeds
		// based on water depth.
		//var waterLevel : float = suimonoModuleObject.GetWaterDepth(cameraTarget);
		if (suimonoModuleObject != null){
			waterLevel = suimonoModuleObject.SuimonoGetHeight(cameraTarget.position,"object depth");

			isInWater = false;
			
			if (waterLevel < 0.0f) waterLevel = 0.0f;
			if (waterLevel > 0.0f){
		
				isInWater = true;
				isInWaterDeep = false;
				isUnderWater = false;
				//isFloating = false;
				//isAtSurface = false;
				
				if (waterLevel >= 0.9f && waterLevel < 1.8f) isInWaterDeep = true;
				if (waterLevel >= 1.8f) isUnderWater = true;
				//if (waterLevel >= 1.2f && waterLevel < 1.8f) isAtSurface = true;
				//if (isInWaterDeep && waterLevel > 2.0f) isFloating = true;

			}
		}
		

		

		//---------------------------------
		// SET MOVEMENT SPEEDS
		//---------------------------------
		float spdLerp = 5.0f;
		if (isRunning && moveForward > 0.0f) spdLerp = 2.5f;
		
		moveSpeed = walkSpeed;
		if (isInWaterDeep || isUnderWater) isRunning = false;
		if (isRunning) moveSpeed = runSpeed;
		
		if (moveForward != 0.0f && moveSideways != 0.0f) moveSpeed *= 0.75f;
		
		if (!isInWater) moveSpeed *= 0.0f;


		useSpeed = Mathf.Lerp(useSpeed, (moveSpeed * moveForward), Time.deltaTime*spdLerp);
		useSideSpeed = Mathf.Lerp(useSideSpeed, (moveSpeed * moveSideways), Time.deltaTime*spdLerp);




		//---------------------------------
		//  CHARACTER POSITIONING
		//---------------------------------
		
		if (isControllable){

			//ROTATE BOAT
			if (moveForward != 0.0f){
				xMove = Mathf.Lerp(xMove,useSpeed,Time.deltaTime);
				zMove = Mathf.Lerp(zMove,useSpeed,Time.deltaTime);
				cameraTarget.eulerAngles = new Vector3(
					cameraTarget.eulerAngles.x,
					(cameraTarget.eulerAngles.y + Mathf.Lerp(0.0f,20.0f*moveSideways*moveForward,Time.deltaTime*(Mathf.Abs(xMove*10.0f))) ),
					cameraTarget.eulerAngles.z
					);
				if (isInWater){
					cameraTarget.eulerAngles = new Vector3(
						cameraTarget.eulerAngles.x,
						cameraTarget.eulerAngles.y,
						(cameraTarget.eulerAngles.z  + Mathf.Lerp(0.0f,-130.0f*moveSideways*moveForward,Time.deltaTime*(Mathf.Abs(zMove*5.0f))))
						);
				}
			} else {
				xMove = Mathf.Lerp(xMove,0.0f,Time.deltaTime);
			}


			//MOVE BOAT
			if (cameraTarget.GetComponent<Rigidbody>()){
				//calculate forward / backward movement
				Vector3 setNewPos;
				setNewPos = ((cameraTarget.transform.forward * (xMove)));
				
				//calculate vertical while underwater
				Vector3 setNewVertPos = new Vector3(0f,0f,0f);

				//set final movement
				cameraTarget.GetComponent<Rigidbody>().MovePosition(cameraTarget.GetComponent<Rigidbody>().position + (setNewPos + setNewVertPos));
			}
			
			
			
			//---------------------------------
			//  CAMERA POSITIONING
			//---------------------------------

			//Calculate Follow Distance
			float followLerpSpeed = 2.0f;
			followDistance -= (MouseScrollDistance*8.0f);
			followDistance = Mathf.Clamp(followDistance,minZoomAmount,maxZoomAmount);
			followTgtDistance = Mathf.Lerp(followTgtDistance,followDistance,Time.deltaTime*followLerpSpeed);
			
			// Calculate Rotation
			camRotation = Mathf.Lerp(oldMouseRotation,MouseRotationDistance*axisSensitivity.x,Time.deltaTime);
			targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x,(targetRotation.eulerAngles.y + camRotation),targetRotation.eulerAngles.z);
			cameraObject.transform.eulerAngles = new Vector3(targetRotation.eulerAngles.x,cameraObject.transform.eulerAngles.y,cameraObject.transform.eulerAngles.z);
			cameraObject.transform.eulerAngles = new Vector3(cameraObject.transform.eulerAngles.x,targetRotation.eulerAngles.y,cameraObject.transform.eulerAngles.z);
			
			camHeight = Mathf.Lerp(camHeight,camHeight+MouseVerticalDistance*axisSensitivity.y,Time.deltaTime);

			if (keepAboveSurface && suimonoModuleObject != null){
				camHeight = Mathf.Clamp(camHeight,waterLevel+0.25f,12.0f);
			} else {
				camHeight = Mathf.Clamp(camHeight,-1.0f,12.0f);
			}

			// SET CAMERA POSITION and ROTATIONS
			cameraObject.transform.position = cameraTarget.transform.position+(-cameraObject.transform.forward*followTgtDistance);
			cameraObject.transform.position = new Vector3(cameraObject.transform.position.x,(cameraObject.transform.position.y + camHeight),cameraObject.transform.position.z);
			cameraObject.transform.LookAt(new Vector3(targetPosition.x,targetPosition.y + followHeight,targetPosition.z));
			
			//CHECK CAMERA OCCLUSION and REPOSITION
			if (handleObjectOcclusion){
			//RaycastHit[] hits;
			Vector3 testPos = cameraTarget.transform.position;
			testPos = new Vector3(testPos.x,(testPos.y + followHeight),testPos.z);
			RaycastHit hit = new RaycastHit();
		    if(Physics.Linecast(testPos,cameraObject.transform.position, out hit)) {
			    if (hit.transform.gameObject.layer != 4){
					if (hit.transform == transform || hit.transform == cameraTarget){
						//do nothing
					} else {
						//check for triggers
						bool trigCheck = false;
						if (hit.transform.GetComponent<Collider>() != null){
							if (hit.transform.GetComponent<Collider>().isTrigger) trigCheck = true;
						}
						
						if (!trigCheck){
			           	//calculate ray
			            //Ray dirRay = new Ray(testPos, testPos - cameraObject.transform.position);
			           	 //move camera
			            cameraObject.transform.position = hit.point;
			            }
			        }
		        }
		    }
			}

			//set camera offset
			cameraObject.transform.position = new Vector3((cameraObject.transform.position.x + cameraOffset.x),cameraObject.transform.position.y,cameraObject.transform.position.z);
			cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, (cameraObject.transform.position.y + cameraOffset.y),cameraObject.transform.position.z);
			
			//set camera leaning
			cameraObject.transform.eulerAngles = new Vector3(
				cameraObject.transform.rotation.eulerAngles.x,
				cameraObject.transform.rotation.eulerAngles.y,
				cameraLean);
		
		}
		
		
		
		
		//---------------------------------
		//  SET CAMERA SETTINGS and FX
		//---------------------------------
		if (isControllable){
			//SET CAMERA SETTINGS
			cameraObject.GetComponent<Camera>().fieldOfView = camFOV;
		}




		//------------------------------------
		//  SEND MODES TO CHARACTER ANIMATOR
		//------------------------------------
		if (targetAnimator != null){
		

				//send normal animations
				if (moveForward > 0.0f){
					targetAnimator.behaviorIsRevving = true;
					targetAnimator.behaviorIsRevvingHigh = isRunning;
					targetAnimator.behaviorIsRevvingBack = false;
				} else if (moveForward < 0.0f){
					targetAnimator.behaviorIsRevving = false;
					targetAnimator.behaviorIsRevvingHigh = false;
					targetAnimator.behaviorIsRevvingBack = true;
				} else if (moveForward == 0.0f){
					targetAnimator.behaviorIsRevving = false;
					targetAnimator.behaviorIsRevvingHigh = false;
					targetAnimator.behaviorIsRevvingBack = false;
				}
				targetAnimator.engineRotation = moveSideways;
		}

	}

		if (targetAnimator != null){
				targetAnimator.behaviorIsOn = isActive;
		}
	}





}




