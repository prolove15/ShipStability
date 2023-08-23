using UnityEngine;
using System.Collections;


namespace Suimono.Core
{

	public class fx_buoyancy : MonoBehaviour {


		//PUBLIC VARIABLES
		public bool applyToParent = false;
		public bool engageBuoyancy = false;
		public float activationRange = 5000.0f;
		public bool inheritForce = false;
		public bool keepAtSurface = false;
		public float buoyancyOffset = 0.0f;
		public float buoyancyStrength = 1.0f;
		public float forceAmount = 1.0f;
		public float forceHeightFactor = 0.0f;

		// PRIVATE VARIABLES
		private float maxVerticalSpeed = 5.0f;
		private float surfaceRange = 0.2f;
		private float buoyancy = 0.0f;
		private float surfaceLevel = 0.0f;
		private float underwaterLevel = 0.0f;
		private bool isUnderwater = false;
		private Transform physTarget;
		private Suimono.Core.SuimonoModule moduleObject;
		private float waveHeight = 0.0f;
		private float modTime = 0.0f;
		private float splitFac = 1.0f;
		private Rigidbody rigidbodyComponent;
		private float isOver = 0.0f;
		private Vector2 forceAngles = new Vector2(0.0f,0.0f);
		private float forceSpeed = 0.0f;
		private float waveHt = 0.0f;
		//private float displace;

		private int randSeed;
		private Suimono.Core.Random buyRand;

		//collect for GC
		private Vector3 gizPos;
		private float testObjectHeight;
		private float buoyancyFactor;
		private float forceMod;
		private float waveFac;
		private float[] heightValues;
		private bool isEnabled = true;
		private bool performHeight = false;
		private float currRange = -1.0f;
		//private float camRange = -1.0f;
		//private Vector3 currCamPos = new Vector3(-1f,-1f,-1f);
		private Vector3 physPosition;
		private bool saveRigidbodyState;
		private float lerpSurfacePosTime = 0f;
		private float targetYPosition;
		private float startYPosition;
		private bool saveKeepAtSurface;


		void OnDrawGizmos (){
			gizPos = transform.position;
			gizPos.y += 0.03f;
			Gizmos.DrawIcon(gizPos, "gui_icon_buoy.psd", true);
			gizPos.y -= 0.03f;
		}



		void Start () {

			if (GameObject.Find("SUIMONO_Module") != null){
				moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
			}

			//set random
			randSeed = System.Environment.TickCount;
			buyRand = new Suimono.Core.Random(randSeed);

			//get number of buoyant objects
			if (applyToParent){
				var buoyancyObjects = transform.parent.gameObject.GetComponentsInChildren<fx_buoyancy>();
				if (buoyancyObjects != null){
					splitFac = 1f/buoyancyObjects.Length;
				}
			}

			//set physics target
			if (applyToParent){
				physTarget = this.transform.parent.transform;
				if (physTarget != null){
				if (rigidbodyComponent == null){
					rigidbodyComponent = physTarget.GetComponent<Rigidbody>();
				}}
			} else {
				physTarget = this.transform;
				if (physTarget != null){
				if (rigidbodyComponent == null){
					rigidbodyComponent = GetComponent<Rigidbody>();
				}}
			}
		}
		


		void FixedUpdate(){

			SetUpdate();

			//Determine Vertical Speed
			if (isUnderwater == false){
				maxVerticalSpeed = 0.25f;

			} else if (isUnderwater == true){
				maxVerticalSpeed = Mathf.Clamp(surfaceLevel - (transform.position.y+buoyancyOffset-0.5f),0.0f,5.0f);
				if (maxVerticalSpeed > 4.0f) maxVerticalSpeed = 4.0f;
			}

			//Set Buoyancy
			buoyancy = 1 + (maxVerticalSpeed * buoyancyStrength);

		}





