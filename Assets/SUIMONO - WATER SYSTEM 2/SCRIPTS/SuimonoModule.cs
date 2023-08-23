using UnityEngine;
using System.Collections;
using System.Collections.Generic;



#if UNITY_EDITOR
	using UnityEditor;
#endif


namespace Suimono.Core
{
	[ExecuteInEditMode]
	public class SuimonoModule : MonoBehaviour {

		//Underwater Effects variables
		public string suimonoVersionNumber = "";
		public float systemTime = 0f;

		//layers
		public bool autoSetLayers = true;
		public string layerWater;
		public int layerWaterNum = -1;
		public string layerDepth;
		public int layerDepthNum = -1;
		public string layerScreenFX;
		public int layerScreenFXNum = -1;
		//public string layerUnderwater;

		public bool layersAreSet = false;

		#if UNITY_EDITOR
			public SerializedObject tagManager;
			public SerializedProperty projectlayers;
		#endif

		public bool autoSetCameraFX = true;
		public Transform manualCamera;
		public Transform mainCamera;
		public int cameraTypeIndex = 0;
		[System.NonSerialized] public List<string> cameraTypeOptions = new List<string>(){"Auto Select Camera","Manual Select Camera"};
		public Transform setCamera;
		public Transform setTrack;
		public Light setLight;

		public bool enableUnderwaterFX = true;
		public bool enableInteraction = true;
		public float  objectEnableUnderwaterFX = 1f;

		public bool disableMSAA = false;
		
		public bool enableRefraction = true;
		public bool enableReflections = true;
		public bool enableDynamicReflections = true;
		public bool enableCaustics = true;
		public bool enableCausticsBlending = false;
		public bool enableAdvancedEdge = true;
		public bool enableAdvancedDistort = true;
		public bool enableTenkoku = false;
		public bool enableAutoAdvance = true;
		public bool showPerformance = false;
		public bool showGeneral = false;

		public Color underwaterColor = new Color(0.58f,0.61f,0.61f,0.0f);
		public bool enableTransition = true;
		public float  transition_offset = 0.1f;
		public GameObject fxRippleObject;

		//private ParticleSystem underwaterDebris;
		private float underLightAmt = 0f;

		private GameObject targetSurface;
		private float doTransitionTimer = 0f;
		 
		public bool isUnderwater = false;
		static bool doWaterTransition = false;


		//transparency
		public bool enableTransparency = true;
		private bool useEnableTransparency = true;
		public int transResolution = 3;
		public int transLayer = 0;
		public LayerMask transLayerMask;
		public int causticLayer = 0;
		public LayerMask causticLayerMask;
		[System.NonSerialized] public List<string> suiLayerMasks;
		[System.NonSerialized] public List<string> resOptions = new List<string>(){"4096","2048","1024","512","256","128","64","32","16","8"};
		[System.NonSerialized] public List<int> resolutions = new List<int>(){4096,2048,1024,512,256,128,64,32,16,8};

		public float transRenderDistance = 100f;
		//private Suimono.Core.cameraTools transToolsObject;
		//private Camera transCamObject;
		//private Suimono.Core.cameraTools causticToolsObject;
		//private Suimono.Core.cameraCausticsHandler causticHandlerObjectTrans;
		//private Suimono.Core.cameraCausticsHandler causticHandlerObject;
		//private Camera causticCamObject;
		//private GameObject wakeObject;
		//private Camera wakeCamObject;
		//private GameObject normalsObject;
		//private Camera normalsCamObject;

		public bool playSounds = true;
		public bool playSoundBelow = true;
		public bool playSoundAbove = true;
		public float maxVolume = 1f;
		public int maxSounds = 10;
		public AudioClip[] defaultSplashSound;

		//public Suimono.Core.SuimonoModuleFX fxObject;

		private float setvolume = 0.65f;

		//private Suimono.Core.fx_soundModule sndparentobj;
		private GameObject underSoundObject;
		private AudioSource underSoundComponent;
		private AudioSource[] sndComponents;
		private int currentSound = 0;

		public float transitionStrength = 1f;

		public float currentObjectIsOver = 0f;
		public float currentObjectDepth = 0f;
		public float currentTransitionDepth = 0f;
		public float currentSurfaceLevel = 0f;
		public float underwaterThreshold = 0.1f;
		public Suimono.Core.SuimonoObject suimonoObject;


		private ParticleSystem.Particle[] effectBubbles;
		public Suimono.Core.SuimonoModuleLib suimonoModuleLibrary;

		//private Renderer underwaterDebrisRendererComponent;
		//private Transform underwaterDebrisTransform;
		public Camera setCameraComponent;
		private float underTrans = 0f;

		//tenkoku specific variables
		public float useTenkoku = 0f;
		public float tenkokuWindDir = 0f;
		public float tenkokuWindAmt = 0f;
		public bool tenkokuUseWind = true;
		private GameObject tenObject;
		public bool showTenkoku = true;
		public bool tenkokuUseReflect = true;
		private WindZone tenkokuWindModule;

		//collect for GC
		private int lx;
		private int fx;
		private int px;
		private ParticleSystem.Particle[] setParticles;
		private AudioClip setstep;
		private float setpitch;
		//private float waitTime;
		private AudioSource useSoundAudioComponent;
		private float useRefract; 
		private float useLight = 1f; 
		private Color useLightCol;
		private Vector2 flow_dir;
		private Vector3 tempAngle;
		//private Color getmap;
		private float getheight;
		private float getheightC;
		private float getheightT;
		private float getheightR;
		private bool isOverWater;
		private float surfaceLevel;
		//private float groundLevel;
		private int layer;
		private int layermask;
		private Vector3 testpos;
		private int i;
		private RaycastHit hit;
		private Vector2 pixelUV;
		private float returnValue;
		private float[] returnValueAll;
		private float h1;
		private float setDegrees = 0f;
		private float enabledUFX = 1f;
		private float enabledCaustics = 1f;

		private float setUnderBright;
		//public Suimono.Core.fx_causticModule causticObject;
		private float enCaustic = 0f;
		private float setEdge = 1f;
		private Suimono.Core.Suimono_UnderwaterFog underwaterObject;
		private GameObject currentSurfaceObject = null;

		public float[] heightValues;
		//public Light causticObjectLight;
		public float isForward = 0f;
		public float isAdvDist = 0f;

		//Height Variables
		public float waveScale = 1f;
		public float flowSpeed = 0.02f;
		public float offset = 0f;
		public Texture2D heightTex;
		public Texture2D heightTexT;
		public Texture2D heightTexR;
		public Transform heightObject;
		public Vector2 relativePos = new Vector2(0f,0f);
		public Vector3 texCoord = new Vector3(0f,0f,0f);
		public Vector3 texCoord1 = new Vector3(0f,0f,0f);
		public Vector3 texCoordT = new Vector3(0f,0f,0f);
		public Vector3 texCoordT1 = new Vector3(0f,0f,0f);
		public Vector3 texCoordR = new Vector3(0f,0f,0f);
		public Vector3 texCoordR1 = new Vector3(0f,0f,0f);
		public Color heightVal0;
		public Color heightVal1;
		public Color heightValT0;
		public Color heightValT1;
		public Color heightValR0;
		public Color heightValR1;
		public float localTime = 0f;
		private float baseHeight = 0f;
		private float baseAngle = 0f;
		private Color[] pixelArray;
		private Color[] pixelArrayT;
		private Color[] pixelArrayR;

		private Texture2D useDecodeTex;
		private Color[] useDecodeArray;
		public int row;
		public int pixIndex;
		public Color pixCol;

		public int t;
		public int y;
		#if UNITY_EDITOR
			public SerializedProperty layerTP;
			public SerializedProperty layerWP;
			public SerializedProperty layerSP;
			public SerializedProperty layerXP;
			public SerializedProperty layerN;
		#endif

		public Vector3 dir;
		public Vector3 pivotPoint;
		public float useLocalTime;
		public Vector2 flow_dirC;
		public Vector2 flowSpeed0;
		public Vector2 flowSpeed1;
		public Vector2 flowSpeed2;
		public Vector2 flowSpeed3;
		public float tScale;
		public Vector2 oPos;

		private int renderCount = 0;
		private int randSeed;
		private Suimono.Core.Random modRand;

