using UnityEngine;
using System.Collections;



public class sui_demo_ControllerCharacter : MonoBehaviour {


	//PUBLIC VARIABLES
	public bool isActive = false;
	public bool isControllable = true;
	public bool isExtraZoom = false;
	public bool keepAboveSurface = false;
	public bool handleObjectOcclusion = true;

	public Transform cameraTarget;
	public Transform buoyancyTarget;
	public bool reverseYAxis = true;
	public bool reverseXAxis = false;
	public Vector2 mouseSensitivity = new Vector2(4.0f,4.0f);
	public float cameraFOV = 35.0f;
	public Vector2 cameraOffset = new Vector2(0.0f,0.0f);
	public float cameraLean = 0.0f;
	public float walkSpeed = 0.02f;
	public float runSpeed = 0.4f;
	public float sprintSpeed = 0.4f;
	public float rotationSensitivity = 6.0f;
	public Vector3 rotationLimits = new Vector3(0.0f,0.0f,0.0f);
	public float minZoomAmount = 1.25f;
	public float maxZoomAmount = 8.0f;
	public bool showDebug = false;
	public sui_demo_animCharacter targetAnimator;
	public bool isInBoat = false;


	//PRIVATE VARIABLES
	private Transform cameraObject;
	private Suimono.Core.fx_buoyancy buoyancyObject;
	private float rotSense = 0.0f;
	private Vector2 axisSensitivity = new Vector2(4.0f,4.0f);
	private float followDistance = 5.0f;
	private float followHeight = 1.0f;
	private float followLat = 0.0f;
	private float camFOV = 35.0f;
	private float camRotation = 0.0f;
	private Vector3 camRot;
	private float camHeight = 2.0f;
	private bool isInWater = false;
	private bool isInWaterDeep = false;
	private bool isUnderWater = false;
	private bool isAtSurface = false;
	private bool isFloating = false;
	private bool isFalling = false;
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
	private float useSpeed = 0.0f;
	private float useSideSpeed = 0.0f;
	private float useVertSpeed = 0.0f;
	private float moveSpeed = 0.05f;
	private float moveForward = 0.0f;
	private float moveSideways = 0.0f;
	private float moveForwardTgt = 0.0f;
	private float moveSidewaysTgt = 0.0f;
	private float moveVert = 0.0f;
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
	private float xMove = 0.0f;
	private float runButtonTime = 0.0f;
	private bool toggleRun = false;
	private float gSlope = 0.0f;
	private float useSlope = 0.0f;
	private float waterLevel;