		void SetUpdate () {


		if (moduleObject != null){

			//set Random
			if (buyRand == null) buyRand = new Suimono.Core.Random(randSeed);

			//check activations
			performHeight = true;
			if (physTarget != null && moduleObject.setCamera != null){
				
				//check for range activation
				if (activationRange > 0f){
					currRange = Vector3.Distance(moduleObject.setCamera.transform.position, physTarget.transform.position);
					if (currRange >= activationRange){
						performHeight = false;
					}
				}

				if (activationRange <= 0f) performHeight = true;
				
				/*
				//check for frustrum activation
				camRange = 0.2f;
				if (moduleObject != null && performHeight){
				if (moduleObject.setCameraComponent != null){
					currCamPos = moduleObject.setCameraComponent.WorldToViewportPoint(physTarget.transform.position);
					if (currCamPos.x > (1f+camRange) || currCamPos.y > (1f+camRange)){
						performHeight = false;
					}
					if (currCamPos.x < (0f-camRange) || currCamPos.y < (0f-camRange)){
						performHeight = false;
					}
				}
				}
				*/

				//check for enable activation
				if (!isEnabled){
					performHeight = false;
				}
			}
			

			//perform height check
			if (performHeight){
				// Get all height variables from Suimono Module object
				heightValues = moduleObject.SuimonoGetHeightAll(this.transform.position);
				isOver = heightValues[4];
				waveHt = heightValues[8];
				surfaceLevel = heightValues[0];
				forceAngles = moduleObject.SuimonoConvertAngleToVector(heightValues[6]);
				forceSpeed = heightValues[7]*0.1f;
			}

			//clamp variables
			forceHeightFactor = Mathf.Clamp01(forceHeightFactor);
			
			//Reset values
			isUnderwater = false;
			underwaterLevel = 0f;

			//calculate scaling
			testObjectHeight = (transform.position.y+buoyancyOffset-0.5f);
			
				waveHeight = surfaceLevel;
				if (testObjectHeight < waveHeight){
					isUnderwater = true;
				}
				underwaterLevel =  waveHeight-testObjectHeight;


			//set buoyancy
			if (!keepAtSurface && rigidbodyComponent) rigidbodyComponent.isKinematic = saveRigidbodyState;

			if (!keepAtSurface && engageBuoyancy && isOver == 1f){
			if (rigidbodyComponent && !rigidbodyComponent.isKinematic){
					
					//reset rigidbody if turned off
					if (rigidbodyComponent.isKinematic){
						rigidbodyComponent.isKinematic = saveRigidbodyState;
					}

					buoyancyFactor = 10.0f;

					if (isUnderwater){

						if (this.transform.position.y+buoyancyOffset-0.5f < waveHeight-surfaceRange){
						
							// add vertical force to buoyancy while underwater
							forceMod = (buoyancyFactor * (buoyancy * rigidbodyComponent.mass) * (underwaterLevel) * splitFac * (isUnderwater ? 1f : 0f) );
							if (rigidbodyComponent.velocity.y < maxVerticalSpeed){	
								rigidbodyComponent.AddForceAtPosition(new Vector3(0f,1f,0f) * forceMod, transform.position);
							}

							modTime = 0f;
							
						} else {
							
							// slow down vertical velocity as it reaches water surface or wave zenith
							modTime = (this.transform.position.y+buoyancyOffset-0.5f) / (waveHeight+buyRand.Next(0f,0.25f) * (isUnderwater ? 1f : 0f));
							if (rigidbodyComponent.velocity.y > 0f){
								rigidbodyComponent.velocity = new Vector3(
									rigidbodyComponent.velocity.x,
									Mathf.SmoothStep(rigidbodyComponent.velocity.y,0f,modTime),
									rigidbodyComponent.velocity.z);
							}
						}
					
					
						//Add Water Force / Direction to Buoyancy Object
						if (inheritForce){
						if (this.transform.position.y+buoyancyOffset-0.5f <= waveHeight){
							waveFac = Mathf.Lerp(0f,forceHeightFactor,waveHt);
							if (forceHeightFactor == 0f) waveFac = 1f;
							rigidbodyComponent.AddForceAtPosition(new Vector3(forceAngles.x,0f,forceAngles.y) * (buoyancyFactor*2f) * forceSpeed * waveFac * splitFac * forceAmount, transform.position);
						}
						}

					}
					
			}
			}



			//Keep At Surface Option
			if (keepAtSurface && isOver == 1f){



				saveKeepAtSurface = keepAtSurface;
				float testPos = (surfaceLevel - physTarget.position.y - buoyancyOffset);
				if (  testPos >= -0.25f){

					//remove rigidbody
					if (rigidbodyComponent != null){
						//rigidbodyComponent.velocity = Vector3.zero;
						if (!rigidbodyComponent.isKinematic){
								saveRigidbodyState = false;
								rigidbodyComponent.isKinematic = true;
						}
					}

					//set Y position
					physPosition = physTarget.position;
					physPosition.y = Mathf.Lerp(startYPosition, targetYPosition, lerpSurfacePosTime);
					physTarget.position = physPosition;

				} else {
					rigidbodyComponent.isKinematic = saveRigidbodyState;
				}


				//set timer for smooth blend
				lerpSurfacePosTime += Time.deltaTime * 4f;
				if (lerpSurfacePosTime > 1f || keepAtSurface != saveKeepAtSurface){
					lerpSurfacePosTime = 0f;
					startYPosition = physTarget.position.y;
					targetYPosition = surfaceLevel - buoyancyOffset;//physTarget.position.y;
				}

			}
			
		}
		}






	}
}