		//private float surfaceTimer = 0f;
		//private Suimono.Core.SuimonoObject[] sObjects;
		private List<SuimonoObject> sObjects;
       	private List<Renderer> sRends;
       	private List<Renderer> sRendSCs;



		//Variables for Unity 5.4+ only
		#if UNITY_5_4_OR_NEWER
			private ParticleSystem.EmissionModule debrisEmission;
		#endif

		private ColorSpace _colorspace;
		private float _deltaTime;


		void Awake () {

			//###  SET CURRENT SUIMONO NUMBER   ###
			suimonoVersionNumber = "2.1.14";
			
			//Force name
			gameObject.name = "SUIMONO_Module";

			//Initialize Lists
			sObjects = new List<SuimonoObject>();
		    sRends = new List<Renderer>();
		    sRendSCs = new List<Renderer>();
		}



		void Start () {

			//### DISCONNECT FROM PREFAB ###;
			#if UNITY_EDITOR
			#if !UNITY_2018_3_OR_NEWER
				PrefabUtility.DisconnectPrefabInstance(this.gameObject);
			#endif
			#endif
		    
		    //set random
			randSeed = System.Environment.TickCount;
			modRand = new Suimono.Core.Random(randSeed);

		    //### SET LAYERS ###;
			InitLayers();

			//Set Camera and Track Objects
			Suimono_CheckCamera();

			//SET PHYSICS LAYER INTERACTIONS
			//This is introduced because Unity 5 no longer handles mesh colliders and triggers without throwing an error.
			//thanks a whole lot guys O_o (for nuthin').  The below physics setup should workaround this problem for everyone.
			if (autoSetLayers){
				for (lx = 0; lx < 32; lx++){
					//loop through and decouple layer collisions for all layers(up to 20).
					//layer 4 is the built-in water layer.
					Physics.IgnoreLayerCollision(lx,layerWaterNum);
				}
			}

			//INITIATE OBJECTS
		    suimonoModuleLibrary = this.gameObject.GetComponent<Suimono.Core.SuimonoModuleLib>() as Suimono.Core.SuimonoModuleLib;
		    /*
		    if (GameObject.Find("_caustic_effects") != null) causticObject = GameObject.Find("_caustic_effects").GetComponent<Suimono.Core.fx_causticModule>();
			if (causticObject != null) causticObjectLight = GameObject.Find("mainCausticObject").GetComponent<Light>();

			//transparency objects
			transToolsObject = this.transform.Find("cam_SuimonoTrans").gameObject.GetComponent<Suimono.Core.cameraTools>();
			transCamObject = this.transform.Find("cam_SuimonoTrans").gameObject.GetComponent<Camera>() as Camera;
			causticHandlerObjectTrans = this.transform.Find("cam_SuimonoTrans").gameObject.GetComponent<cameraCausticsHandler>();

			causticToolsObject = this.transform.Find("cam_SuimonoCaustic").gameObject.GetComponent<Suimono.Core.cameraTools>();
			causticCamObject = this.transform.Find("cam_SuimonoCaustic").gameObject.GetComponent<Camera>() as Camera;
			causticHandlerObject = this.transform.Find("cam_SuimonoCaustic").gameObject.GetComponent<Suimono.Core.cameraCausticsHandler>();

			//wake advanced effect objects
			wakeObject = this.transform.Find("cam_SuimonoWake").gameObject;
			wakeCamObject = this.transform.Find("cam_SuimonoWake").gameObject.GetComponent<Camera>() as Camera;
			normalsObject = this.transform.Find("cam_SuimonoNormals").gameObject;
			normalsCamObject = this.transform.Find("cam_SuimonoNormals").gameObject.GetComponent<Camera>() as Camera;

		    //Effects Initialization
		    fxObject = this.gameObject.GetComponent<Suimono.Core.SuimonoModuleFX>() as Suimono.Core.SuimonoModuleFX;
			if (GameObject.Find("_sound_effects") != null) sndparentobj = GameObject.Find("_sound_effects").gameObject.GetComponent<Suimono.Core.fx_soundModule>();
		    if (GameObject.Find("effect_underwater_debris") != null) underwaterDebris = GameObject.Find("effect_underwater_debris").gameObject.GetComponent<ParticleSystem>();
		    //if (GameObject.Find("effect_fx_bubbles") != null) effectBubbleSystem = GameObject.Find("effect_fx_bubbles").gameObject.GetComponent<ParticleSystem>();
		    */



			//get Surface objects
			//InitSurfaceRenderers();


			#if UNITY_EDITOR
			if (EditorApplication.isPlaying){
			#endif

			if (suimonoModuleLibrary.sndparentobj != null){


				//init sound object pool
				maxSounds = suimonoModuleLibrary.sndparentobj.maxSounds;
				sndComponents = new AudioSource[maxSounds];
				GameObject[] soundObjectPrefab = new GameObject[maxSounds];

				for (int sx = 0; sx < (maxSounds); sx++){
					soundObjectPrefab[sx] = new GameObject("SuimonoAudioObject");
					soundObjectPrefab[sx].transform.parent = suimonoModuleLibrary.sndparentobj.transform;
					soundObjectPrefab[sx].AddComponent<AudioSource>();
					sndComponents[sx] = soundObjectPrefab[sx].GetComponent<AudioSource>();

				}
				
				//init underwater sound
				if(suimonoModuleLibrary.sndparentobj.underwaterSound != null){
					underSoundObject = new GameObject("Underwater Sound");
					underSoundObject.AddComponent<AudioSource>();
					underSoundObject.transform.parent = suimonoModuleLibrary.sndparentobj.transform;
					underSoundComponent = underSoundObject.GetComponent<AudioSource>();
				}
			}

			#if UNITY_EDITOR
			}
			#endif



			//tun off antialiasing (causes unexpected rendering issues.  Recommend post fx aliasing instead)
			if (disableMSAA){
				QualitySettings.antiAliasing = 0;
			}

			//cache quality settings
			_colorspace = QualitySettings.activeColorSpace;

			//set linear space flag
			if (_colorspace == ColorSpace.Linear){
				Shader.SetGlobalFloat("_Suimono_isLinear",1.0f);
			} else {
				Shader.SetGlobalFloat("_Suimono_isLinear",0.0f);
			}


			//store pixel arrays for Height Calculation
			if (suimonoModuleLibrary != null){
				
				if (suimonoModuleLibrary.texNormalC != null){
					heightTex = suimonoModuleLibrary.texNormalC;
					pixelArray = suimonoModuleLibrary.texNormalC.GetPixels(0);
				}
				if (suimonoModuleLibrary.texNormalT != null){
					heightTexT = suimonoModuleLibrary.texNormalT;
					pixelArrayT = suimonoModuleLibrary.texNormalT.GetPixels(0);
				} 
				if (suimonoModuleLibrary.texNormalR != null){
					heightTexR = suimonoModuleLibrary.texNormalR;
					pixelArrayR = suimonoModuleLibrary.texNormalR.GetPixels(0);
				} 

			}

			//set tenkoku flag
			tenObject = GameObject.Find("Tenkoku DynamicSky");
			Shader.SetGlobalFloat("_useTenkoku",0.0f);
		}
		







