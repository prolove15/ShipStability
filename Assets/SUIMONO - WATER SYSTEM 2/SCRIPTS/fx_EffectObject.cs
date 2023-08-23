using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Sui_FX_Rules{
		none,isUnderWater,isAboveWater,isAtWaterSurface,speedIsGreater,speedIsLess,
		waterDepthGreater,waterDepthLess
		}
public enum Sui_FX_RuleModifiers{
		and,or
		}
public enum Sui_FX_System{
		none,bubbles,rings,ringfoam,splash,splashdrops
		}
//public enum Sui_FX_ActionType{
//		repeat,once
//		}



namespace Suimono.Core
{

	[ExecuteInEditMode]
	public class fx_EffectObject : MonoBehaviour {

		//PUBLIC VARIABLES
		public Suimono.Core.SuimonoModuleFX fxObject;			
		public Sui_FX_Rules[] effectRule;
		public float[] effectData;
		public Sui_FX_Rules[] resetRule;
		public string[] effectSystemName;
		public Sui_FX_System[] effectSystem;
		//public Sui_FX_ActionType actionType = Sui_FX_ActionType.repeat;
		public Vector2 effectDelay = new Vector2(1.0f,1.0f);
		public Vector2 emitTime = new Vector2(1.0f,1.0f);
		public Vector2 emitNum = new Vector2(1.0f,1.0f);
		public Vector2 effectSize = new Vector2(1.0f,1.0f);
		public float emitSpeed;
		public float speedThreshold;
		public float directionMultiplier;
		public bool emitAtWaterLevel = false;
		public float effectDistance = 100.0f;

		//audio
		public AudioClip audioObj;
		public Vector2 audioVol = new Vector2(0.9f,1.0f);
		public Vector2 audioPit = new Vector2(0.8f,1.2f);
		public float audioSpeed;

		//events
		public bool enableEvents = false;

		//color
		public Color tintCol = new Color(1f,1f,1f,1f);
		public bool clampRot = false;

		// for custom editor
		public int actionIndex  = 1;
		[System.NonSerialized]public List<string> actionOptions = new List<string>(){
			"Once","Repeat","Specific"
			};
		public int actionNum = 5;
		public float actionReset = 15f;

		public int typeIndex  = 0;
		public int[] ruleIndex;
		[System.NonSerialized]public List<string> ruleOptions = new List<string>(){
			"None","Object Is Underwater","Object Is Above Water","Object Is At Surface",
			"Object Speed Is Greater Than","Object Speed Is Less Than","Water Depth Is Greater Than",
			"Water Depth Is Less Than"
			};

		public int systemIndex = 0;
		public List<string> sysNames = new List<string>();
		public float currentSpeed ;


		//PRIVATE VARIABLES
		private int actionCount = 0;
		private float actionTimer = 0f;
		private Vector3 savePos = new Vector3(0f,0f,0f);
		private Suimono.Core.SuimonoModule moduleObject;
		private float emitTimer;
		private bool delayPass = true;
		private bool actionPass = true;
		private float useSpd;
		private float useAudioSpd;
		private float isOverWater;
		private float currentWaterPos;
		private Vector3 emitPos;
		private bool rulepass = false;
		private float timerAudio = 0.0f;
		private float timerParticle = 0.0f;
		private float currentCamDistance = 0.0f;

		//collect for GC
		private Vector3 gizPos;
		private int sN;
		private int s;
		private bool[] ruleCheck;
		private int ruleCKNum = 0;
		private bool[] resetCheck;
		private int rCK;
		private int emitN;
		private float emitS;
		private Vector3 emitV;
		private float emitR;
		private float emitAR;
		private bool rp;
		private float ruleData;
		private float depth;
		private Sui_FX_Rules[] tempRules;
		private int[] tempIndex;
		private float[] tempData;
		private int aR;	
		private int endLP;
		private int setInt;
		private float[] heightValues;
		
		private Transform transf;

		private int randSeed;
		private Suimono.Core.Random fxRand;
						
		// create a simple cheap "smear" effect on the InvokeRepeating processor load.
		// by shifting the work into rough groups via a simple static int, that we loop over.
		 
		// our global counter
		static int staggerOffset = 0;
		 
		// our loop, we chose groups of roughly 20
		static int staggerModulus = 20;
		 
		// our actual stagger value 
		private float stagger;

		private float _deltaTime;



		void OnDrawGizmos (){
			gizPos = transform.position;
			gizPos.y += 0.03f;
			Gizmos.DrawIcon(gizPos, "gui_icon_fxobj.psd", true);
		}



		void Start () {

			// Object References
			transf = this.transform;
			if (GameObject.Find("SUIMONO_Module")){
				moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
				if (moduleObject != null) fxObject = moduleObject.suimonoModuleLibrary.fxObject;
			}


			//populate system names
			if (fxObject != null){
				sysNames = fxObject.sysNames;
			}	

			//set random
			randSeed = System.Environment.TickCount;
			fxRand = new Suimono.Core.Random(randSeed);

			//run update loop at set FPS interval
		    staggerOffset++;
		    stagger = (staggerOffset+0f) *0.05f  ;
		    staggerOffset = staggerOffset % staggerModulus;
		    
			InvokeRepeating("SetUpdate",0.1f+stagger,(1.0f/30.0f));
		}
		


