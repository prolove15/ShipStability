using UnityEngine;
using System.Collections;



public class sui_demo_ControllerOrbit : MonoBehaviour {



	//PUBLIC VARIABLES
	public bool isActive = false;
	public bool isControllable = true;
	public bool isExtraZoom = false;
	public Transform cameraTarget;
	public bool reverseYAxis = true;
	public bool reverseXAxis = false;
	public Vector2 mouseSensitivity = new Vector2(4.0f,4.0f);
	public float cameraFOV = 35.0f;
	public Vector3 cameraOffset = new Vector3(0.0f,0.0f,0.0f);
	public float cameraLean = 0.0f;
	public float rotationSensitivity = 6.0f;
	public Vector3 rotationLimits = new Vector3(0.0f,0.0f,0.0f);
	public float minZoomAmount = 1.25f;
	public float maxZoomAmount = 8.0f;
	public bool showDebug = false;
	public sui_demo_animCharacter targetAnimator;


	//PRIVATE VARIABLES
	private Transform cameraObject;
	//private float rotSense = 0.0f;
	private Vector2 axisSensitivity = new Vector2(4.0f,4.0f);
	private float followDistance = 10.0f;
	private float followHeight = 1.0f;
	private float followLat = 0.0f;
	private float camFOV = 35.0f;
	private float camRotation = 0.0f;
	private Vector3 camRot;
	private float camHeight = 4.0f;
	private bool isInWater = false;
	private bool isInWaterDeep = false;
	private bool isUnderWater = false;
	private Vector3 targetPosition;
	private float MouseRotationDistance = 0.0f;
	private float MouseVerticalDistance = 0.0f;
	private GameObject suimonoGameObject;
	private Suimono.Core.SuimonoModule suimonoModuleObject;
	private float followTgtDistance = 0.0f;
	private bool orbitView = false;
	private Quaternion targetRotation;
	private float MouseScrollDistance = 0.0f;
	//private Transform playerObject;
	//private float projEmitTimer = 0.0f;
	//private float camVRotation = 0.0f;
	//private float firingTime = 0.0f;
	//private float sightingTime = 0.0f;
	private float setFOV = 1.0f;
	//private float targetUseLean = 0.0f;
	//private float useSpeed = 0.0f;
	//private float useSideSpeed = 0.0f;
	//private float useVertSpeed = 0.0f;
	private float moveForward = 0.0f;
	private float moveSideways = 0.0f;
	//private float moveForwardTgt = 0.0f;
	//private float moveSidewaysTgt = 0.0f;
	//private float moveVert = 0.0f;
	private bool isWalking = false;
	private bool isRunning = false;
	private bool isSprinting = false;
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
	private sui_demo_ControllerMaster MC;
	private sui_demo_InputController IC;
	//private float xMove = 0.0f;
	private float runButtonTime = 0.0f;
	private bool toggleRun = false;
	private float gSlope = 0.0f;
	private float useSlope = 0.0f;