		void InitLayers () {

			//check whether layers are set
			#if UNITY_EDITOR
				tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
				projectlayers = tagManager.FindProperty("layers");
				layersAreSet = false;
				
				for (t = 8; t <= 31; t++){
			    	layerTP = projectlayers.GetArrayElementAtIndex(t);
			        if (layerTP.stringValue != ""){
			        	if (layerTP.stringValue == "Suimono_Water" || layerTP.stringValue == "Suimono_Depth" || layerTP.stringValue == "Suimono_Screen" || layerTP.stringValue == "Suimono_Underwater"){
			        		layersAreSet = true;
			        	}
			        }
			    }

	    	    if (!autoSetLayers){
					//if (layerTP.stringValue == "Suimono_Water") layerWaterNum = t;
					//if (layerTP.stringValue == "Suimono_Depth") layerDepthNum = t;
					//if (layerTP.stringValue == "Suimono_Screen") layerScreenFXNum = t;

					layerWaterNum = LayerMask.NameToLayer("Suimono_Water");
					layerDepthNum = LayerMask.NameToLayer("Suimono_Depth");
					layerScreenFXNum = LayerMask.NameToLayer("Suimono_Screen");

	    		}


		    #endif


		    if (autoSetLayers){

			    //Set Layers if Applicable
				if (!layersAreSet){
				#if UNITY_EDITOR
					
			        if (projectlayers == null || !projectlayers.isArray){
			            Debug.LogWarning("Can't set up Suimono layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
			            Debug.LogWarning("Layers is null: " + (projectlayers == null));
			            return;
			        }

					layerWater = "Suimono_Water";
					layerDepth = "Suimono_Depth";
					layerScreenFX = "Suimono_Screen";

					//ASSIGN LAYERS
					layerWaterNum = -1;
					layerDepthNum = -1;
					layerScreenFXNum = -1;

					for (y = 8; y <= 31; y++){
			        	layerWP = projectlayers.GetArrayElementAtIndex(y);
			            if (layerWP.stringValue != layerWater && layerWP.stringValue == "" && layerWaterNum == -1){
			            	layerWaterNum = y;
			                if (!layersAreSet) Debug.Log("Setting up Suimono layers.  Layer " + layerWaterNum + " is now called " + layerWater);
			                layerWP.stringValue = layerWater;
			            }
			        	layerSP = projectlayers.GetArrayElementAtIndex(y);
			            if (layerSP.stringValue != layerDepth && layerWP.stringValue == "" && layerDepthNum == -1){
			            	layerDepthNum = y;
			                if (!layersAreSet) Debug.Log("Setting up Suimono layers.  Layer " + layerDepthNum + " is now called " + layerDepth);
			                layerSP.stringValue = layerDepth;
			            }
			            layerXP = projectlayers.GetArrayElementAtIndex(y);
			            if (layerXP.stringValue != layerScreenFX && layerWP.stringValue == "" && layerScreenFXNum == -1){
			            	layerScreenFXNum = y;
			                if (!layersAreSet) Debug.Log("Setting up Suimono layers.  Layer " + layerScreenFXNum + " is now called " + layerScreenFX);
			                layerXP.stringValue = layerScreenFX;
			            }
			        }

			        if (!layersAreSet) tagManager.ApplyModifiedProperties();

			    #endif
			    layersAreSet = true;
				}
			}
		}






/*
		void InitLayers_orig () {

			//check whether layers are set
			#if UNITY_EDITOR
				tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
				projectlayers = tagManager.FindProperty("layers");
				layersAreSet = false;
				
				for (t = 8; t <= 31; t++){
			    	layerTP = projectlayers.GetArrayElementAtIndex(t);
			        if (layerTP.stringValue != ""){
			        	if (layerTP.stringValue == "Suimono_Water" || layerTP.stringValue == "Suimono_Depth" || layerTP.stringValue == "Suimono_Screen" || layerTP.stringValue == "Suimono_Underwater"){
			        		layersAreSet = true;
			        	}
			        }

		    	    if (!autoSetLayers){
						if (layerTP.stringValue == "Suimono_Water") layerWaterNum = t;
						if (layerTP.stringValue == "Suimono_Depth") layerDepthNum = t;
						if (layerTP.stringValue == "Suimono_Screen") layerScreenFXNum = t;
		    		}

			    }
		    #endif


		    if (autoSetLayers){

			    //Set Layers if Applicable
				if (!layersAreSet){
				#if UNITY_EDITOR
					
			        if (projectlayers == null || !projectlayers.isArray){
			            Debug.LogWarning("Can't set up Suimono layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
			            Debug.LogWarning("Layers is null: " + (projectlayers == null));
			            return;
			        }

					layerWater = "Suimono_Water";
					layerDepth = "Suimono_Depth";
					layerScreenFX = "Suimono_Screen";
					layerUnderwater = "Suimono_Underwater";

					//ASSIGN LAYERS
					layerWaterNum = -1;
					layerDepthNum = -1;
					layerScreenFXNum = -1;

					for (y = 8; y <= 31; y++){
			        	layerWP = projectlayers.GetArrayElementAtIndex(y);
			            if (layerWP.stringValue != layerWater && layerWP.stringValue == "" && layerWaterNum == -1){
			            	layerWaterNum = y;
			                if (!layersAreSet) Debug.Log("Setting up Suimono layers.  Layer " + layerWaterNum + " is now called " + layerWater);
			                layerWP.stringValue = layerWater;
			            }
			        	layerSP = projectlayers.GetArrayElementAtIndex(y);
			            if (layerSP.stringValue != layerDepth && layerWP.stringValue == "" && layerDepthNum == -1){
			            	layerDepthNum = y;
			                if (!layersAreSet) Debug.Log("Setting up Suimono layers.  Layer " + layerDepthNum + " is now called " + layerDepth);
			                layerSP.stringValue = layerDepth;
			            }
			            layerXP = projectlayers.GetArrayElementAtIndex(y);
			            if (layerXP.stringValue != layerScreenFX && layerWP.stringValue == "" && layerScreenFXNum == -1){
			            	layerScreenFXNum = y;
			                if (!layersAreSet) Debug.Log("Setting up Suimono layers.  Layer " + layerScreenFXNum + " is now called " + layerScreenFX);
			                layerXP.stringValue = layerScreenFX;
			            }
			        }

			        if (!layersAreSet) tagManager.ApplyModifiedProperties();

			    #endif
			    layersAreSet = true;
				}
			}
		}
*/




		void LateUpdate (){

			//Cache Time for performance
			_deltaTime = Time.deltaTime;


			//Update Random
			if (modRand == null) modRand = new Suimono.Core.Random(randSeed);
			
			//Set Water System Time
			if (systemTime < 0.0f) systemTime = 0.0f;
			if (enableAutoAdvance) systemTime += _deltaTime;


			//set project layer masks
			#if UNITY_EDITOR
				suiLayerMasks = new List<string>();
				tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
				projectlayers = tagManager.FindProperty("layers");
				for (i = 0; i < projectlayers.arraySize; i++){
					layerN = projectlayers.GetArrayElementAtIndex(i);
					suiLayerMasks.Add(layerN.stringValue);
				}
			#endif

			//CHECK FOR SURFACE OBJECT CULLING
			SetCullFunction();

			//GET TENKOKU SPECIFIC VARIABLES
			useTenkoku = 0.0f;
			if (tenObject != null){
				if (tenObject.activeInHierarchy){
					useTenkoku = 1.0f;
				}

				if (useTenkoku == 1.0f){
					if (setLight == null) setLight = GameObject.Find("LIGHT_World").GetComponent<Light>();
					if (tenkokuWindModule == null){
						tenkokuWindModule = GameObject.Find("Tenkoku_WindZone").GetComponent<WindZone>();
					} else {
						tenkokuWindDir = tenkokuWindModule.transform.eulerAngles.y;
						tenkokuWindAmt = tenkokuWindModule.windMain;
					}
				}
			}
			Shader.SetGlobalFloat("_useTenkoku",useTenkoku);


			//GET RIPPLE OBJECT REFERENCE AND LAYER
			if (Application.isPlaying && fxRippleObject == null){
				fxRippleObject = GameObject.Find("fx_rippleNormals(Clone)");
			}
			if (fxRippleObject != null){
				fxRippleObject.layer = layerScreenFXNum;
			}


			//SET COMPONENT LAYERS
			if (suimonoModuleLibrary.normalsCamObject != null) suimonoModuleLibrary.normalsCamObject.cullingMask = 1<<layerScreenFXNum;
			if (suimonoModuleLibrary.wakeCamObject != null) suimonoModuleLibrary.wakeCamObject.cullingMask = 1<<layerScreenFXNum;


			//HANDLE COMPONENTS

			//Tranparency function
			if (suimonoModuleLibrary.transCamObject != null){
				transLayer = (transLayer & ~(1 << layerWaterNum)); //remove water layer from transparent mask
				transLayer = (transLayer & ~(1 << layerDepthNum)); //remove Depth layer from transparent mask
				transLayer = (transLayer & ~(1 << layerScreenFXNum)); //remove Screen layer from transparent mask

				suimonoModuleLibrary.transCamObject.cullingMask = transLayer; 
				suimonoModuleLibrary.transCamObject.farClipPlane = transRenderDistance*1.2f;
				Shader.SetGlobalFloat("_suimonoTransDist", transRenderDistance);
			} else {
				suimonoModuleLibrary.transCamObject = this.transform.Find("cam_SuimonoTrans").gameObject.GetComponent<Camera>() as Camera;
			}

			if (suimonoModuleLibrary.transToolsObject != null){
				suimonoModuleLibrary.transToolsObject.resolution = System.Convert.ToInt32(resolutions[transResolution]);
				//if (useEnableTransparency == false){
				//	suimonoModuleLibrary.transToolsObject.gameObject.SetActive(false);
				//} else {
				//	suimonoModuleLibrary.transToolsObject.gameObject.SetActive(true);
				//}
				suimonoModuleLibrary.transToolsObject.gameObject.SetActive(enableTransparency);

			} else {
				suimonoModuleLibrary.transToolsObject = this.transform.Find("cam_SuimonoTrans").gameObject.GetComponent<cameraTools>();
			}
			

			//Caustic function
			if (suimonoModuleLibrary.causticCamObject != null){

				if (enableCaustics){
					suimonoModuleLibrary.causticCamObject.gameObject.SetActive(enableCausticsBlending);
					transLayer = (transLayer & ~(1 << layerDepthNum)); //remove Depth layer from transparent mask
					transLayer = (transLayer & ~(1 << layerScreenFXNum)); //remove Screen layer from transparent mask
					suimonoModuleLibrary.causticCamObject.cullingMask = transLayer; 
					suimonoModuleLibrary.causticCamObject.farClipPlane = transRenderDistance*1.2f;
				}

				//remove caustics from transparency function
				if (suimonoModuleLibrary.causticHandlerObjectTrans != null){
					suimonoModuleLibrary.causticHandlerObjectTrans.enabled = !enableCausticsBlending;
					//if (!enableCaustics){
					//	suimonoModuleLibrary.causticHandlerObjectTrans.enabled = false;
					//}
				}
			} else {
				suimonoModuleLibrary.causticCamObject = this.transform.Find("cam_SuimonoCaustic").gameObject.GetComponent<Camera>() as Camera;
			}

			if (suimonoModuleLibrary.causticToolsObject != null){
				suimonoModuleLibrary.causticToolsObject.resolution = System.Convert.ToInt32(resolutions[transResolution]);
			} else {
				suimonoModuleLibrary.causticToolsObject = this.transform.Find("cam_SuimonoCaustic").gameObject.GetComponent<Suimono.Core.cameraTools>();
			}

			Shader.SetGlobalFloat("_enableTransparency",useEnableTransparency?1.0f:0.0f);

			//caustics function
			enCaustic = 0.0f;
			if (enableCaustics) enCaustic = 1.0f;
			Shader.SetGlobalFloat("_suimono_enableCaustic",enCaustic);

			// Remove Suimono layers from caustics rendering.
			// Note: This can cause "layer exclusion" console notice in deferred rendering
			// when too many layers are excluded from lighting.
			
			//causticLayer = (causticLayer &= ~(1 << layerWaterNum));
			//causticLayer = (causticLayer &= ~(1 << layerDepthNum));
			//causticLayer = (causticLayer &= ~(1 << layerScreenFXNum));
			causticLayer = (causticLayer &= ~(1 << layerWaterNum << layerDepthNum << layerScreenFXNum));

			//advanced edge function
			setEdge = 1.0f;
			if (!enableAdvancedEdge) setEdge = 0.0f;
			Shader.SetGlobalFloat("_suimono_advancedEdge",setEdge);
		}




		void FixedUpdate (){

			//SET PHYSICS LAYER INTERACTIONS
			//This is introduced because Unity 5 no longer handles mesh colliders and triggers without throwing an error.
			//thanks a whole lot guys O_o (for nuthin').  The below physics setup should workaround this problem for everyone.
			if (autoSetLayers){
				for (lx = 0; lx < 20; lx++){
					//loop through and decouple layer collisions for all layers(up to 20).
					//layer 4 is the built-in water layer.
					Physics.IgnoreLayerCollision(lx,layerWaterNum);
				}
			}

			//Set Camera and Track Objects
			Suimono_CheckCamera();

			//set caustics
			if (suimonoModuleLibrary.causticObject != null){
				
				if (Application.isPlaying){
					suimonoModuleLibrary.causticObject.enableCaustics = enableCaustics;
				} else {
					suimonoModuleLibrary.causticObject.enableCaustics = false;
				}
				
				if (setLight != null){
					suimonoModuleLibrary.causticObject.sceneLightObject = setLight;
				}
			}

			//play underwater sounds
			PlayUnderwaterSound();
			

			//######## HANDLE FORWARD RENDERING SWITCH #######
			if (setCamera != null){
				isForward = 0.0f;
				if (setCameraComponent.actualRenderingPath == RenderingPath.Forward){
					isForward = 1.0f;
				}
				Shader.SetGlobalFloat("_isForward",isForward);
			}


			//######## HANDLE ADVANCED DISTORTION SWITCH #######
			if (enableAdvancedDistort){
				isAdvDist = 1.0f;
				suimonoModuleLibrary.wakeObject.SetActive(true);
				suimonoModuleLibrary.normalsObject.SetActive(true);
			} else {
				isAdvDist = 0.0f;
				suimonoModuleLibrary.wakeObject.SetActive(false);
				suimonoModuleLibrary.normalsObject.SetActive(false);
			}
			Shader.SetGlobalFloat("_suimono_advancedDistort",isAdvDist);


			//######## Set Camera Background Color on Shader #######
			if (setCameraComponent != null){
				if (suimonoObject != null){
					setCameraComponent.backgroundColor = suimonoObject.underwaterColor;
				}
				Shader.SetGlobalColor("_cameraBGColor",setCameraComponent.backgroundColor);
			}


			//######## Set Camera Specific Settings #######
			if (setCameraComponent != null){

				//set camera depth mode to 'Depth'.  The alternative mode
				//'DepthNormals' causes rendering errors in Deferred Rendering
				if (setCameraComponent.actualRenderingPath == RenderingPath.DeferredShading){
					setCameraComponent.depthTextureMode = DepthTextureMode.Depth;
					
				} else if (setCameraComponent.actualRenderingPath == RenderingPath.DeferredLighting){
					setCameraComponent.depthTextureMode = DepthTextureMode.Depth;

				} else {
					setCameraComponent.depthTextureMode = DepthTextureMode.DepthNormals;
				}
				
				//Set Water specific visibility layers on camera
				setCameraComponent.cullingMask = setCameraComponent.cullingMask | (1 <<layerWaterNum);
				setCameraComponent.cullingMask = (setCameraComponent.cullingMask & ~(1 << layerDepthNum) & ~(1 << layerScreenFXNum));
			}
		}




		void OnDisable(){
			CancelInvoke("StoreSurfaceHeight");
		}

		void OnEnable(){
			InvokeRepeating("StoreSurfaceHeight",0.01f,0.1f);
		}

		void StoreSurfaceHeight(){
			if (this.enabled){
				if (setCamera != null){

					heightValues = SuimonoGetHeightAll(setCamera.transform.position);
					currentSurfaceLevel = heightValues[1];
					currentObjectDepth = heightValues[3];
					currentObjectIsOver = heightValues[4];
					currentTransitionDepth = heightValues[9];
					objectEnableUnderwaterFX = heightValues[10];

					checkUnderwaterEffects();
					checkWaterTransition();
				}
			}
		}



		void PlayUnderwaterSound(){
		if (Application.isPlaying){
			if (underSoundObject != null && setTrack != null && underSoundComponent != null){
				underSoundObject.transform.position = setTrack.position;

				if (currentTransitionDepth > 0.0f){
					if (playSoundBelow && playSounds){
						underSoundComponent.clip = suimonoModuleLibrary.sndparentobj.underwaterSound;
						underSoundComponent.volume = maxVolume;
						underSoundComponent.loop = true;
						if (!underSoundComponent.isPlaying){
							underSoundComponent.Play();
						}
					} else {
						underSoundComponent.Stop();
					}
				} else {
					if (suimonoModuleLibrary.sndparentobj.underwaterSound != null){
						if (playSoundAbove && playSounds){
							underSoundComponent.clip = suimonoModuleLibrary.sndparentobj.abovewaterSound;
							underSoundComponent.volume = 0.45f*maxVolume;
							underSoundComponent.loop = true;
							if (!underSoundComponent.isPlaying){
								underSoundComponent.Play();
							}
						} else {
							underSoundComponent.Stop();
						}
					}
				}
			}
		}
		}




		public void AddFX(int fxSystem, Vector3 effectPos, int addRate, float addSize, float addRot, float addARot, Vector3 addVeloc, Color addCol){
			if (suimonoModuleLibrary.fxObject != null){
				fx = fxSystem;

				if (suimonoModuleLibrary.fxObject.fxParticles[fx] != null){

					suimonoModuleLibrary.fxObject.fxParticles[fx].Emit(addRate);
					//get particles
					if (setParticles != null) setParticles = null;
					setParticles = new ParticleSystem.Particle[suimonoModuleLibrary.fxObject.fxParticles[fx].particleCount];
					suimonoModuleLibrary.fxObject.fxParticles[fx].GetParticles(setParticles);
					//set particles
					if (suimonoModuleLibrary.fxObject.fxParticles[fx].particleCount > 0.0f){
					for (px = (suimonoModuleLibrary.fxObject.fxParticles[fx].particleCount-addRate); px < suimonoModuleLibrary.fxObject.fxParticles[fx].particleCount; px++){
							
							//set position
							setParticles[px].position = new Vector3(effectPos.x, effectPos.y, effectPos.z);
							
							//set variables
							#if UNITY_5_4_OR_NEWER
								setParticles[px].startSize = addSize;
							#else
								setParticles[px].size = addSize;
							#endif

							setParticles[px].rotation = addRot;
							setParticles[px].angularVelocity = addARot;
							
							setParticles[px].velocity = new Vector3(addVeloc.x, addVeloc.y, addVeloc.z);
							
							#if UNITY_5_4_OR_NEWER
								setParticles[px].startColor *= addCol;
							#else
								setParticles[px].color *= addCol;
							#endif

					}
					suimonoModuleLibrary.fxObject.fxParticles[fx].SetParticles(setParticles,setParticles.Length);
					suimonoModuleLibrary.fxObject.fxParticles[fx].Play();
					}
				}
			}
		}




		public void AddSoundFX(AudioClip sndClip, Vector3 soundPos, Vector3 sndVelocity){

			if (enableInteraction){

				setpitch = 1.0f;
				setvolume = 1.0f;

				if (playSounds && suimonoModuleLibrary.sndparentobj.defaultSplashSound.Length >= 1 ){
					setstep = suimonoModuleLibrary.sndparentobj.defaultSplashSound[modRand.Next(0,defaultSplashSound.Length-1)];
					setpitch = sndVelocity.y;
					setvolume = sndVelocity.z;
					setvolume = Mathf.Lerp(0.0f,1.0f,setvolume) * maxVolume;

					//check depth and morph sounds if underwater
					if (currentObjectDepth > 0.0f){
						setpitch *= 0.25f;
						setvolume *= 0.5f;
					}
					
					useSoundAudioComponent = sndComponents[currentSound];
					useSoundAudioComponent.clip = sndClip;
					if (!useSoundAudioComponent.isPlaying){
						useSoundAudioComponent.transform.localPosition = soundPos;
						useSoundAudioComponent.volume = setvolume;
						useSoundAudioComponent.pitch = setpitch;
						useSoundAudioComponent.minDistance = 4.0f;
						useSoundAudioComponent.maxDistance = 20.0f;
						useSoundAudioComponent.clip = setstep;
						useSoundAudioComponent.loop = false;
						useSoundAudioComponent.Play();
					}

					currentSound += 1;
					if (currentSound >= (maxSounds-1)) currentSound = 0;
				}

			}
		}






		public void AddSound(string sndMode, Vector3 soundPos, Vector3 sndVelocity){

		if (enableInteraction){

			setpitch = 1.0f;
			setvolume = 1.0f;
			
			if (playSounds && suimonoModuleLibrary.sndparentobj.defaultSplashSound.Length >= 1 ){
				setstep = suimonoModuleLibrary.sndparentobj.defaultSplashSound[modRand.Next(0,suimonoModuleLibrary.sndparentobj.defaultSplashSound.Length-1)];
				setpitch = sndVelocity.y;
				setvolume = sndVelocity.z;
				setvolume = Mathf.Lerp(0.0f,10.0f,setvolume);

				//check depth and morph sounds if underwater
				if (currentObjectDepth > 0.0f){
					setpitch *= 0.25f;
					setvolume *= 0.5f;
				}
				
				useSoundAudioComponent = sndComponents[currentSound];
				if (!useSoundAudioComponent.isPlaying){
					useSoundAudioComponent.transform.localPosition = soundPos;
					useSoundAudioComponent.volume = setvolume;
					useSoundAudioComponent.pitch = setpitch;
					useSoundAudioComponent.minDistance = 4.0f;
					useSoundAudioComponent.maxDistance = 20.0f;
					useSoundAudioComponent.clip = setstep;
					useSoundAudioComponent.loop = false;
					useSoundAudioComponent.Play();
				}

				currentSound += 1;
				if (currentSound >= (maxSounds-1)) currentSound = 0;
			}
		}
		}




		void checkUnderwaterEffects(){

			if (Application.isPlaying){
				if (currentTransitionDepth > underwaterThreshold){
				
					if (suimonoObject != null){
						if (enableUnderwaterFX && suimonoObject.enableUnderwater && currentObjectIsOver==1.0f){
							
							isUnderwater = true;
							Shader.SetGlobalFloat("_Suimono_IsUnderwater",1.0f);
							if (suimonoObject != null){
								suimonoObject.useShader = suimonoObject.shader_Under;
							}
							if (suimonoModuleLibrary.causticHandlerObject != null){
								suimonoModuleLibrary.causticHandlerObjectTrans.isUnderwater = true;
								suimonoModuleLibrary.causticHandlerObject.isUnderwater = true;
							}
						}
					}
					
				} else {
					//swap camera rendering to back to default
					isUnderwater = false;
			   		Shader.SetGlobalFloat("_Suimono_IsUnderwater",0.0f);
					if (suimonoObject != null){
						suimonoObject.useShader = suimonoObject.shader_Surface;
					}
					if (suimonoModuleLibrary.causticHandlerObject != null){
						suimonoModuleLibrary.causticHandlerObjectTrans.isUnderwater = false;
						suimonoModuleLibrary.causticHandlerObject.isUnderwater = false;
					}
				}
			}
		}





		void checkWaterTransition () {
		if (Application.isPlaying){

			doTransitionTimer += _deltaTime;
			
			//SET COLORS
			if (currentTransitionDepth > underwaterThreshold && currentObjectIsOver==1.0f){
				
				doWaterTransition = true;

			    //set underwater debris
			    if (suimonoObject != null && setCamera != null){

					if (enableUnderwaterFX && suimonoObject.enableUnderwater && objectEnableUnderwaterFX==1.0f){

						if (suimonoObject.enableUnderDebris){
				       		suimonoModuleLibrary.underwaterDebrisTransform.position = setCamera.transform.position;
						    suimonoModuleLibrary.underwaterDebrisTransform.rotation = setCamera.transform.rotation;
						    suimonoModuleLibrary.underwaterDebrisTransform.Translate(Vector3.forward * 5.0f);

							suimonoModuleLibrary.underwaterDebrisRendererComponent.enabled=true;

							
							#if UNITY_5_4_OR_NEWER
								debrisEmission = suimonoModuleLibrary.underwaterDebris.emission;
								debrisEmission.enabled = isUnderwater;
							#else
								suimonoModuleLibrary.underwaterDebris.enableEmission = isUnderwater;
							#endif

						} else {
							if (suimonoModuleLibrary.underwaterDebris != null) suimonoModuleLibrary.underwaterDebrisRendererComponent.enabled = false;
						}

						setUnderBright = underLightAmt;
						setUnderBright *= 0.5f;


				       	//set attributes to shader
				       	useLight = 1.0f;
				       	useLightCol = new Color(1f,1f,1f,1f);
				       	useRefract = 1.0f;
				       	if (setLight != null){
				       		useLight = setLight.intensity;
				       		useLightCol = setLight.color;
				       	}
				       	if (!enableRefraction) useRefract = 0.0f;


						if (underwaterObject == null){
							if (setCamera.gameObject.GetComponent<Suimono.Core.Suimono_UnderwaterFog>() != null){
								underwaterObject = setCamera.gameObject.GetComponent<Suimono.Core.Suimono_UnderwaterFog>();
							}
						}
						if (underwaterObject != null){
							underwaterObject.lightFactor = suimonoObject.underLightFactor * useLight;
							underwaterObject.refractAmt = suimonoObject.underRefractionAmount * useRefract;
							underwaterObject.refractScale = suimonoObject.underRefractionScale;
							underwaterObject.refractSpd = suimonoObject.underRefractionSpeed * useRefract;
							underwaterObject.fogEnd = suimonoObject.underwaterFogDist;
							underwaterObject.fogStart = suimonoObject.underwaterFogSpread;
							underwaterObject.blurSpread = suimonoObject.underBlurAmount;
							underwaterObject.underwaterColor = suimonoObject.underwaterColor;
							underwaterObject.darkRange = suimonoObject.underDarkRange;

							underwaterObject.transitionStrength = transitionStrength;
							
							Shader.SetGlobalColor("_suimono_lightColor",useLightCol);
							underwaterObject.doTransition = false;

							//set caustic and underwater light brightness
							if (suimonoModuleLibrary.causticObject != null){
								
								if (Application.isPlaying){

									if (suimonoModuleLibrary.causticObject != null){
										suimonoModuleLibrary.causticObject.heightFac = underwaterObject.hFac * 2.0f;
									}
								}
							}
						}


					} else {
						if (suimonoModuleLibrary.underwaterDebris != null) suimonoModuleLibrary.underwaterDebrisRendererComponent.enabled = false;
					}
				}

				if (underwaterObject != null){
					underwaterObject.cancelTransition = true;
				}

		    } else {

		        //reset underwater debris
		        if (suimonoModuleLibrary.underwaterDebris != null){
		       		suimonoModuleLibrary.underwaterDebrisRendererComponent.enabled=false;
		       	}

		     	//show water transition
		     	if (enableTransition){
		     	if (doWaterTransition && setCamera != null){
		     		
		     		doTransitionTimer = 0.0f;

		      		if (underwaterObject != null){
						underwaterObject.doTransition = true;
					}

		       		doWaterTransition = false;
		
		     	} else {
		     		underTrans = 1.0f;
		     	}
		       	}
		    }

		    if (!enableUnderwaterFX){
		    	if (suimonoModuleLibrary.underwaterDebrisRendererComponent != null){
					suimonoModuleLibrary.underwaterDebrisRendererComponent.enabled=false;
				}
		    }
		}
		}




		void Suimono_CheckCamera(){

			//get main camera object
			if (cameraTypeIndex == 0){
				if (Camera.main != null){
					mainCamera = Camera.main.transform;
				}
				manualCamera = null;
			}

			if (cameraTypeIndex == 1){
				if (manualCamera != null){
					mainCamera = manualCamera;
				} else {
					if (Camera.main != null){
						mainCamera = Camera.main.transform;
					}
				}
			}

			//set camera
			if (setCamera != mainCamera){
				setCamera = mainCamera;
				setCameraComponent = setCamera.gameObject.GetComponent<Camera>();
				underwaterObject = setCamera.gameObject.GetComponent<Suimono.Core.Suimono_UnderwaterFog>();
			}

			//set camera component
			if (setCameraComponent == null && setCamera != null){
					setCameraComponent = setCamera.gameObject.GetComponent<Camera>();
			}

			//reset camera component
			if (setCamera != null && setCameraComponent != null){
				if (setCameraComponent.transform != setCamera){
					setCameraComponent = setCamera.gameObject.GetComponent<Camera>();
					underwaterObject = setCamera.gameObject.GetComponent<Suimono.Core.Suimono_UnderwaterFog>();
				}
			}
			
			//set track object
			if (setTrack == null && setCamera != null){
				setTrack = setCamera.transform;
			}

			//install camera effects
			InstallCameraEffect();
		}






		public Vector2 SuimonoConvertAngleToDegrees(float convertAngle){

			flow_dir = new Vector3(0f,0f,0f);
			tempAngle = new Vector3(0f,0f,0f);
			if (convertAngle <= 180.0f){
				tempAngle = Vector3.Slerp(Vector3.forward,-Vector3.forward,(convertAngle)/180.0f);
				flow_dir = new Vector2(tempAngle.x,tempAngle.z);
			}
			if (convertAngle > 180.0f){
				tempAngle = Vector3.Slerp(-Vector3.forward,Vector3.forward,(convertAngle-180.0f)/180.0f);
				flow_dir = new Vector2(-tempAngle.x,tempAngle.z);
			}
			
			return flow_dir;
		}






		public Vector2 SuimonoConvertAngleToVector(float convertAngle){

			flow_dir = new Vector3(0f,0f,0f);
			tempAngle = new Vector3(0f,0f,0f);
			if (convertAngle <= 180.0f){
				tempAngle = Vector3.Slerp(Vector3.forward,-Vector3.forward,(convertAngle)/180.0f);
				flow_dir = new Vector2(tempAngle.x,tempAngle.z);
			}
			if (convertAngle > 180.0f){
				tempAngle = Vector3.Slerp(-Vector3.forward,Vector3.forward,(convertAngle-180.0f)/180.0f);
				flow_dir = new Vector2(-tempAngle.x,tempAngle.z);
			}
			
			return flow_dir;
		}







		public float SuimonoGetHeight(Vector3 testObject, string returnMode) {

			// Get Heights
			CalculateHeights(testObject);

			// Return values
			returnValue = 0.0f;
			
			if (returnMode == "height") returnValue = getheight;
			if (returnMode == "surfaceLevel") returnValue = surfaceLevel+getheight;
			if (returnMode == "baseLevel") returnValue = surfaceLevel;
			if (returnMode == "object depth") returnValue = getheight-testObject.y;
			if (returnMode == "isOverWater" && isOverWater) returnValue = 1.0f;
			if (returnMode == "isOverWater" && !isOverWater) returnValue = 0.0f;
			
			if (returnMode == "isAtSurface"){
				if (((surfaceLevel+getheight)-testObject.y) < 0.25f && ((surfaceLevel+getheight)-testObject.y) > -0.25f)
					returnValue = 1.0f;
			}
			
			if (suimonoObject != null){
				if (returnMode == "direction") returnValue = suimonoObject.flowDirection;
				if (returnMode == "speed") returnValue = suimonoObject.flowSpeed;
					
				if (returnMode == "wave height"){
					h1 = 0.0f;
					returnValue = getheight/h1;
				}
			}

			if (returnMode == "transitionDepth") returnValue = ((surfaceLevel+getheight)-(testObject.y-(transition_offset*underTrans)));


			if (returnMode == "underwaterEnabled"){
				enabledUFX = 1f;
				if (!suimonoObject.enableUnderwater) enabledUFX = 0f;
				returnValue = enabledUFX;
			}

			if (returnMode == "causticsEnabled"){
				enabledCaustics = 1f;
				if (!suimonoObject.enableCausticFX) enabledCaustics = 0f;
				returnValue = enabledCaustics;
			}

			return returnValue;
		}






		public float[] SuimonoGetHeightAll(Vector3 testObject){

			// Get Heights
			CalculateHeights(testObject);

			// Return values
			returnValueAll = new float[12];
			
			// 0 height
			returnValueAll[0]=(getheight);
			
			// 1 surface level
			returnValueAll[1]=(getheight);

			// 2 base level
			returnValueAll[2]=(surfaceLevel);
			
			// 3 object depth
			returnValueAll[3]=((getheight)-testObject.y);

			// 4 is Over Water
			returnValue = 1.0f;
			if (!isOverWater) returnValue = 0.0f;
			returnValueAll[4]= returnValue;
			
			// 5 is at surface
			returnValue = 0.0f;
			if (((getheight)-testObject.y) < 0.25f && ((getheight)-testObject.y) > -0.25f) returnValue = 1.0f;
			returnValueAll[5]=(returnValue);
			
			// 6 direction
			if (suimonoObject != null){
				setDegrees = suimonoObject.flowDirection + suimonoObject.transform.eulerAngles.y;
				if (setDegrees < 0.0f) setDegrees = 365.0f + setDegrees;
				if (setDegrees > 365.0f) setDegrees = setDegrees-365.0f;
				if (suimonoObject != null) returnValueAll[6] = setDegrees;
				if (suimonoObject == null) returnValueAll[6] = 0.0f;
				
				// 7 speed
				if (suimonoObject != null) returnValueAll[7] = (suimonoObject.flowSpeed);
				if (suimonoObject == null) returnValueAll[7] = 0.0f;
				
				// 8 wave height
				if (suimonoObject != null) h1 = (suimonoObject.lgWaveHeight);
				if (suimonoObject == null) h1 = 0.0f;
				returnValueAll[8]=(getheight/h1);
			}
			
			// 9 transition depth
			returnValueAll[9] = ((getheight)-(testObject.y-(transition_offset*underTrans)));

			// 10 enabled Underwater FX
			enabledUFX = 1f;
			if (suimonoObject != null){
				if (!suimonoObject.enableUnderwater) enabledUFX = 0f;
				returnValueAll[10] = enabledUFX;
			}
			// 11 enabled Underwater FX
			enabledCaustics = 1f;
			if (suimonoObject != null){
				if (!suimonoObject.enableCausticFX) enabledCaustics = 0f;
				returnValueAll[11] = enabledCaustics;
			}
			
			return returnValueAll;
		}





		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles){
			dir = point - pivot;
			dir = Quaternion.Euler(angles * -1f) * dir;
			point = dir + pivot;
			return point;
		}