		void SetUpdate(){
			
			if (moduleObject != null){

				//Cache Time for performance
				_deltaTime = Time.deltaTime;

				//check for action timing
				actionPass = false;
				if (actionIndex == 0 && actionCount < 1) actionPass = true;
				if (actionIndex == 2 && actionCount < actionNum) actionPass = true;
				if (actionIndex == 1) actionPass = true;

				if (actionCount > 0 && (actionIndex == 0 || actionIndex == 2)){
					actionTimer += _deltaTime;
					if (actionTimer > actionReset && actionReset > 0f){
						actionCount = 0;
						actionTimer = 0f;
					}
				}



				//set Random
				if (fxRand == null) fxRand = new Suimono.Core.Random(randSeed);

				//get objects while in editor mode
				#if UNITY_EDITOR
				if (!Application.isPlaying){
					if (moduleObject == null){	
					if (GameObject.Find("SUIMONO_Module")){
						moduleObject = GameObject.Find("SUIMONO_Module").GetComponent<Suimono.Core.SuimonoModule>() as Suimono.Core.SuimonoModule;
						fxObject = moduleObject.suimonoModuleLibrary.fxObject;
					}
					}
				}
				#endif

				if (Application.isPlaying){

					//calculate camera distance
					if (moduleObject.setTrack != null){
						currentCamDistance = Vector3.Distance(transf.position,moduleObject.setTrack.transform.position);
						if (currentCamDistance <= effectDistance){
						
							//track position / speed
							if (savePos != transf.position){
								currentSpeed = Vector3.Distance(savePos,new Vector3(transf.position.x,transf.position.y,transf.position.z))/_deltaTime;
							}
							savePos = transf.position;

							// track timers and emit
							timerParticle += _deltaTime;
							timerAudio += _deltaTime;
							
							EmitFX();
							if (timerAudio > audioSpeed){
								timerAudio = 0f;
								EmitSoundFX();
							}

						}
					}


					//Event trigger function
					if (enableEvents){
                   		timerEvent += _deltaTime;
                    	BroadcastEvent();
                	}

				}
			}
		}




		void EmitSoundFX(){

			if (actionPass){
				if (audioObj != null && moduleObject != null){
					if (moduleObject.gameObject.activeInHierarchy){
						if (rulepass){
							moduleObject.AddSoundFX(audioObj,emitPos,new Vector3(0f,fxRand.Next(audioPit.x,audioPit.y),fxRand.Next(audioVol.x,audioVol.y)));
						}
					}
				}
			}
		}



		void EmitFX () {

		if (moduleObject.enableInteraction){
			if (Application.isPlaying && moduleObject != null && moduleObject.gameObject.activeInHierarchy){	

				if (actionPass){

					//######################################
					//##    CALCULATE TIMING and DELAYS   ##
					//######################################
					delayPass = false;

					emitTimer += _deltaTime;
					if (emitTimer >= emitSpeed){
						emitTimer = 0f;
						delayPass = true;
					}

					
					
					//####################################
					//##    CALCULATE WATER RELATION   ##
					//####################################
					heightValues = moduleObject.SuimonoGetHeightAll(transf.position);
					currentWaterPos = heightValues[3];
					isOverWater = heightValues[4];


					//##########################
					//##    CALCULATE RULES   ##
					//##########################
					rulepass = false;
					if (ruleCheck == null) ruleCheck = new bool[effectRule.Length];
					ruleCKNum = 0;
					if (resetCheck == null) resetCheck = new bool[resetRule.Length];
					

					if (Application.isPlaying){	

						var rp = false;
						for (rCK = 0; rCK < effectRule.Length; rCK++){


							//CHECK ALL RULES

							rp = false;
							ruleData = speedThreshold;

							//get depth
							depth = currentWaterPos;
							
							if (rCK < effectData.Length) ruleData = effectData[rCK];
							
							//rules.none
							if (ruleIndex[rCK] == 0){
								rp = true;
							}

							//rules.ObjectIsUnderwater
							if (ruleIndex[rCK] == 1 && isOverWater==1f){
								if (depth > 0f) rp = true;
							}

							//rules.ObjectIsAbovewater
							if (ruleIndex[rCK] == 2 && isOverWater==1f){
								if (depth <= 0f) rp = true;
							}

							//rules.ObjectIsAtSurface
							if (ruleIndex[rCK] == 3 && isOverWater==1f){
								if (depth < 0.15f && depth > -0.15f) rp = true;
							}

							//rules.speedIsGreater
							if (ruleIndex[rCK] == 4 && isOverWater==1f){
								if (currentSpeed > ruleData) rp = true;
							}

							//rules.speedIsLess
							if (ruleIndex[rCK] == 5 && isOverWater==1f){
								if (currentSpeed < ruleData) rp = true;
							}
							
							//rules.WaterDepthGreater
							if (ruleIndex[rCK] == 6 && isOverWater==1f){
								if (depth > ruleData) rp = true;
							}

							//rules.WaterDepthIsLess
							if (ruleIndex[rCK] == 7 && isOverWater==1f){
								if (depth < ruleData) rp = true;
							}

							ruleCheck[rCK] = rp;




						}
					}

					//determine if all rules are passed
					for (rCK = 0; rCK < effectRule.Length; rCK++){
						if (ruleCheck[rCK]) ruleCKNum += 1;
					}
					if (ruleCKNum == effectRule.Length) rulepass = true;
					
					//no rules
					if (effectRule.Length == 0){
						rulepass = true;
					}
					
					
					//######################
					//##    INITIATE FX   ##
					//######################
					if (delayPass && rulepass){
					
						emitN = Mathf.FloorToInt(fxRand.Next(emitNum.x,emitNum.y));
						emitS = fxRand.Next(effectSize.x,effectSize.y);
						emitV = new Vector3(0f,0f,0f);
						emitPos = transform.position;
						emitR = transform.eulerAngles.y-180f;

						if (!clampRot){
							emitR = fxRand.Next(-30f,10f);
						}
						emitAR = fxRand.Next(-360f,360f);
						
						//get water level
						if (emitAtWaterLevel){
							emitPos = new Vector3(emitPos.x, (transform.position.y + currentWaterPos)-0.35f, emitPos.z);
						}
						
						if (directionMultiplier > 0f){
							emitV = transform.up * (directionMultiplier * Mathf.Clamp01((currentSpeed/speedThreshold)));
						}
						
						//EMIT PARTICLE SYSTEM
						if (timerParticle > emitSpeed){
							timerParticle = 0f;

						if (systemIndex-1 >= 0){
							emitPos.y += (emitS*0.4f);
							emitPos.x += fxRand.Next(-0.2f,0.2f);
							emitPos.z += fxRand.Next(-0.2f,0.2f);
							moduleObject.AddFX(systemIndex-1, emitPos, emitN, fxRand.Next(0.5f,0.75f)*emitS, emitR, emitAR, emitV, tintCol);
						}

						actionCount++;

						}
					}

				}
			}
		}
		}