	void Awake() {

		//get Suimono Specific Objects
		suimonoGameObject = GameObject.Find("SUIMONO_Module");
		if (suimonoGameObject != null) suimonoModuleObject = suimonoGameObject.GetComponent<Suimono.Core.SuimonoModule>();
		
		targetPosition = cameraTarget.position;
		targetRotation = cameraTarget.rotation;
		
		MC = this.gameObject.GetComponent<sui_demo_ControllerMaster>() as sui_demo_ControllerMaster;
		IC = this.gameObject.GetComponent<sui_demo_InputController>() as sui_demo_InputController;
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

				//RESET MOVEMENT
				moveForward = 0.0f;
				moveSideways = 0.0f;
				//moveVert = 0.0f;

				// GET WASD MOVEMENT KEYS
				if (IC.inputKeyW) moveForward = 1.0f;
				if (IC.inputKeyS) moveForward = -1.0f;
				if (IC.inputKeyA) moveSideways = -1.0f;
				if (IC.inputKeyD) moveSideways = 1.0f;
				//if (IC.inputKeyQ) moveVert = -1.0f;
				//if (IC.inputKeyE) moveVert = 1.0f;
					
				//MOUSE BUTTON 0
				//isMouseMove = IC.inputMouseKey0;

				//MOUSE BUTTON 1
				isExtraZoom = IC.inputMouseKey1;
				if (isExtraZoom){
					setFOV = 0.5f;
				} else {
					setFOV = 1.0f;
				}
				
				//SHIFT RUN/SPRINT
				// Tap Shift to toggle
				// hold shift to sprint
				isWalking = false;
				if (moveForward != 0.0f || moveSideways != 0.0f) isWalking = true;
				if (IC.inputKeySHIFTL){
					runButtonTime += Time.deltaTime;
						if (runButtonTime > 0.2f){
							isSprinting = true;
						}
				} else {
					if (runButtonTime > 0.0f){
						if (runButtonTime < 0.2f){
							isRunning = !isRunning;
							if (isRunning) toggleRun = true;
						}
						if (runButtonTime > 0.2f){
							isRunning = false;
						}
					}
					if (isSprinting && toggleRun) isRunning = true;
					isSprinting = false;
					runButtonTime = 0.0f;
				}


				//SPACE
				if (Input.mousePosition.x > 325f || Input.mousePosition.y < 265f){
					orbitView = IC.inputMouseKey0 || IC.inputMouseKey1;
				} else {
					orbitView = false;
					IC.inputMouseKey0 = false;
					IC.inputMouseKey1 = false;
				}

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

			axisSensitivity = new Vector2(
				Mathf.Lerp(axisSensitivity.x,mouseSensitivity.x,Time.deltaTime*4.0f),
				Mathf.Lerp(axisSensitivity.y,mouseSensitivity.y,Time.deltaTime*4.0f)
				);

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
				float waterLevel = suimonoModuleObject.SuimonoGetHeight(cameraTarget.position,"object depth");


				isInWater = false;
				
				if (waterLevel < 0.0f) waterLevel = 0.0f;
				if (waterLevel > 0.0f){
			
					isInWater = true;
					isInWaterDeep = false;
					isUnderWater = false;
					
					if (waterLevel >= 0.9f && waterLevel < 1.8f) isInWaterDeep = true;
					if (waterLevel >= 1.8f) isUnderWater = true;

				}
			}
			

			
			if (isControllable){

				//---------------------------------
				//  CAMERA POSITIONING
				//---------------------------------

				//Calculate Follow Distance
				float followLerpSpeed = 2.0f;
				followDistance -= (MouseScrollDistance*20.0f);
				followDistance = Mathf.Clamp(followDistance,minZoomAmount,maxZoomAmount);
				followTgtDistance = Mathf.Lerp(followTgtDistance,followDistance,Time.deltaTime*followLerpSpeed);
				
				// Calculate Rotation
				if (orbitView) camRotation = Mathf.Lerp(oldMouseRotation,MouseRotationDistance*axisSensitivity.x,Time.deltaTime);

				targetRotation.eulerAngles = new Vector3(
					targetRotation.eulerAngles.x,
					(targetRotation.eulerAngles.y+camRotation),
					targetRotation.eulerAngles.z
					);

				cameraObject.transform.eulerAngles = new Vector3(
					targetRotation.eulerAngles.x,
					targetRotation.eulerAngles.y,
					cameraObject.transform.eulerAngles.z
					);

				if (orbitView) camHeight = Mathf.Lerp(camHeight,camHeight+MouseVerticalDistance*axisSensitivity.y,Time.deltaTime);
				camHeight = Mathf.Clamp(camHeight,-45.0f,45.0f);

				// SET CAMERA POSITION and ROTATIONS
				cameraObject.transform.position = new Vector3(
					cameraTarget.transform.position.x+cameraOffset.x+(-cameraObject.transform.up.x*followTgtDistance),
					Mathf.Lerp(camHeight,cameraTarget.transform.position.y+cameraOffset.y+(-cameraObject.transform.up.y*followTgtDistance),Time.deltaTime*0.5f),
					cameraTarget.transform.position.z+cameraOffset.z+(-cameraObject.transform.up.z*followTgtDistance)
					);

				cameraObject.transform.LookAt(new Vector3(targetPosition.x,targetPosition.y + followHeight,targetPosition.z));
						
			}
			
			
			//---------------------------------
			//  SET CAMERA SETTINGS and FX
			//---------------------------------
			if (isControllable){
				//SET CAMERA SETTINGS
				cameraObject.GetComponent<Camera>().fieldOfView = camFOV;
			}


		}


		//------------------------------------
		//  SEND MODES TO CHARACTER ANIMATOR
		//------------------------------------
		if (targetAnimator != null){



			targetAnimator.isWalking = isWalking;
			targetAnimator.isRunning = isRunning;
			targetAnimator.isSprinting = isSprinting;
			targetAnimator.moveForward = moveForward;
			targetAnimator.moveSideways = moveSideways;

			targetAnimator.gSlope = gSlope;
			targetAnimator.useSlope = useSlope;
			targetAnimator.isInWater = isInWater;
			targetAnimator.isInWaterDeep = isInWaterDeep;
			targetAnimator.isUnderWater = isUnderWater;	
		}
	}



}