		public Color DecodeHeightPixels(float texPosx, float texPosy, int texNum){

			if (texNum == 0){
				useDecodeTex = heightTex;
				useDecodeArray = pixelArray;
			}
			if (texNum == 1){
				useDecodeTex = heightTexT;
				useDecodeArray = pixelArrayT;
			}
			if (texNum == 2){
				useDecodeTex = heightTexR;
				useDecodeArray = pixelArrayR;
			}

			texPosx = (texPosx % useDecodeTex.width);
			texPosy = (texPosy % useDecodeTex.height);
			if (texPosx < 0f) texPosx = useDecodeTex.width - Mathf.Abs(texPosx);
			if (texPosy < 0f) texPosy = useDecodeTex.height - Mathf.Abs(texPosy);
			if (texPosx > useDecodeTex.width) texPosx = texPosx - useDecodeTex.width;
			if (texPosy > useDecodeTex.height) texPosy = texPosy - useDecodeTex.height;

			row = (useDecodeArray.Length/useDecodeTex.height) - Mathf.FloorToInt(texPosy);
			pixIndex = ((Mathf.FloorToInt(texPosx) + 1) + (useDecodeArray.Length - (useDecodeTex.width * row))) - 1;
			if (pixIndex > useDecodeArray.Length) pixIndex = pixIndex - (useDecodeArray.Length);
			if (pixIndex < 0) pixIndex = useDecodeArray.Length - pixIndex;

			pixCol = useDecodeArray[pixIndex];

			if (_colorspace == ColorSpace.Linear){
				pixCol.a = Mathf.Clamp(Mathf.Lerp(-0.035f, 0.5f, pixCol.a ), 0f, 1f);
			}
			if (_colorspace == ColorSpace.Gamma){
				pixCol.a = Mathf.Clamp(Mathf.Lerp(-0.035f, 0.5f, pixCol.a ), 0f, 1f);
			}

			return pixCol;
		}