		public void AddRule(){

			tempRules  = effectRule;
			
			tempIndex  = ruleIndex;
			tempData = effectData;

			effectRule = new Sui_FX_Rules[tempRules.Length+1];
			ruleIndex = new int[tempRules.Length+1];
			effectData = new float[tempRules.Length+1];	

			for (aR = 0; aR < tempRules.Length; aR++){
				effectRule[aR] = tempRules[aR];
				ruleIndex[aR] = tempIndex[aR];
				effectData[aR] = tempData[aR];
			}
			effectRule[tempRules.Length] = Sui_FX_Rules.none;
			ruleIndex[tempRules.Length] = 0;
			effectData[tempRules.Length] = 0;
			
		}




		public void DeleteRule(int ruleNum){

		 	tempRules = effectRule;
		 	tempIndex = ruleIndex;
		 	tempData = effectData;
		 	
			endLP = tempRules.Length-1;
			
			if (endLP <= 0){
				endLP = 0;
				effectRule = new Sui_FX_Rules[0];
				ruleIndex = new int[0];
				effectData = new float[0];
				
			} else {

				effectRule = new Sui_FX_Rules[endLP];
				ruleIndex = new int[endLP];
				effectData = new float[endLP];	
				setInt = -1;
				
				for (aR = 0; aR <= endLP; aR++){
					
					if (aR != ruleNum){
						setInt += 1;
					} else {
						setInt += 2;
					}

					if (setInt <= endLP){
						effectRule[aR] = tempRules[setInt];
						ruleIndex[aR] = tempIndex[setInt];
						effectData[aR] = tempData[setInt];
					}
				}
			}	
		}





		void OnDisable(){
			CancelInvoke("SetUpdate");
		}

		void OnEnable(){

		    staggerOffset++;
		    stagger = (staggerOffset+0f) *0.05f  ;
		    staggerOffset = staggerOffset % staggerModulus;
		    
			CancelInvoke("SetUpdate");
			InvokeRepeating("SetUpdate",0.1f+stagger,(1f/10f));
		}





        /// Used to broadcast location info.
        public delegate void TriggerHandler(Vector3 position, Quaternion rotatoin);

        // Broadcast when Event Trigger fires.
        public event TriggerHandler OnTrigger;

        // Broadcast interval in second.
        public float eventInterval = 1f;

        // Should event returns water level position instead of fx_EffectObject position?
        public bool eventAtSurface = false;

        // simple timer
        private float timerEvent;


        void BroadcastEvent()
        {
            if (!moduleObject.isActiveAndEnabled || !rulepass || OnTrigger == null || timerEvent < eventInterval) return;
            timerEvent = 0f;
            //Position: if atSurface is true, then return water level position, otherwise return this.transform.position
            OnTrigger.Invoke(eventAtSurface ? new Vector3(emitPos.x, (transf.position.y + currentWaterPos) - 0.35f, emitPos.z) : transf.position, transf.rotation);
        }





	}
}