	void Awake() {

		//get Suimono Specific Objects
		suimonoGameObject = GameObject.Find("SUIMONO_Module");
		if (suimonoGameObject != null) suimonoModuleObject = suimonoGameObject.GetComponent<Suimono.Core.SuimonoModule>();
		
		targetPosition = cameraTarget.position;
		targetRotation = cameraTarget.rotation;
		
		if (cameraTarget != null){
			targetAnimator = cameraTarget.gameObject.GetComponent<sui_demo_animCharacter>();
		}
		
		if (buoyancyTarget != null){
			buoyancyObject = buoyancyTarget.GetComponent<Suimono.Core.fx_buoyancy>();
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

				//RESET MOVEMENT
				moveForward = 0.0f;
				moveSideways = 0.0f;
				moveVert = 0.0f;

				// GET WASD MOVEMENT KEYS
				if (IC.inputKeyW) moveForward = 1.0f;
				if (IC.inputKeyS) moveForward = -1.0f;
				if (IC.inputKeyA) moveSideways = -1.0f;
				if (IC.inputKeyD) moveSideways = 1.0f;
				if (IC.inputKeyQ) moveVert = -1.0f;
				if (IC.inputKeyE) moveVert = 1.0f;

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
				orbitView = IC.inputKeySPACE;

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
			if (suimonoModuleObject != null){
				waterLevel = suimonoModuleObject.SuimonoGetHeight(cameraTarget.position,"object depth");

				isInWater = false;
				
				if (waterLevel < 0.0f) waterLevel = 0.0f;
				if (waterLevel > 0.0f){
			
					isInWater = true;
					isInWaterDeep = false;
					isUnderWater = false;
					isFloating = false;
					isAtSurface = false;
					
					if (waterLevel >= 0.9f && waterLevel < 1.8f) isInWaterDeep = true;
					if (waterLevel >= 1.8f) isUnderWater = true;
					if (waterLevel >= 1.2f && waterLevel < 1.8f) isAtSurface = true;
					if (isInWaterDeep && waterLevel > 2.0f) isFloating = true;

				}
				
				//turn off buoyancy when underwater (if object exists)
				if (isUnderWater){
					if (buoyancyObject != null) buoyancyObject.engageBuoyancy = false;
				} else {
					if (buoyancyObject != null) buoyancyObject.engageBuoyancy = true;
				}
				
			}
			

			
			if (isControllable){

				//---------------------------------
				//  CHARACTER ROTATION
				//---------------------------------

				//ROTATE CHARACTER
				if (!orbitView){

				//calculate rotation sensitivity
				rotSense = rotationSensitivity;
				if (isSprinting) rotSense *= 2.0f;

				//calculate rotation  3-360 hedge
				float rotH = 0.0f;
				float tgt = 0.0f;

					//move forward
					if (moveForward == 1.0f && moveSideways == 0.0f){
						tgt = cameraObject.transform.eulerAngles.y;
						if ((cameraTarget.eulerAngles.y-tgt) > 180.0f) rotH = -360.0f;
						if ((cameraTarget.eulerAngles.y-tgt) < -180.0f) rotH = 360.0f;
						cameraTarget.eulerAngles = new Vector3(
							cameraTarget.eulerAngles.x,
							Mathf.Lerp(cameraTarget.eulerAngles.y+rotH,tgt,Time.deltaTime*rotSense),
							cameraTarget.eulerAngles.z
							);

					//move backward
					} else if (moveForward == -1.0f && moveSideways == 0.0f){
						rotSense *= 1.0f;
						tgt = cameraObject.transform.eulerAngles.y-180.0f;
						if ((cameraTarget.eulerAngles.y-tgt) > 180.0f) rotH = -360.0f;
						if ((cameraTarget.eulerAngles.y-tgt) < -180.0f) rotH = 360.0f;
						cameraTarget.eulerAngles = new Vector3(
							cameraTarget.eulerAngles.x,
							Mathf.Lerp(cameraTarget.eulerAngles.y+rotH,tgt,Time.deltaTime*rotSense),
							cameraTarget.eulerAngles.z
							);

					//move sideways
					} else if (moveSideways != 0.0f && moveForward == 0.0f){
						tgt = cameraObject.transform.eulerAngles.y+(90f*moveSideways);
						if ((cameraTarget.eulerAngles.y-tgt) > 180.0f) rotH = -360.0f;
						if ((cameraTarget.eulerAngles.y-tgt) < -180.0f) rotH = 360.0f;
						cameraTarget.eulerAngles = new Vector3(
							cameraTarget.eulerAngles.x,
							Mathf.Lerp(cameraTarget.eulerAngles.y+rotH,tgt,Time.deltaTime*rotSense),
							cameraTarget.eulerAngles.z
							);

					//move forward side
					} else if (moveSideways != 0.0f && moveForward == 1.0f){
						rotSense *= 1.4f;
						tgt = cameraObject.transform.eulerAngles.y+(45f*moveSideways);
						if ((cameraTarget.eulerAngles.y-tgt) > 180.0f) rotH = -360.0f;
						if ((cameraTarget.eulerAngles.y-tgt) < -180.0f) rotH = 360.0f;
						cameraTarget.eulerAngles = new Vector3(
							cameraTarget.eulerAngles.x,
							Mathf.Lerp(cameraTarget.eulerAngles.y+rotH,tgt,Time.deltaTime*rotSense),
							cameraTarget.eulerAngles.z
							);

					//move backward side
					} else if (moveSideways != 0.0f && moveForward == -1.0f){
						rotSense *= 1.4f;
						tgt = cameraObject.transform.eulerAngles.y-180.0f-(45f*moveSideways);
						if ((cameraTarget.eulerAngles.y-tgt) > 180.0f) rotH = 360.0f;
						if ((cameraTarget.eulerAngles.y-tgt) < -180.0f) rotH = -360.0f;
						cameraTarget.eulerAngles = new Vector3(
							cameraTarget.eulerAngles.x,
							Mathf.Lerp(cameraTarget.eulerAngles.y-rotH,tgt,Time.deltaTime*rotSense),
							cameraTarget.eulerAngles.z
							);

					} else {
					
						xMove = Mathf.Lerp(xMove,0.0f,Time.deltaTime);
					}
					
					cameraTarget.eulerAngles = new Vector3(0.0f,cameraTarget.eulerAngles.y,0.0f);
				}



				//---------------------------------
				// CALCULATE GROUND SLOPE
				//---------------------------------
				gSlope = 0.0f;
				useSlope = 0.0f;
				Vector3 startPos = cameraTarget.position + new Vector3(0f,1.0f,0f);
				Vector3 endPos = cameraTarget.position + new Vector3(0f,-0.25f,0f);
			    RaycastHit hitA;
			    RaycastHit hitB;

				if(Physics.Linecast(startPos, endPos, out hitA)){
					if (hitA.transform != cameraTarget.transform){
						if (showDebug) Debug.DrawLine (startPos, endPos, Color.red);

						if (Physics.Linecast(startPos+(cameraTarget.forward*0.25f), endPos+(cameraTarget.forward*0.25f), out hitB)){
							if (showDebug) Debug.DrawLine (startPos+(cameraTarget.forward*0.25f), endPos+(cameraTarget.forward*0.25f), Color.green);
							if (showDebug) Debug.DrawLine (hitA.point,hitB.point, Color.black);

							//calculate angle
							float aFac1 = (hitB.point.y-hitA.point.y);
							float aFac2 = (hitB.point.x-hitA.point.x);
							gSlope = (Mathf.Atan2(aFac1,aFac2) * Mathf.Rad2Deg);
							useSlope = gSlope;
							if (gSlope < 0.0f) useSlope = 90.0f+((Mathf.Atan2(aFac1,aFac2) * Mathf.Rad2Deg) % 90.0f);
						}
					}
				}


				//---------------------------------
				// SET MOVEMENT SPEEDS
				//---------------------------------
				if (isUnderWater) moveSideways = 0.0f;
				
				if (moveForward == 0.0f && moveSideways == 0.0f){
					isWalking = false;
					isRunning = false;
					isSprinting = false;
				}

				float spdLerp = 1.7f;
				if (isRunning) spdLerp = 2.5f;
				if (isSprinting) spdLerp = 3.5f;	

				moveSpeed = walkSpeed;
				if (isInWaterDeep || isUnderWater) isRunning = false;
				if (isRunning) moveSpeed = runSpeed;
				if (isSprinting) moveSpeed = sprintSpeed;

				if (moveForward != 0.0f && moveSideways != 0.0f) moveSpeed *= 0.5f;

				float wMod = 1.0f;
				if (isInWater) wMod = 0.8f;
				if (isInWaterDeep) wMod = 0.6f;
				if (isUnderWater) wMod = 0.5f;
				moveSpeed *= wMod;

				//slowmovement based on slope
				if (gSlope > 0.0f){
					if (useSlope > 0.25f && useSlope < 90.0f){
						moveSpeed *= (1.25f-(useSlope / 90.0f));
					}
				}

				useSpeed = Mathf.Lerp(useSpeed, (moveSpeed * moveForward), Time.deltaTime*spdLerp);
				useSideSpeed = Mathf.Lerp(useSideSpeed, (moveSpeed * moveSideways), Time.deltaTime*spdLerp);

				if (isUnderWater || isInWater){
					useVertSpeed = Mathf.Lerp(useVertSpeed, (moveSpeed * moveVert), Time.deltaTime*spdLerp);
				} else {
					useVertSpeed = 0.0f;
				}



				//---------------------------------
				//  CHARACTER MOVEMENT
				//---------------------------------
				if (cameraTarget.GetComponent<Rigidbody>()){

					//calculate forward / backward movement
					Vector3 setNewPos;
					setNewPos = ((cameraTarget.transform.forward * useSpeed * moveForwardTgt));
					if (moveForward != 0.0f && moveForwardTgt != moveForward) moveForwardTgt = moveForward;

					//calculate sideways movement
					Vector3 setNewSidePos;
					setNewSidePos = ((cameraTarget.transform.forward * useSideSpeed * moveSidewaysTgt));
					if (moveSideways != 0.0f && moveSidewaysTgt != moveSideways) moveSidewaysTgt = moveSideways;

					//calculate vertical while underwater
					Vector3 setNewVertPos;
					setNewVertPos = new Vector3(0f,useVertSpeed,0f);

					//set final movement
					cameraTarget.GetComponent<Rigidbody>().MovePosition(cameraTarget.GetComponent<Rigidbody>().position + (setNewPos + setNewSidePos + setNewVertPos));
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

				targetRotation.eulerAngles = new Vector3(
					targetRotation.eulerAngles.x,
					(targetRotation.eulerAngles.y + camRotation),
					targetRotation.eulerAngles.z
					);

				cameraObject.transform.eulerAngles = new Vector3(
					targetRotation.eulerAngles.x,
					targetRotation.eulerAngles.y,
					cameraObject.transform.eulerAngles.z
					);

				
				camHeight = Mathf.Lerp(camHeight,camHeight+MouseVerticalDistance*axisSensitivity.y,Time.deltaTime);
				
				
				if (keepAboveSurface && suimonoModuleObject != null){
					camHeight = Mathf.Clamp(camHeight,waterLevel+0.25f,12.0f);
				} else {
					camHeight = Mathf.Clamp(camHeight,-1.0f,12.0f);
				}

				// SET CAMERA POSITION and ROTATIONS
				cameraObject.transform.position = cameraTarget.transform.position+(-cameraObject.transform.forward*followTgtDistance);

				cameraObject.transform.position = new Vector3(
					cameraObject.transform.position.x,
					(cameraObject.transform.position.y + camHeight),
					cameraObject.transform.position.z
					);

				cameraObject.transform.LookAt(new Vector3(targetPosition.x,targetPosition.y + followHeight,targetPosition.z));
				
				//CHECK CAMERA OCCLUSION and REPOSITION
				if (handleObjectOcclusion){
				//RaycastHit[] hits;
				Vector3 testPos = cameraTarget.transform.position;
				testPos = new Vector3(testPos.x, (testPos.y + followHeight), testPos.z);
				RaycastHit hit = new RaycastHit();
			    if(Physics.Linecast(testPos,cameraObject.transform.position, out hit)) {
				    if (hit.transform.gameObject.layer != suimonoModuleObject.layerWaterNum){
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

				//set camera leaning
				cameraObject.transform.eulerAngles = new Vector3(
					cameraObject.transform.rotation.eulerAngles.x,
					cameraObject.transform.rotation.eulerAngles.y,
					cameraLean
					);
			}
			
			
			
			//---------------------------------
			//  SET CAMERA SETTINGS and FX
			//---------------------------------
			if (isControllable){
				//SET CAMERA SETTINGS
				cameraObject.GetComponent<Camera>().fieldOfView = camFOV;
			}
		}


		//-----------------------------------
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
			targetAnimator.isFloating = isFloating;
			targetAnimator.isAtSurface = isAtSurface;
			targetAnimator.isFalling = isFalling;
			
			targetAnimator.isInBoat = isInBoat;

		}
	}



}