		void CalculateHeights(Vector3 testObject){

			getheight = -1.0f;
			getheightC = -1.0f;
			getheightT = -1.0f;
			getheightR = 0.0f;
			isOverWater = false;
			surfaceLevel = -1.0f;

			layermask = 1 <<layerWaterNum;
			testpos = new Vector3(testObject.x,testObject.y+30000f,testObject.z);

			if(Physics.Raycast(testpos, -Vector3.up, out hit, 60000f, layermask)){

				targetSurface = hit.transform.gameObject;
				if (currentSurfaceObject != targetSurface || suimonoObject == null){
					currentSurfaceObject = targetSurface;
					if (hit.transform.parent != null && hit.transform.parent.gameObject.GetComponent<Suimono.Core.SuimonoObject>() != null){
						suimonoObject = hit.transform.parent.gameObject.GetComponent<Suimono.Core.SuimonoObject>();
					}
				}

				if (suimonoObject != null){
				if (enableInteraction && suimonoObject.enableInteraction){
					

					if (suimonoObject.typeIndex == 0){
						heightObject = hit.transform;
					} else {
						heightObject = hit.transform.parent;
					}

					if (suimonoObject != null && hit.collider != null){

						isOverWater = true;
						surfaceLevel = heightObject.position.y;

						//calculate relative position
						if (heightObject != null){
							baseHeight = heightObject.position.y;
							baseAngle = heightObject.rotation.y;
							relativePos.x = ((heightObject.position.x - testObject.x)/(20.0f*heightObject.localScale.x) + 1f) * 0.5f * heightObject.localScale.x;
							relativePos.y = ((heightObject.position.z - testObject.z)/(20.0f*heightObject.localScale.z) + 1f) * 0.5f * heightObject.localScale.z;
						}

						//Custom UV Scaling
						float useXScale = suimonoObject.enableCustomMesh ? suimonoObject.cmScaleX : 1f;
						float useYScale = suimonoObject.enableCustomMesh ? suimonoObject.cmScaleY : 1f;
						relativePos.x = relativePos.x * useXScale;
						relativePos.y = relativePos.y * useYScale;

						//calculate offset
						useLocalTime = suimonoObject.localTime;
						flow_dirC = SuimonoConvertAngleToVector(suimonoObject.flowDirection);
						flowSpeed0 = new Vector2(flow_dirC.x*useLocalTime,flow_dirC.y*useLocalTime);
						flowSpeed1 = new Vector2(flow_dirC.x*useLocalTime*0.25f,flow_dirC.y*useLocalTime*0.25f);
						flowSpeed2 = new Vector2(flow_dirC.x*useLocalTime*0.0625f,flow_dirC.y*useLocalTime*0.0625f);
						flowSpeed3 = new Vector2(flow_dirC.x*useLocalTime*0.125f,flow_dirC.y*useLocalTime*0.125f);
						tScale = (1.0f/(suimonoObject.waveScale));
						oPos = new Vector2(0.0f-suimonoObject.savePos.x,0.0f-suimonoObject.savePos.y);

						//calculate texture coordinates
						if (heightTex != null){

							texCoord.x = (relativePos.x * tScale + flowSpeed0.x + (oPos.x)) * heightTex.width;
							texCoord.z = (relativePos.y * tScale + flowSpeed0.y + (oPos.y)) * heightTex.height;
							texCoord1.x = ((relativePos.x * tScale * 0.75f) - flowSpeed1.x + (oPos.x*0.75f)) * heightTex.width;
							texCoord1.z = ((relativePos.y * tScale * 0.75f) - flowSpeed1.y + (oPos.y*0.75f)) * heightTex.height;

							texCoordT.x = (relativePos.x * tScale + flowSpeed0.x + (oPos.x)) * heightTexT.width;
							texCoordT.z = (relativePos.y * tScale + flowSpeed0.y + (oPos.y)) * heightTexT.height;
							texCoordT1.x = ((relativePos.x * tScale * 0.5f) - flowSpeed1.x + (oPos.x*0.5f)) * heightTexT.width;
							texCoordT1.z = ((relativePos.y * tScale * 0.5f) - flowSpeed1.y + (oPos.y*0.5f)) * heightTexT.height;

							texCoordR.x = (relativePos.x * suimonoObject.lgWaveScale * tScale + flowSpeed2.x + (oPos.x*suimonoObject.lgWaveScale)) * heightTexR.width;
							texCoordR.z = (relativePos.y * suimonoObject.lgWaveScale * tScale + flowSpeed2.y + (oPos.y*suimonoObject.lgWaveScale)) * heightTexR.height;
							texCoordR1.x = ((relativePos.x * suimonoObject.lgWaveScale * tScale) + flowSpeed3.x + (oPos.x*suimonoObject.lgWaveScale)) * heightTexR.width;
							texCoordR1.z = ((relativePos.y * suimonoObject.lgWaveScale * tScale ) + flowSpeed3.y + (oPos.y*suimonoObject.lgWaveScale)) * heightTexR.height;

							//rotate coordinates
							if (baseAngle != 0.0f){
					    	
					    		pivotPoint = new Vector3(heightTex.width*heightObject.localScale.x*tScale*0.5f+(flowSpeed0.x*heightTex.width),0f,heightTex.height*heightObject.localScale.z*tScale*0.5f+(flowSpeed0.y*heightTex.height));
					    		texCoord = RotatePointAroundPivot(texCoord,pivotPoint,heightObject.eulerAngles);
					    		pivotPoint = new Vector3(heightTex.width*heightObject.localScale.x*tScale*0.5f*0.75f-(flowSpeed1.x*heightTex.width),0f,heightTex.height*heightObject.localScale.z*tScale*0.5f*0.75f-(flowSpeed1.y*heightTex.height));
					    		texCoord1 = RotatePointAroundPivot(texCoord1,pivotPoint,heightObject.eulerAngles);

					    		pivotPoint = new Vector3(heightTexT.width*heightObject.localScale.x*tScale*0.5f+(flowSpeed0.x*heightTexT.width),0f,heightTexT.height*heightObject.localScale.z*tScale*0.5f+(flowSpeed0.y*heightTexT.height));
					    		texCoordT = RotatePointAroundPivot(texCoordT,pivotPoint,heightObject.eulerAngles);
					    		pivotPoint = new Vector3(heightTexT.width*heightObject.localScale.x*tScale*0.5f*0.5f-(flowSpeed1.x*heightTexT.width),0f,heightTexT.height*heightObject.localScale.z*tScale*0.5f*0.5f-(flowSpeed1.y*heightTexT.height));
					    		texCoordT1 = RotatePointAroundPivot(texCoordT1,pivotPoint,heightObject.eulerAngles);

						    	pivotPoint = new Vector3(heightTexR.width*heightObject.localScale.x*suimonoObject.lgWaveScale*tScale*0.5f+(flowSpeed2.x*heightTexR.width),0f,heightTexR.height*heightObject.localScale.z*suimonoObject.lgWaveScale*tScale*0.5f+(flowSpeed2.y*heightTexR.height));
					    		texCoordR = RotatePointAroundPivot(texCoordR,pivotPoint,heightObject.eulerAngles);
					    		pivotPoint = new Vector3(heightTexR.width*heightObject.localScale.x*suimonoObject.lgWaveScale*tScale*0.5f+(flowSpeed3.x*heightTexR.width),0f,heightTexR.height*heightObject.localScale.z*suimonoObject.lgWaveScale*tScale*0.5f+(flowSpeed3.y*heightTexR.height));
					    		texCoordR1 = RotatePointAroundPivot(texCoordR1,pivotPoint,heightObject.eulerAngles);
					    	}

							//decode height value
							heightVal0 = DecodeHeightPixels(texCoord.x,texCoord.z,0);
							heightVal1 = DecodeHeightPixels(texCoord1.x,texCoord1.z,0);
							heightValT0 = DecodeHeightPixels(texCoordT.x,texCoordT.z,1);
							heightValT1 = DecodeHeightPixels(texCoordT1.x,texCoordT1.z,1);
							heightValR0 = DecodeHeightPixels(texCoordR.x,texCoordR.z,2);
							heightValR1 = DecodeHeightPixels(texCoordR1.x,texCoordR1.z,2);

							//set heightvalue
							getheightC = (heightVal0.a + heightVal1.a) * 0.8f;
							getheightT = ((heightValT0.a*0.2f) + (heightValT0.a * heightValT1.a * 0.8f)) * suimonoObject.turbulenceFactor * 0.5f;
							getheightR = ((heightValR0.a * 4.0f) + (heightValR1.a * 3.0f));

							getheight = baseHeight + (getheightC * suimonoObject.waveHeight);
							getheight += (getheightT * suimonoObject.waveHeight);
							getheight += (getheightR * suimonoObject.lgWaveHeight);
							getheight = Mathf.Lerp(baseHeight,getheight,suimonoObject.useHeightProjection);
						}
					}
				}
				}
			}
		}



	    public void RegisterSurface(SuimonoObject surface)
	    {
	    	if (Application.isPlaying){
		        if (surface != null && sObjects != null){
			        // If IndexOf() returns a valid index, the surface was already registered before and we can break
			        if (sObjects.IndexOf(surface) > -1) return;

		       		sObjects.Add(surface);
		        	sRends.Add(surface.transform.Find("Suimono_Object").gameObject.GetComponent<Renderer>());
		        	sRendSCs.Add( surface.transform.Find("Suimono_ObjectScale").gameObject.GetComponent<Renderer>() );
		    	}
	    	}
        }

	    public void DeregisterSurface(SuimonoObject surface)
	    {
	    	if (Application.isPlaying){
		    	if (surface != null && sObjects != null){
			        int surfaceIndex = sObjects.IndexOf(surface);
		            // If IndexOf() returns -1, the surface wasn't registered before and we can break
			        if (surfaceIndex < 0) return;

		            sObjects.RemoveAt(surfaceIndex);
			        sRends.RemoveAt(surfaceIndex);
			        sRendSCs.RemoveAt(surfaceIndex);
		    	}
	    	}
	    }

/*
		//Check for visibility of all Suimono Surfaces in Scene
		void InitSurfaceRenderers(){
			sObjects = FindObjectsOfType(typeof(Suimono.Core.SuimonoObject)) as Suimono.Core.SuimonoObject[];
       		sRends = new List<Renderer>();
       		sRendSCs = new List<Renderer>();

       		for (int s = 0; s < sObjects.Length; s++){
       			sRends.Add( sObjects[s].transform.Find("Suimono_Object").gameObject.GetComponent<Renderer>() );
       			sRendSCs.Add( sObjects[s].transform.Find("Suimono_ObjectScale").gameObject.GetComponent<Renderer>() );
       		}
		}
*/

/*
		//Checks if surface objects are visible and disables module functions accordingly.
		void SetCullFunction(){

			renderCount = 0;

			//Update surface count every 5 seconds
			surfaceTimer += _deltaTime;
			if (surfaceTimer > 5f){
				surfaceTimer = 0f;
				InitSurfaceRenderers();
			}

			//check for visibility
       		for (int sX = 0; sX < sRends.Count; sX++){
       			if (sRends[sX] != null){
        	    if (sRends[sX].isVisible){
        	    	if (sObjects[sX].typeIndex == 0){
        	    		if (sRendSCs[sX].isVisible){
        	    			renderCount++;
        	    		}
        	    	} else {
						renderCount++;
					}
				}
				}
       		}

       		//Enable Functions
       		if (renderCount > 0 || isUnderwater){
       			useEnableTransparency = enableTransparency;
			}

			//Disable Functions
       		if (renderCount <= 0 && !isUnderwater){
       			useEnableTransparency = false;
			}	


		}
*/

        //Checks if surface objects are visible and disables module functions accordingly.
        void SetCullFunction()
        {

			renderCount = 0;

			//Update surface count every 5 seconds
			//surfaceTimer += _deltaTime;
			//if (surfaceTimer > 5f){
			//	surfaceTimer = 0f;
			//	InitSurfaceRenderers();
			//}

			//check for visibility
       		for (int sX = 0; sX < sObjects.Count; sX++)
		    {
                if (sRends[sX] == null || !sRends[sX].isVisible) continue;

                if (sObjects[sX].typeIndex == 0)
                {
		            if (sRendSCs[sX] != null && sRendSCs[sX].isVisible) { renderCount++; }
		        }
                else { renderCount++; }
		    }

       		//Enable Functions
       		if (renderCount > 0 || isUnderwater){
       			useEnableTransparency = enableTransparency;
			}

			//Disable Functions
       		if (renderCount <= 0 && !isUnderwater){
       			useEnableTransparency = false;
			}


		}




		void InstallCameraEffect(){

			//Installs Camera effect if it doesn't already exist.
			if (setCameraComponent != null && autoSetCameraFX){
				if (setCameraComponent.gameObject.GetComponent<Suimono.Core.Suimono_UnderwaterFog>() != null){
					//do nothing
				} else {
					if (enableUnderwaterFX){
						setCameraComponent.gameObject.AddComponent<Suimono.Core.Suimono_UnderwaterFog>();
					}
				}
			}
		}



	}
}