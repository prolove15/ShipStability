using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
	using UnityEditor;
#endif



namespace Suimono.Core
{

	[ExecuteInEditMode]
	public class SuimonoObject : MonoBehaviour {


		public float systemTime = 0.0f;
		public float systemLocalTime = 0.0f;
		public float flowSpeed = 0.1f;
		public float flowDirection = 180.0f;
		public bool useBeaufortScale = false;
		public float beaufortScale = 1.0f;
		public float turbulenceFactor = 1.0f;
		public float waveScale = 0.5f;
		public float lgWaveHeight = 0.0f;
		public float lgWaveScale = 1.0f;
		public float waveHeight = 1.0f;
		public float heightProjection = 1.0f;
		public float useHeightProjection = 1.0f;
		public float refractStrength = 1.0f;
		public float reflectProjection = 1.0f;
		public float reflectBlur = 0.0f;
		public float aberrationScale = 0.1f;
		public float specularPower = 1f;
		public float roughness = 0.1f;
		public float roughness2 = 0.35f;
		public float reflectTerm = 0.0255f;
		public float reflectSharpen = 0.0f;
		public bool showDepthMask = false;
		public bool showWorldMask = false;
		public float cameraDistance = 1000.0f;
		public float underwaterDepth = 5f;

		//DX9 version
		public bool useDX9Settings = false;


		//objects
		public Suimono.Core.SuimonoModule moduleObject;
		private Suimono.Core.SuimonoModuleLib suimonoModuleLibrary;
		private GameObject suimonoObject;
		private Renderer surfaceRenderer;
		private MeshFilter surfaceMesh;
		private MeshCollider surfaceCollider;
		private Suimono.Core.cameraTools surfaceReflections;
		private Suimono.Core.Suimono_DistanceBlur surfaceReflBlur;
		private GameObject scaleObject;
		private Renderer scaleRenderer;
		private MeshCollider scaleCollider;
		private MeshFilter scaleMesh;
		private Renderer surfaceVolume;

		//materials
		private Material tempMaterial;

		//type and options
		public string suimonoVersionNumber;
		public bool showGeneral = false;
		public int typeIndex = 1;
		[System.NonSerialized] public List<string> typeOptions = new List<string>(){"Infinite 3D Ocean", "3D Waves", "Flat Plane"};
		public int editorIndex = 1;
		public int editorUseIndex = 1;
		[System.NonSerialized] public List<string> editorOptions = new List<string>(){"Simple","Advanced"};


		//mesh and lod
		public bool enableCustomMesh = false;
		public float cmScaleX = 1f;
		public float cmScaleY = 1f;
		public int lodIndex;
		public int useLodIndex;
		[System.NonSerialized] public List<string> lodOptions = new List<string>(){"High Detail","Medium Detail","Low Detail","Single Quad"};
		public Mesh customMesh;
		public float oceanScale = 2.0f;
		private bool meshWasSet = false;

		//casutics
		public bool enableCausticFX = true;
		public float causticsFade = 0.55f;
		public Color causticsColor = new Color(1f,1f,1f,1f);

		//tessellation
		public bool enableTess = true;
		public bool useEnableTess = true;
		public float waveTessAmt = 8.0f;
		public float waveTessMin = 0.0f;
		public float waveTessSpread = 0.08f;

		//interaction
		public bool enableInteraction = true;

		//reflections
		public float dynamicReflectFlag = 1.0f;
		public bool enableReflections = true;
		public bool enableDynamicReflections = true;

		public bool useEnableReflections = true;
		public bool useEnableDynamicReflections = true;

		public bool useReflections = true;
		public bool useDynReflections = true;
		public int reflectLayer = 0;
		public int reflectResolution = 4;
		public LayerMask reflectLayerMask;
		public float reflectionDistance = 1000.0f;

		[System.NonSerialized] public List<string> suiLayerMasks = new List<string>();

		[System.NonSerialized] public List<string> resOptions = new List<string>(){"4096","2048","1024","512","256","128","64","32","16","8"};
		[System.NonSerialized] public List<int> resolutions = new List<int>(){4096,2048,1024,512,256,128,64,32,16,8};
		public int reflectFallback = 1;
		[System.NonSerialized] public List<string> resFallbackOptions = new List<string>(){"None","skybox","Custom Cubemap","Color"};
		public Texture customRefCubemap;
		public Color customRefColor = new Color(0.9f,0.9f,0.9f,1.0f);
		public Color reflectionColor = new Color(1f,1f,1f,1f);

		//custom textures
		public bool enableCustomTextures = false;
		public Texture2D customTexNormal1;
		//public Texture2D customTexHeight1;
		public Texture2D customTexNormal2;
		//public Texture2D customTexHeight2;
		public Texture2D customTexNormal3;
		//public Texture2D customTexHeight3;
		public Texture2D useTexNormal1;
		//public Texture2D useTexHeight1;
		public Texture2D useTexNormal2;
		//public Texture2D useTexHeight2;
		public Texture2D useTexNormal3;
		//public Texture2D useTexHeight3;

		//waves
		public bool showWaves = false;
		public bool customWaves = false;
		public float localTime = 0.0f;
		private Vector2 flow_dir = new Vector2(0f,0f);
		private Vector3 tempAngle;
		public float beaufortVal = 1.0f;

		//shorelines
		public bool showShore = false;
		public float shorelineHeight = 0.75f;
		public float shorelineFreq = 0.5f;
		public float shorelineScale = 0.15f;
		public float shorelineSpeed = 2.5f;
		public float shorelineNorm = 0.5f;

		//surface
		public bool showSurface = false;
		public float overallBright = 1.0f;
		public float overallTransparency = 1.0f;
		public float depthAmt = 0.1f;
		public float shallowAmt = 0.1f;
		public Color depthColor;
		public Color shallowColor;
		public float edgeAmt = 0.1f;
		public Color specularColor;
		public Color sssColor;
		public Color blendColor;
		public Color overlayColor;

		//foam
		public bool showFoam = false;
		public bool enableFoam = true;
		public Color foamColor = new Color(0.9f,0.9f,0.9f,1.0f);
		public float foamScale = 40.0f;
		public float foamSpeed = 0.1f;
		public float edgeFoamAmt = 0.5f;
		public float shallowFoamAmt = 1.0f;
		public float hFoamHeight = 1.0f;
		public float hFoamSpread = 1.0f;
		public float heightFoamAmt = 0.5f;

		//underwater 
		public bool showUnderwater = false;
		public Color underwaterColor = new Color(1f,0f,0f,1f);
		public float underLightFactor = 1.0f;
		public float underRefractionAmount = 0.005f;
		public float underRefractionScale = 1.5f;
		public float underRefractionSpeed = 0.5f;
		public float underwaterFogDist = 20.0f;
		public float underwaterFogSpread = 0.0f;
		public bool enableUnderwater = true;
		public bool enableUnderDebris = false;
		public float underBlurAmount = 1.0f;
		public float underDarkRange = 40.0f;

		//scaling
		public float setScale = 1.0f;
		public Vector3 currentAngles = new Vector3(0f,0f,0f);
		public Vector3 currentPosition = new Vector3(0f,0f,0f);
		public Vector3 newPos = new Vector3(0f,0f,0f);
		public float spacer = 0.0f;
		public float setScaleX = 0.0f;
		public float setScaleZ = 0.0f;
		public float offamt = 0.0f;
		public Vector2 savePos = new Vector2(0f,0f);
		public Vector2 recPos = new Vector2(0f,0f);
		public Vector2 _suimono_uv = new Vector2(0f,0f);

		//editor
		public bool showSimpleEditor = false;

		//shaders
		public Shader useShader;
		public Shader currUseShader;
		public Shader shader_Surface;
		public Shader shader_Scale;
		public Shader shader_Under;

		//presets
		[System.NonSerialized] public List<string> presetDirs;
		public string[] presetFiles;
		public int presetIndex = -1;
		public int presetUseIndex = -1;
		public int presetFileIndex = 0;
		public int presetFileUseIndex = 0;
		public string[] presetOptions;
		public bool showPresets = false;
		public bool presetStartTransition = false;
		public float presetTimer = 1f;
		public string currentPresetFolder = "Built-In Presets";
		public string currentPresetName = "";
		public int presetTransitionCurrent = 0;
		public float presetTransitionTime = 1.0f;
		public int presetTransIndexFrm = 0;
		public int presetTransIndexTo = 0;
		public bool presetToggleSave = false;
		public bool presetsLoaded = false;
		public string[] presetDataArray;
		public string presetDataString;
		public string dir = "";
		public string baseDir = "SUIMONO - WATER SYSTEM 2/RESOURCES/";
		public string presetSaveName = "my custom preset";
		public string presetFile = "";
		public string workData;
		public string workData2;


		//temporary preset data
		Color temp_depthColor;
		Color temp_shallowColor;
		Color temp_blendColor;
		Color temp_overlayColor;
		Color temp_causticsColor;
		Color temp_reflectionColor;
		Color temp_specularColor;
		Color temp_sssColor;
		Color temp_foamColor;
		Color temp_underwaterColor;
		float temp_beaufortScale;
		float temp_flowDirection;
		float temp_flowSpeed;
		float temp_waveScale;
		float temp_waveHeight;
		float temp_heightProjection;
		float temp_turbulenceFactor;
		float temp_lgWaveHeight;
		float temp_lgWaveScale;
		float temp_shorelineHeight;
		float temp_shorelineFreq;
		float temp_shorelineScale;
		float temp_shorelineSpeed;
		float temp_shorelineNorm;
		float temp_overallBright;
		float temp_overallTransparency;
		float temp_edgeAmt;
		float temp_depthAmt;
		float temp_shallowAmt;
		float temp_refractStrength;
		float temp_aberrationScale;
		float temp_causticsFade;
		float temp_reflectProjection;
		float temp_reflectBlur;
		float temp_reflectTerm;
		float temp_reflectSharpen;
		float temp_roughness;
		float temp_roughness2;
		float temp_foamScale;
		float temp_foamSpeed;
		float temp_edgeFoamAmt;
		float temp_shallowFoamAmt;
		float temp_heightFoamAmt;
		float temp_hFoamHeight;
		float temp_hFoamSpread;
		float temp_underLightFactor;
		float temp_underRefractionAmount;
		float temp_underRefractionScale;
		float temp_underRefractionSpeed;
		float temp_underBlurAmount;
		float temp_underwaterFogDist;
		float temp_underwaterFogSpread;
		float temp_underDarkRange;


		public string materialPath;
		public float oceanUseScale;
		public float useSc;
		public Vector2 setSc;
		public Vector2 scaleOff;
		public int i;
		public string layerName;
		public Material skybox;

		[System.NonSerialized] public List<string> presetDirsArr = new List<string>();
		public int d;
		public int dn;
		[System.NonSerialized] public List<string> presetFilesArr = new List<string>();
		public string pdir;
		public FileInfo[] fileInfo;
		public int f = 0;
		public int px = 0;
		public int nx = 0;
		public int ax = 0;
		public int n = 0;

		[System.NonSerialized] public List<string> tempPresetDirsArr = new List<string>();
		public FileInfo[] dirInfo;
		public string[] tempPresetDirs;
		[System.NonSerialized] public List<string> tempPresetFilesArr = new List<string>();
		public string[] tempPresetFiles;
		public string oldName;
		public string moveName;
		public int setNum;

		public StreamWriter sw;
		public StreamReader sr;
		public string key;
		public string dat;
		public int pFrom;
		public int pTo;
		public int dx;
		public TextAsset datFile;
		public string[] dataS;
		public string retData;
		public bool retVal;

		private float suimono_refl_off = 0.0f;
		private float suimono_refl_sky = 0.0f;
		private float suimono_refl_cube = 0.0f;
		private float suimono_refl_color = 0.0f;

		private static bool reloadData = false;


		void Start(){

			//DISCONNECT FROM PREFAB
			#if UNITY_EDITOR
			#if !UNITY_2018_3_OR_NEWER
			PrefabUtility.DisconnectPrefabInstance(this.gameObject);
			#endif
			#endif


			//get Suimono objects
			if (GameObject.Find("SUIMONO_Module") != null){

				moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
				//moduleObject = GameObject.Find("SUIMONO_Module").GetComponent<Suimono.Core.SuimonoModule>() as Suimono.Core.SuimonoModule;
				if (moduleObject != null) suimonoModuleLibrary = moduleObject.GetComponent<Suimono.Core.SuimonoModuleLib>() as Suimono.Core.SuimonoModuleLib;
			}



			//SET PRESET DIRECTORIES
			#if UNITY_EDITOR

				// Original Find Resources
				baseDir = "SUIMONO - WATER SYSTEM 2/RESOURCES/";

				// Find Presets regardless of moved directory
				/*
				string basePath = AssetDatabase.GetAssetPath(moduleObject.suimonoModuleLibrary.surfaceObject);
				basePath = basePath.Replace("Assets/", "");
				string[] splitstring = basePath.Split(new string[] {"/PREFABS"}, System.StringSplitOptions.None);
				baseDir = splitstring[0] + "/RESOURCES/";
				*/

			#else
				baseDir = "Resources/";
			#endif

			dir = Path.Combine(Application.dataPath, baseDir);



			//get surface objects
			suimonoObject = this.transform.Find("Suimono_Object").gameObject;
			surfaceRenderer = transform.Find("Suimono_Object").gameObject.GetComponent<Renderer>();
			surfaceMesh = transform.Find("Suimono_Object").GetComponent<MeshFilter>();
			surfaceCollider = transform.Find("Suimono_Object").GetComponent<MeshCollider>();
			surfaceReflections = transform.Find("cam_LocalReflections").gameObject.GetComponent<Suimono.Core.cameraTools>() as Suimono.Core.cameraTools;
			surfaceReflBlur = transform.Find("cam_LocalReflections").gameObject.GetComponent<Suimono.Core.Suimono_DistanceBlur>() as Suimono.Core.Suimono_DistanceBlur;

			//get scale object (infinite ocean)
			scaleObject = this.transform.Find("Suimono_ObjectScale").gameObject;
			scaleRenderer = transform.Find("Suimono_ObjectScale").gameObject.GetComponent<Renderer>();
			scaleCollider = transform.Find("Suimono_ObjectScale").gameObject.GetComponent<MeshCollider>();
			if (scaleCollider == null){
				scaleCollider = transform.Find("Suimono_ObjectScale").gameObject.AddComponent<MeshCollider>();
			}
			scaleMesh = transform.Find("Suimono_ObjectScale").GetComponent<MeshFilter>();

			//Store Shader References
			shader_Surface = Shader.Find("Suimono2/surface");
			shader_Scale = Shader.Find("Suimono2/surface_scale");
			shader_Under = Shader.Find("Suimono2/surface_under");

			//save material if not already saved
			//#if UNITY_EDITOR
				//materialPath = "Assets/SUIMONO - WATER SYSTEM 2/Resources/mat_" + this.gameObject.name + ".mat";
				//if (AssetDatabase.LoadAssetAtPath(materialPath,Material) == null){
				//	tempMaterial = new Material(suimonoObject.GetComponent<Renderer>().sharedMaterial);
				//	AssetDatabase.CreateAsset(tempMaterial, materialPath);
				//}
				//tempMaterial = AssetDatabase.LoadAssetAtPath(materialPath,Material);
			//#else
				//tempMaterial = new Material(suimonoObject.GetComponent<Renderer>().sharedMaterial);
				tempMaterial = new Material(suimonoModuleLibrary.materialSurface);
			//#endif

			//setup custom material surface
			if (suimonoObject != null){
				tempMaterial.shader = shader_Surface;
				suimonoObject.GetComponent<Renderer>().sharedMaterial = tempMaterial;
				surfaceRenderer = transform.Find("Suimono_Object").gameObject.GetComponent<Renderer>();
			}

			//Load initial scene data
			ReloadData();
		}




		#if UNITY_EDITOR
			//Check for scene reload and set flag
			[UnityEditor.Callbacks.DidReloadScripts]
			private static void ReloadDataFlag(){
				reloadData = true;
			}
		#endif


	    void OnEnable()
	    {
	    	if (Application.isPlaying){
		    	if (moduleObject == null){
		    		moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
		    	}
		    	if (moduleObject != null){
		        	moduleObject.RegisterSurface(this);
		    	}
	    	}
	    }

	    void OnDisable()
	    {
	    	if (Application.isPlaying){
		    	if (moduleObject == null){
		    		moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
		    	}
		    	if (moduleObject != null){
		        	moduleObject.DeregisterSurface(this);
		    	}
	    	}
	    }


		void ReloadData(){
			reloadData = false;
			
			//Set Layer Masks
			#if UNITY_EDITOR
				suiLayerMasks = new List<string>();
				for (i = 0; i < 32; i++){
					layerName = LayerMask.LayerToName(i);
					suiLayerMasks.Add(layerName);
				}
			#endif
			
			//Init Presets
			#if !UNITY_WEBPLAYER && UNITY_EDITOR
				PresetInit();
				PresetLoad(presetIndex);
			#endif

		}



		void LateUpdate () {

			if (moduleObject != null){

				//-------------------------------------------------------
				//###  SET SUIMONO VERSION  ###
				//-------------------------------------------------------
				//inherit suimono version number from module object for display in UI elements
				suimonoVersionNumber = moduleObject.suimonoVersionNumber;


				//-------------------------------------------------------
				//###  RELOAD TEMPORARY DATA  ###
				//-------------------------------------------------------
				if (reloadData) ReloadData();


				//-------------------------------------------------------
				//###  SET LOCAL TIME AND DIRECTION  ###
				//-------------------------------------------------------
				systemLocalTime = moduleObject.systemTime;
				localTime = systemLocalTime * flowSpeed * (1.0f/waveScale);
				flow_dir = SuimonoConvertAngleToVector(flowDirection);

				if (surfaceRenderer != null){
					surfaceRenderer.sharedMaterial.SetVector("_suimono_Dir",new Vector4(flow_dir.x,1.0f,flow_dir.y,localTime));
				}
				
				//-------------------------------------------------------
				//###  SET LAYER MASK  ###
				//-------------------------------------------------------
				if (moduleObject.autoSetLayers){
					gameObject.layer = moduleObject.layerWaterNum;
					if (suimonoObject != null) suimonoObject.layer = moduleObject.layerWaterNum;
					if (scaleObject != null) scaleObject.layer = moduleObject.layerWaterNum;
					if (surfaceReflections != null) surfaceReflections.gameObject.layer = moduleObject.layerWaterNum; 
				}

				//-------------------------------------------------------
				//###  FORCE SIZING  ###
				//-------------------------------------------------------
				if (underwaterDepth < 0.1f) underwaterDepth = 0.1f;
				if (!enableCustomMesh){
					transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
				}
				if (typeIndex == 0){
					suimonoObject.transform.localScale = new Vector3(suimonoObject.transform.localScale.x, 1.0f, suimonoObject.transform.localScale.z);
					scaleObject.transform.localScale = new Vector3(scaleObject.transform.localScale.x, 1.0f, scaleObject.transform.localScale.z);
					surfaceReflections.transform.localScale = new Vector3(surfaceReflections.transform.localScale.x, 1.0f, surfaceReflections.transform.localScale.z);
				}

				//-------------------------------------------------------
				//###  CALCULATE BEAUFORT SCALE  ###
				//-------------------------------------------------------
				useBeaufortScale = !customWaves;
				if (useBeaufortScale){
					beaufortVal = beaufortScale/12.0f;
					turbulenceFactor = Mathf.Clamp(Mathf.Lerp(-0.1f,2.1f,beaufortVal)*0.9f,0.0f,0.75f);
					waveHeight = Mathf.Clamp(Mathf.Lerp(0.0f,5.0f,beaufortVal),0.0f,0.65f);
					waveHeight = waveHeight - Mathf.Clamp(Mathf.Lerp(-1.5f,1.0f,beaufortVal),0.0f,0.5f);
					lgWaveHeight = Mathf.Clamp(Mathf.Lerp(-0.2f,1.1f,beaufortVal)*2.8f,0.0f,3.0f);

					//freeze scale for ocean
					if (typeIndex == 0){
						waveScale = 0.5f;
						lgWaveScale = 0.03125f;
					}
				}


				//-------------------------------------------------------
				//###  LOAD PRESET CHANGES  ###
				//-------------------------------------------------------
				#if UNITY_EDITOR
					// detect changes to preset and run update when applicable
					if (presetUseIndex != presetIndex){
						presetUseIndex = presetIndex;
						PresetLoad(presetIndex);
					}
					// reload preset settings when editor mode is switched between simple and advanced.
					if (editorUseIndex != editorIndex){
						editorUseIndex = editorIndex;
						PresetLoad(presetIndex);
					}
				#endif



				//-------------------------------------------------------
				//###  HANDLE PRESET TRANSITIONS  ###
				//-------------------------------------------------------
				if (presetStartTransition){
					if (presetTimer >= 1.0f){
						presetStartTransition = false;
					} else {
						presetTimer += (Time.deltaTime / presetTransitionTime);
						PresetLoadBuild(currentPresetFolder,currentPresetName);
					}
				}




				//-------------------------------------------------------
				//###  SET MESH LOD LEVEL  ###
				//-------------------------------------------------------
				// change the surface mesh based on the selected level of detail
				if (typeIndex == 0) useLodIndex = 0; //infinite ocean requires high detail mesh
				if (typeIndex == 1) useLodIndex = lodIndex; //3d waves setting picks mesh set in UI
				if (typeIndex == 2) useLodIndex = 3; // flat surface forces single quad mesh
				
				if (typeIndex == 0) enableCustomMesh = false; // force custom mesh off when using infinite ocean
				if (enableCustomMesh == false){
					if (suimonoModuleLibrary && !meshWasSet){
						if (suimonoModuleLibrary.texNormalC && surfaceMesh != null) surfaceMesh.mesh = suimonoModuleLibrary.meshLevel[useLodIndex];	
						if (suimonoModuleLibrary.texNormalC && surfaceCollider != null) surfaceCollider.sharedMesh = suimonoModuleLibrary.meshLevel[3];
						meshWasSet = true;
					} else {
						meshWasSet = false;
					}
				} else {
					if (customMesh != null){
						if (surfaceMesh != null) surfaceMesh.mesh = customMesh;
						if (surfaceCollider != null) surfaceCollider.sharedMesh = customMesh;
					} else {
						if (suimonoModuleLibrary.texNormalC && surfaceMesh != null) surfaceMesh.mesh = suimonoModuleLibrary.meshLevel[useLodIndex];
						if (suimonoModuleLibrary.texNormalC && surfaceCollider != null) surfaceCollider.sharedMesh = suimonoModuleLibrary.meshLevel[3];
						meshWasSet = false;
					}
				}
				if (useLodIndex == 3){
					useHeightProjection = 0.0f;
					useEnableTess = false;
				} else {
					useHeightProjection = heightProjection;
					useEnableTess = enableTess;
				}

				//set mesh UV scale
				Shader.SetGlobalFloat("cmScaleX", enableCustomMesh ? cmScaleX : 1f);
				Shader.SetGlobalFloat("cmScaleY", enableCustomMesh ? cmScaleY : 1f);

				//set scale mesh
				if (suimonoModuleLibrary.texNormalC && scaleMesh != null) scaleMesh.mesh = suimonoModuleLibrary.meshLevel[1];	


				//-------------------------------------------------------
				//###  Set Custom Textures  ###
				//-------------------------------------------------------
				if (enableCustomTextures){
					if (customTexNormal1 != null){
						useTexNormal1 = customTexNormal1;
						} else {
							useTexNormal1 = suimonoModuleLibrary.texNormalC;
						}
					//if (customTexHeight1 != null){
					//	useTexHeight1 = customTexHeight1;
					//	} else {
					//		useTexHeight1 = suimonoModuleLibrary.texHeightC;
					//	}
					if (customTexNormal2 != null){
						useTexNormal2 = customTexNormal2;
						} else {
							useTexNormal2 = suimonoModuleLibrary.texNormalT;
						}
					//if (customTexHeight2 != null){
					//	useTexHeight2 = customTexHeight2;
					//	} else {
					//		useTexHeight2 = suimonoModuleLibrary.texHeightT;
					//	}
					if (customTexNormal3 != null){
						useTexNormal3 = customTexNormal3;
						} else {
							useTexNormal3 = suimonoModuleLibrary.texNormalR;
						}
					//if (customTexHeight3 != null){
					//	useTexHeight3 = customTexHeight3;
					//	} else {
					//		useTexHeight3 = suimonoModuleLibrary.texHeightR;
					//	}

				} else {
					if (suimonoModuleLibrary != null){
						useTexNormal1 = suimonoModuleLibrary.texNormalC;
						//useTexHeight1 = suimonoModuleLibrary.texHeightC;
						useTexNormal2 = suimonoModuleLibrary.texNormalT;
						//useTexHeight2 = suimonoModuleLibrary.texHeightT;
						useTexNormal3 = suimonoModuleLibrary.texNormalR;
						//useTexHeight3 = suimonoModuleLibrary.texHeightR;
					}
				}
				if (suimonoModuleLibrary != null){
					if (surfaceRenderer != null) surfaceRenderer.sharedMaterial.SetTexture("_MaskTex",suimonoModuleLibrary.texMask);
				}


				//-------------------------------------------------------
				//###  SET REFLECTION OBJECT PROPERTIES  ###
				//-------------------------------------------------------
				if (surfaceReflections != null && moduleObject != null){

					useEnableReflections = enableReflections;
					useEnableDynamicReflections = enableDynamicReflections;

					if (!moduleObject.enableReflections) useEnableReflections = false;
					if (!moduleObject.enableDynamicReflections) useEnableDynamicReflections = false;
					
					if (!useEnableReflections || !moduleObject.enableReflections){
						useReflections = false;
						surfaceReflections.gameObject.SetActive(false);
					} else {

						if (!useEnableDynamicReflections || !moduleObject.enableDynamicReflections){
							surfaceReflections.gameObject.SetActive(false);
						} else {
							surfaceReflections.gameObject.SetActive(true);
							useReflections = true;
							reflectLayer = (reflectLayer & ~(1 << moduleObject.layerWaterNum));
							reflectLayer = (reflectLayer & ~(1 << moduleObject.layerDepthNum));
							reflectLayer = (reflectLayer & ~(1 << moduleObject.layerScreenFXNum));

							surfaceReflections.setLayers = reflectLayer;
							surfaceReflections.resolution = System.Convert.ToInt32(resolutions[reflectResolution]);
							if (moduleObject.setCameraComponent != null){
								reflectionDistance = moduleObject.setCameraComponent.farClipPlane+200.0f;
							}
							surfaceReflections.reflectionDistance = reflectionDistance;

							//blur settings
							surfaceReflBlur.blurAmt = reflectBlur;

							if (useShader == shader_Under){
								surfaceReflections.isUnderwater = true;
							} else {
								surfaceReflections.isUnderwater = false;
							}
						}
					}
				}


				//-------------------------------------------------------
				//###  SEND SETTINGS TO SHADER  ###
				//-------------------------------------------------------
				if (surfaceRenderer != null){

					//set shader
					if (Application.isPlaying && useShader != null){
						if (currUseShader != useShader){
							currUseShader = useShader;
							surfaceRenderer.sharedMaterial.shader = currUseShader;
						}
					}

					//set playmode
					if (!Application.isPlaying){
						surfaceRenderer.sharedMaterial.SetFloat("_isPlaying",0.0f);
					} else {
						surfaceRenderer.sharedMaterial.SetFloat("_isPlaying",1.0f);
					}

					//set texture
					surfaceRenderer.sharedMaterial.SetTexture("_NormalTexS",useTexNormal1);
					surfaceRenderer.sharedMaterial.SetTexture("_NormalTexD",useTexNormal2);
					surfaceRenderer.sharedMaterial.SetTexture("_NormalTexR",useTexNormal3);

					//set beaufort and waves
					surfaceRenderer.sharedMaterial.SetFloat("_beaufortFlag",useBeaufortScale ? 1.0f : 0.0f);
					surfaceRenderer.sharedMaterial.SetFloat("_beaufortScale",beaufortVal);
					surfaceRenderer.sharedMaterial.SetFloat("_turbulenceFactor",turbulenceFactor);
				
					//custom UV scaling
					float useXScale = enableCustomMesh ? cmScaleX : 1f;
					float useYScale = enableCustomMesh ? cmScaleY : 1f;

					//set texture speed and scale
					surfaceRenderer.sharedMaterial.SetTextureScale("_NormalTexS",new Vector2(((suimonoObject.transform.localScale.x/waveScale)*transform.localScale.x) * useXScale,((suimonoObject.transform.localScale.z/waveScale)*transform.localScale.z)*useYScale));
					surfaceRenderer.sharedMaterial.SetVector("_scaleUVs",new Vector4((suimonoObject.transform.localScale.x/waveScale) * useXScale,(suimonoObject.transform.localScale.z/waveScale) * useYScale,0f,0f));
					surfaceRenderer.sharedMaterial.SetFloat("_lgWaveScale",lgWaveScale);
					surfaceRenderer.sharedMaterial.SetFloat("_lgWaveHeight",lgWaveHeight);
					
					//set tessellation settings
					if (typeIndex == 0){
						surfaceRenderer.sharedMaterial.SetFloat("_tessScale",suimonoObject.transform.localScale.x);
					} else {
						surfaceRenderer.sharedMaterial.SetFloat("_tessScale",transform.localScale.x);
					}
					surfaceRenderer.sharedMaterial.SetFloat("_Tess",Mathf.Lerp(0.001f,waveTessAmt,useEnableTess ? 1.0f : 0.0f));
					surfaceRenderer.sharedMaterial.SetFloat("_minDist", Mathf.Lerp(-180.0f,0.0f,waveTessMin));
					surfaceRenderer.sharedMaterial.SetFloat("_maxDist", Mathf.Lerp(20.0f,500.0f,waveTessSpread));

					//set system fog coordinates
					surfaceRenderer.sharedMaterial.SetFloat("_unity_fogstart",RenderSettings.fogStartDistance);
					surfaceRenderer.sharedMaterial.SetFloat("_unity_fogend",RenderSettings.fogEndDistance);

					//set caustics
					surfaceRenderer.sharedMaterial.SetFloat("_causticsFlag",enableCausticFX ? 1.0f : 0.0f);
					surfaceRenderer.sharedMaterial.SetFloat("_CausticsFade",Mathf.Lerp(1f,500f,causticsFade));
					surfaceRenderer.sharedMaterial.SetColor("_CausticsColor",causticsColor);

					//set aberration scale
					surfaceRenderer.sharedMaterial.SetFloat("_aberrationScale",aberrationScale);

					//set foam speed and scale
					surfaceRenderer.sharedMaterial.SetFloat("_enableFoam", enableFoam ? 1.0f : 0.0f );
					surfaceRenderer.sharedMaterial.SetFloat("_EdgeFoamFade",Mathf.Lerp(1500.0f,5.0f,edgeFoamAmt));
					surfaceRenderer.sharedMaterial.SetFloat("_HeightFoamAmt",heightFoamAmt);
					surfaceRenderer.sharedMaterial.SetFloat("_HeightFoamHeight",hFoamHeight);
					surfaceRenderer.sharedMaterial.SetFloat("_HeightFoamSpread",hFoamSpread);
					surfaceRenderer.sharedMaterial.SetFloat("_foamSpeed",foamSpeed);
					surfaceRenderer.sharedMaterial.SetTextureScale("_FoamTex",foamScale* new Vector2(((suimonoObject.transform.localScale.x/foamScale)*transform.localScale.x)*useXScale,((suimonoObject.transform.localScale.z/foamScale)*transform.localScale.z)*useYScale));
					surfaceRenderer.sharedMaterial.SetFloat("_foamScale",Mathf.Lerp(160.0f,1.0f,foamScale));
					surfaceRenderer.sharedMaterial.SetColor("_FoamColor",foamColor);
					surfaceRenderer.sharedMaterial.SetFloat("_ShallowFoamAmt",shallowFoamAmt);

					//set height and normal scales
					surfaceRenderer.sharedMaterial.SetFloat("_heightScaleFac",(1.0f/transform.localScale.y));
					surfaceRenderer.sharedMaterial.SetFloat("_heightProjection",useHeightProjection);
					surfaceRenderer.sharedMaterial.SetFloat("_heightScale",waveHeight);
					surfaceRenderer.sharedMaterial.SetFloat("_RefractStrength",refractStrength);
					surfaceRenderer.sharedMaterial.SetFloat("_ReflectStrength",reflectProjection);

					//set shoreline properties
					surfaceRenderer.sharedMaterial.SetFloat("_shorelineHeight",shorelineHeight);
					surfaceRenderer.sharedMaterial.SetFloat("_shorelineFrequency",shorelineFreq);
					surfaceRenderer.sharedMaterial.SetFloat("_shorelineScale",0.1f);
					surfaceRenderer.sharedMaterial.SetFloat("_shorelineSpeed",shorelineSpeed);
					surfaceRenderer.sharedMaterial.SetFloat("_shorelineNorm",shorelineNorm);

					//set physical properties
					surfaceRenderer.sharedMaterial.SetFloat("_specularPower",specularPower);
					surfaceRenderer.sharedMaterial.SetFloat("_roughness",roughness);
					surfaceRenderer.sharedMaterial.SetFloat("_roughness2",roughness2);
					surfaceRenderer.sharedMaterial.SetFloat("_reflecTerm",reflectTerm);
					surfaceRenderer.sharedMaterial.SetFloat("_reflecSharp",Mathf.Lerp(0.0f,-1.5f,reflectSharpen));


					//set surface settings
					surfaceRenderer.sharedMaterial.SetFloat("_overallBrightness", overallBright);
					surfaceRenderer.sharedMaterial.SetFloat("_overallTransparency", overallTransparency);
					surfaceRenderer.sharedMaterial.SetFloat("_DepthFade",Mathf.Lerp(0.1f,200.0f,depthAmt));
					surfaceRenderer.sharedMaterial.SetFloat("_ShallowFade",Mathf.Lerp(0.1f,800.0f,shallowAmt));
					surfaceRenderer.sharedMaterial.SetColor("_depthColor",depthColor);
					surfaceRenderer.sharedMaterial.SetColor("_shallowColor",shallowColor);
					surfaceRenderer.sharedMaterial.SetFloat("_EdgeFade", Mathf.Lerp(10.0f,1000.0f,edgeAmt));
					surfaceRenderer.sharedMaterial.SetColor("_SpecularColor",specularColor);
					surfaceRenderer.sharedMaterial.SetColor("_SSSColor",sssColor);
					surfaceRenderer.sharedMaterial.SetColor("_BlendColor",blendColor);
					surfaceRenderer.sharedMaterial.SetColor("_OverlayColor",overlayColor);
					surfaceRenderer.sharedMaterial.SetColor("_UnderwaterColor",underwaterColor);

					//set reflection properties
					surfaceRenderer.sharedMaterial.SetFloat("_reflectFlag",useEnableReflections ? 1.0f : 0.0f);
					surfaceRenderer.sharedMaterial.SetFloat("_reflectDynamicFlag",useEnableDynamicReflections ? 1.0f : 0.0f);
					surfaceRenderer.sharedMaterial.SetFloat("_reflectFallback", reflectFallback);
					surfaceRenderer.sharedMaterial.SetColor("_reflectFallbackColor", customRefColor);
					surfaceRenderer.sharedMaterial.SetColor("_ReflectionColor", reflectionColor);

					//set skybox texture
			        skybox = RenderSettings.skybox;
			        if(skybox != null && skybox.HasProperty("_Tex") && skybox.HasProperty("_Tint") && skybox.HasProperty("_Exposure") && skybox.HasProperty("_Rotation")){
						surfaceRenderer.sharedMaterial.SetTexture("_SkyCubemap",skybox.GetTexture("_Tex"));
						surfaceRenderer.sharedMaterial.SetColor("_SkyTint", skybox.GetColor("_Tint"));
			        	surfaceRenderer.sharedMaterial.SetFloat("_SkyExposure", skybox.GetFloat("_Exposure"));
			        	surfaceRenderer.sharedMaterial.SetFloat("_SkyRotation", skybox.GetFloat("_Rotation"));
					}

					//set custom cubemap
					if (customRefCubemap != null)
						surfaceRenderer.sharedMaterial.SetTexture("_CubeTex", customRefCubemap);

					//set camera properties
					surfaceRenderer.sharedMaterial.SetFloat("_cameraDistance",cameraDistance);


					//Force shoreline foam scale
					Vector2 setWaveTiling = new Vector2(1f, 10f);
					surfaceRenderer.sharedMaterial.SetTextureScale("_WaveTex", setWaveTiling);
					scaleRenderer.sharedMaterial.SetTextureScale("_WaveTex", setWaveTiling);


					/*
					//-------------------------------------------------------
					//###  SET SHADER DEFINE KEYWORDS  ###
					//-------------------------------------------------------
					if (surfaceRenderer != null){
						surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_TESS_ON");
						surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_TRANS_ON");
						surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_CAUST_ON");
						surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_DYNREFL_ON");
						if (enableTess) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_TESS_ON");
						if (moduleObject.enableTransparency) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_TRANS_ON");
						if (moduleObject.enableCaustics) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_CAUST_ON");
						if (useDynReflections) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_DYNREFL_ON");

						surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_REFL_OFF");
						surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_REFL_SKY");
						surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_REFL_CUBE");
						surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_REFL_COLOR");
						if (reflectFallback == 0) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_REFL_OFF");
						if (reflectFallback == 1) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_REFL_SKY");
						if (reflectFallback == 2) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_REFL_CUBE");
						if (reflectFallback == 3) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_REFL_COLOR");

						//surfaceRenderer.sharedMaterial.DisableKeyword("SUIMONO_FOAM_ON");
						//if (Application.isPlaying && enableFoam) surfaceRenderer.sharedMaterial.EnableKeyword("SUIMONO_FOAM_ON");
					}

					if (scaleRenderer != null && typeIndex == 0){
						scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_TESS_ON");
						scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_TRANS_ON");
						scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_CAUST_ON");
						scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_DYNREFL_ON");
						if (enableTess) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_TESS_ON");
						if (moduleObject.enableTransparency) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_TRANS_ON");
						if (moduleObject.enableCaustics) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_CAUST_ON");
						if (useDynReflections) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_DYNREFL_ON");

						scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_REFL_OFF");
						scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_REFL_SKY");
						scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_REFL_CUBE");
						scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_REFL_COLOR");
						if (reflectFallback == 0) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_REFL_OFF");
						if (reflectFallback == 1) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_REFL_SKY");
						if (reflectFallback == 2) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_REFL_CUBE");
						if (reflectFallback == 3) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_REFL_COLOR");

						//scaleRenderer.sharedMaterial.DisableKeyword("SUIMONO_FOAM_ON");
						//if (Application.isPlaying && enableFoam) scaleRenderer.sharedMaterial.EnableKeyword("SUIMONO_FOAM_ON");

					}
					*/





					//-------------------------------------------------------
					//###  SET SHADER BRANCH KEYS  ###
					//-------------------------------------------------------

					suimono_refl_off = 0.0f;
					suimono_refl_sky = 0.0f;
					suimono_refl_cube = 0.0f;
					suimono_refl_color = 0.0f;
					if (reflectFallback == 0) suimono_refl_off = 1.0f;
					if (reflectFallback == 1) suimono_refl_sky = 1.0f;
					if (reflectFallback == 2) suimono_refl_cube = 1.0f;
					if (reflectFallback == 3) suimono_refl_color = 1.0f;

					if (surfaceRenderer != null){
						surfaceRenderer.sharedMaterial.SetFloat("suimono_tess_on", enableTess?1.0f:0.0f);
						surfaceRenderer.sharedMaterial.SetFloat("suimono_trans_on", moduleObject.enableTransparency?1.0f:0.0f);
						surfaceRenderer.sharedMaterial.SetFloat("suimono_caust_on", moduleObject.enableCaustics?1.0f:0.0f);
						surfaceRenderer.sharedMaterial.SetFloat("suimono_dynrefl_on", useDynReflections?1.0f:0.0f);
						surfaceRenderer.sharedMaterial.SetFloat("suimono_refl_off", suimono_refl_off);
						surfaceRenderer.sharedMaterial.SetFloat("suimono_refl_sky", suimono_refl_sky);
						surfaceRenderer.sharedMaterial.SetFloat("suimono_refl_cube", suimono_refl_cube);
						surfaceRenderer.sharedMaterial.SetFloat("suimono_refl_color", suimono_refl_color);
					}

					if (scaleRenderer != null && typeIndex == 0){
						scaleRenderer.sharedMaterial.SetFloat("suimono_tess_on", enableTess?1.0f:0.0f);
						scaleRenderer.sharedMaterial.SetFloat("suimono_trans_on", moduleObject.enableTransparency?1.0f:0.0f);
						scaleRenderer.sharedMaterial.SetFloat("suimono_caust_on", moduleObject.enableCaustics?1.0f:0.0f);
						scaleRenderer.sharedMaterial.SetFloat("suimono_dynrefl_on", useDynReflections?1.0f:0.0f);
						scaleRenderer.sharedMaterial.SetFloat("suimono_refl_off", suimono_refl_off);
						scaleRenderer.sharedMaterial.SetFloat("suimono_refl_sky", suimono_refl_sky);
						scaleRenderer.sharedMaterial.SetFloat("suimono_refl_cube", suimono_refl_cube);
						scaleRenderer.sharedMaterial.SetFloat("suimono_refl_color", suimono_refl_color);
					}

				}


				//-------------------------------------------------------
				//###  enable / disable infinite scale surface  ###
				//-------------------------------------------------------
				if (typeIndex == 0 && Application.isPlaying){
					if (moduleObject.isUnderwater){
						if (scaleRenderer != null) scaleRenderer.enabled = false;
						if (scaleCollider != null) scaleCollider.enabled = false;

					} else {
						if (scaleRenderer != null) scaleRenderer.enabled = true;
						if (scaleCollider != null) scaleCollider.enabled = true;
					}
				} else {
					if (scaleRenderer != null) scaleRenderer.enabled = false;
					if (scaleCollider != null) scaleCollider.enabled = false;
				}



				//-------------------------------------------------------
				//###  set position and rotation for infinite ocean  ###
				//-------------------------------------------------------
				if (Application.isPlaying){
					if (typeIndex == 0){

						//force rotation
						transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0.0f, transform.eulerAngles.z);

						//calculate scales
						if (oceanScale < 1.0f) oceanScale = 1.0f;
						offamt = (0.4027f * oceanScale)/waveScale;
						spacer = (suimonoObject.transform.localScale.x * 4.0f);
						newPos = new Vector3(moduleObject.setCamera.position.x,suimonoObject.transform.position.y,moduleObject.setCamera.position.z);
						if (Mathf.Abs(suimonoObject.transform.position.x - newPos.x) > spacer){
							if (suimonoObject.transform.position.x > newPos.x) setScaleX -= offamt;
							if (suimonoObject.transform.position.x < newPos.x) setScaleX += offamt;
							suimonoObject.transform.position = new Vector3(newPos.x,suimonoObject.transform.position.y, suimonoObject.transform.position.z);
							scaleObject.transform.position = new Vector3(newPos.x, scaleObject.transform.position.y, scaleObject.transform.position.z);
						}
						if (Mathf.Abs(suimonoObject.transform.position.z - newPos.z) > spacer){
							if (suimonoObject.transform.position.z > newPos.z) setScaleZ -= offamt;
							if (suimonoObject.transform.position.z < newPos.z) setScaleZ += offamt;
							suimonoObject.transform.position = new Vector3(suimonoObject.transform.position.x, suimonoObject.transform.position.y, newPos.z);
							scaleObject.transform.position = new Vector3(scaleObject.transform.position.x, scaleObject.transform.position.y, newPos.z);
						}

						//update position
						if (currentPosition != suimonoObject.transform.position){
							currentPosition = suimonoObject.transform.position;
							savePos = new Vector2(setScaleX,setScaleZ);
						}

						//set shader offset
						surfaceRenderer.sharedMaterial.SetFloat("_suimono_uvx",0.0f-(savePos.x));
						surfaceRenderer.sharedMaterial.SetFloat("_suimono_uvy",0.0f-(savePos.y));

						//set scale object offset
						scaleObject.transform.localPosition = new Vector3(scaleObject.transform.localPosition.x, -0.1f, scaleObject.transform.localPosition.z);

						//set infinite ocean object scaling
						if (scaleRenderer != null){
							setScale = Mathf.Ceil(moduleObject.setCameraComponent.farClipPlane/20.0f)*suimonoObject.transform.localScale.x;
							scaleObject.transform.localScale = new Vector3(setScale*0.5f,1.0f,setScale*0.5f);

							oceanUseScale = 4.0f;
							this.transform.localScale = new Vector3(1f,1f,1f);
							suimonoObject.transform.localScale = new Vector3(oceanUseScale*oceanScale,1.0f,oceanUseScale*oceanScale);

							//copy shader settings to infinite scale surface
							if (scaleRenderer != null){
								scaleRenderer.material.CopyPropertiesFromMaterial(tempMaterial);
									scaleRenderer.sharedMaterial.SetFloat("_suimono_uvx",0.0f-savePos.x);
									scaleRenderer.sharedMaterial.SetFloat("_suimono_uvy",0.0f-savePos.y);
							
									setSc = scaleRenderer.sharedMaterial.GetTextureScale("_NormalTexS");

									useSc = (scaleObject.transform.localScale.x/suimonoObject.transform.localScale.x);
									scaleRenderer.sharedMaterial.SetTextureScale("_NormalTexS", setSc*useSc);
									scaleRenderer.sharedMaterial.SetTextureScale("_FoamTex", setSc*useSc);
							}

						}


					} else {
						savePos = new Vector3(0f,0f,0f);
						suimonoObject.transform.localScale = new Vector3(1f,1f,1f);
						scaleObject.transform.localScale = new Vector3(1f,1f,1f);
						surfaceRenderer.sharedMaterial.SetFloat("_suimono_uvx",0.0f);
						surfaceRenderer.sharedMaterial.SetFloat("_suimono_uvy",0.0f);

					}
				}



				//-------------------------------------------------------
				//###  Set Debug Modes  ###
				//-------------------------------------------------------
				if (surfaceRenderer != null){
					if (showDepthMask){
						surfaceRenderer.sharedMaterial.SetFloat("_suimono_DebugDepthMask",1.0f);
					} else {
						surfaceRenderer.sharedMaterial.SetFloat("_suimono_DebugDepthMask",0.0f);
					}

					if (showWorldMask){
						surfaceRenderer.sharedMaterial.SetFloat("_suimono_DebugWorldNormalMask",1.0f);
					} else {
						surfaceRenderer.sharedMaterial.SetFloat("_suimono_DebugWorldNormalMask",0.0f);
					}

				}


				//-------------------------------------------------------
				//###  Update Preset Listing  ###
				//-------------------------------------------------------
				#if UNITY_EDITOR
					if (presetFileUseIndex != presetFileIndex){
						presetFileUseIndex = presetFileIndex;
						PresetInit();
					}
				#endif
			}
		}





		public Vector2 SuimonoConvertAngleToVector(float convertAngle){
			flow_dir = new Vector2(0f,0f);
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









		// ########## PUBLIC FUNCTIONS #######################################################################################################################

		public void SuimonoSetPreset( string fName, string pName){
			presetTimer = 1f;
			SetTemporaryPresetData();
			PresetLoadBuild(fName,pName);
		}



		public void SuimonoSavePreset( string fName, string pName){
			int setFolder = -1;
			int setPreset = -1;

			setFolder = PresetGetNum("folder",fName);
			setPreset = PresetGetNum("preset",pName);

			if (setFolder >= 0 && setPreset >= 0){
				PresetSave(setFolder,setPreset);
			} else {
				Debug.Log("The Preset "+pName+" in folder "+fName+" cannot be found!");
			}
		}





		//########### NEW PRESETS #########################################################################################################################
		void PresetInit(){

			presetTimer = 1f;

			#if !UNITY_WEBPLAYER

			//get preset directories
			presetDirsArr = new List<string>();
			dirInfo = new DirectoryInfo(dir+"/").GetFiles("SUIMONO_PRESETS_*");
			if (new DirectoryInfo(dir+"/") != null){
				for (d = 0; d < dirInfo.Length; d++){
					presetDirsArr.Add(dirInfo[d].ToString());
				}
			}
			presetDirs = new List<string>(new string[presetDirsArr.Count]);
			for (dn = 0; dn < presetDirsArr.Count; dn++){
				presetDirs[dn] = presetDirsArr[dn].ToString();
				presetDirs[dn] = presetDirs[dn].Remove(0,dir.Length);
				presetDirs[dn] = presetDirs[dn].Replace("SUIMONO_PRESETS_","");
				presetDirs[dn] = presetDirs[dn].Replace(".meta","");
			}


			//get preset files listing
			presetFilesArr = new List<string>();
			pdir = dir + "/SUIMONO_PRESETS_"+presetDirs[presetFileIndex];

			fileInfo = new DirectoryInfo(pdir).GetFiles("SUIMONO_PRESET_*");
			if (new DirectoryInfo(pdir) != null){
				for (f = 0; f < fileInfo.Length; f++){
					presetFilesArr.Add(fileInfo[f].ToString());
				}
			}
			px = 0;
			for (nx = 0; nx < presetFilesArr.Count; nx++){
				if (!presetFilesArr[nx].ToString().Contains(".meta")) px++;
			}
			presetFiles = new string[px];
			ax = 0;
			for (n = 0; n < presetFilesArr.Count; n++){
				if (!presetFilesArr[n].ToString().Contains(".meta")){
					presetFiles[ax] = presetFilesArr[n].ToString();
					presetFiles[ax] = presetFiles[ax].Remove(0,pdir.Length);
					presetFiles[ax] = presetFiles[ax].Replace("SUIMONO_PRESET_","");
					presetFiles[ax] = presetFiles[ax].Replace(".txt","");
					ax++;
				}
			}

			#endif
		}







		public int PresetGetNum( string mode, string pName){

			int setMode = -1;
			int setFolder = -1;
			int setPreset = -1;
			
			#if !UNITY_WEBPLAYER

			if (mode == "folder"){
				//get preset directories
				tempPresetDirsArr = new List<string>();
				dirInfo = new DirectoryInfo(dir+"/").GetFiles("SUIMONO_PRESETS_*");
				if (new DirectoryInfo(dir+"/") != null){
					for (d = 0; d < dirInfo.Length; d++){
						tempPresetDirsArr.Add(dirInfo[d].ToString());
					}
				}
				tempPresetDirs = new string[tempPresetDirsArr.Count];
				for (dn = 0; dn < tempPresetDirsArr.Count; dn++){
					tempPresetDirs[dn] = tempPresetDirsArr[dn].ToString();
					tempPresetDirs[dn] = tempPresetDirs[dn].Remove(0,dir.Length);
					tempPresetDirs[dn] = tempPresetDirs[dn].Replace("SUIMONO_PRESETS_","");
					tempPresetDirs[dn] = tempPresetDirs[dn].Replace(".meta","");
					if (tempPresetDirs[dn] == pName) setFolder = dn;
				}
				setMode = setFolder;
			}

			if (mode == "preset"){
				//get preset files listing
				tempPresetFilesArr = new List<string>();
				pdir = dir + "/SUIMONO_PRESETS_"+presetDirs[presetFileIndex];
				fileInfo = new DirectoryInfo(pdir).GetFiles("SUIMONO_PRESET_*");
				if (new DirectoryInfo(pdir) != null){
					for (f = 0; f < fileInfo.Length; f++){
						tempPresetFilesArr.Add(fileInfo[f].ToString());
					}
				}
				px = 0;
				for (nx = 0; nx < tempPresetFilesArr.Count; nx++){
					if (!tempPresetFilesArr[nx].ToString().Contains(".meta")) px++;
				}
				tempPresetFiles = new string[px];
				ax = 0;
				for (n = 0; n < tempPresetFilesArr.Count; n++){
					if (!tempPresetFilesArr[n].ToString().Contains(".meta")){
						tempPresetFiles[ax] = tempPresetFilesArr[n].ToString();
						tempPresetFiles[ax] = tempPresetFiles[ax].Remove(0,pdir.Length);
						tempPresetFiles[ax] = tempPresetFiles[ax].Replace("SUIMONO_PRESET_","");
						tempPresetFiles[ax] = tempPresetFiles[ax].Replace(".txt","");
						if (tempPresetFiles[ax] == pName) setPreset = ax;
						ax++;
					}
				}
				setMode = setPreset;
			}

			#endif

			return setMode;


		}


		






		public void PresetRename( int ppos, string newName){
		#if UNITY_EDITOR
		#if !UNITY_WEBPLAYER
			pdir = dir + "/SUIMONO_PRESETS_"+presetDirs[presetFileIndex];
			oldName = pdir+"/SUIMONO_PRESET_"+presetFiles[ppos]+".txt";
			moveName = pdir+"/SUIMONO_PRESET_"+newName+".txt";
			File.Move(oldName,moveName);
			AssetDatabase.Refresh();
			PresetInit();
		#endif
		#endif
		}


		public void PresetAdd(){
		#if UNITY_EDITOR
		#if !UNITY_WEBPLAYER
			setNum = presetFiles.Length;
			pdir = dir + "/SUIMONO_PRESETS_"+presetDirs[presetFileIndex];
			while (File.Exists(pdir+"/SUIMONO_PRESET_New Preset "+setNum+".txt")){
				setNum += 1;
			}
			if (presetFiles.Length >= 1){
				File.Create(pdir+"/SUIMONO_PRESET_New Preset "+setNum+".txt").Close();
			} else {
				File.Create(pdir+"/SUIMONO_PRESET_New Preset 1.txt").Close();
				setNum = 1;
			}
			AssetDatabase.Refresh();
			PresetInit();
			SuimonoSavePreset(presetDirs[presetFileIndex],"New Preset "+setNum);
		#endif
		#endif
		}


		public void PresetDelete( int fpos, int ppos ){
		#if UNITY_EDITOR
		#if !UNITY_WEBPLAYER
			pdir = dir + "/SUIMONO_PRESETS_"+presetDirs[fpos];
			if (File.Exists(pdir+"/SUIMONO_PRESET_"+presetFiles[ppos]+".txt")){
				File.Delete(pdir+"/SUIMONO_PRESET_"+presetFiles[ppos]+".txt");
				if (presetIndex == ppos) presetIndex = -1;
			}
			AssetDatabase.Refresh();
			PresetInit();
		#endif
		#endif
		}


		public void PresetSave( int fpos, int ppos ){
		#if UNITY_EDITOR
		#if !UNITY_WEBPLAYER
			pdir = dir + "/SUIMONO_PRESETS_"+presetDirs[fpos];
			if (File.Exists(pdir+"/SUIMONO_PRESET_"+presetFiles[ppos]+".txt")){

				//Caclulate data
				presetDataString = "";
				presetDataString += (PresetEncode("color_depth")) + "\n";
				presetDataString += (PresetEncode("color_shallow")) + "\n";
				presetDataString += (PresetEncode("color_blend")) + "\n";	
				presetDataString += (PresetEncode("color_overlay")) + "\n";
				presetDataString += (PresetEncode("color_caustics")) + "\n";
				presetDataString += (PresetEncode("color_reflection")) + "\n";
				presetDataString += (PresetEncode("color_specular")) + "\n";
				presetDataString += (PresetEncode("color_sss")) + "\n";
				presetDataString += (PresetEncode("color_foam")) + "\n";
				presetDataString += (PresetEncode("color_underwater")) + "\n";
				presetDataString += (PresetEncode("data_beaufort")) + "\n";
				presetDataString += (PresetEncode("data_flowdir")) + "\n";
				presetDataString += (PresetEncode("data_flowspeed")) + "\n";
				presetDataString += (PresetEncode("data_wavescale")) + "\n";
				presetDataString += (PresetEncode("data_customwaves")) + "\n";
				presetDataString += (PresetEncode("data_waveheight")) + "\n";
				presetDataString += (PresetEncode("data_heightprojection")) + "\n";
				presetDataString += (PresetEncode("data_turbulence")) + "\n";
				presetDataString += (PresetEncode("data_lgwaveheight")) + "\n";
				presetDataString += (PresetEncode("data_lgwavescale")) + "\n";
				presetDataString += (PresetEncode("data_shorelineheight")) + "\n";
				presetDataString += (PresetEncode("data_shorelinefreq")) + "\n";
				presetDataString += (PresetEncode("data_shorelinescale")) + "\n";
				presetDataString += (PresetEncode("data_shorelinespeed")) + "\n";
				presetDataString += (PresetEncode("data_shorelinenorm")) + "\n";
				presetDataString += (PresetEncode("data_overallbright")) + "\n";
				presetDataString += (PresetEncode("data_overalltransparency")) + "\n";
				presetDataString += (PresetEncode("data_edgeamt")) + "\n";
				presetDataString += (PresetEncode("data_depthamt")) + "\n";
				presetDataString += (PresetEncode("data_shallowamt")) + "\n";
				presetDataString += (PresetEncode("data_refractstrength")) + "\n";
				presetDataString += (PresetEncode("data_aberrationscale")) + "\n";
				presetDataString += (PresetEncode("data_causticsfade")) + "\n";
				presetDataString += (PresetEncode("data_reflectprojection")) + "\n";
				presetDataString += (PresetEncode("data_reflectblur")) + "\n";
				presetDataString += (PresetEncode("data_reflectterm")) + "\n";
				presetDataString += (PresetEncode("data_reflectsharpen")) + "\n";
				presetDataString += (PresetEncode("data_roughness")) + "\n";
				presetDataString += (PresetEncode("data_roughness2")) + "\n";
				presetDataString += (PresetEncode("data_enablefoam")) + "\n";
				presetDataString += (PresetEncode("data_foamscale")) + "\n";
				presetDataString += (PresetEncode("data_foamspeed")) + "\n";
				presetDataString += (PresetEncode("data_edgefoamamt")) + "\n";
				presetDataString += (PresetEncode("data_shallowfoamamt")) + "\n";
				presetDataString += (PresetEncode("data_heightfoamamt")) + "\n";
				presetDataString += (PresetEncode("data_hfoamheight")) + "\n";
				presetDataString += (PresetEncode("data_hfoamspread")) + "\n";
				presetDataString += (PresetEncode("data_enableunderdebris")) + "\n";
				presetDataString += (PresetEncode("data_underlightfactor")) + "\n";
				presetDataString += (PresetEncode("data_underrefractionamount")) + "\n";
				presetDataString += (PresetEncode("data_underrefractionscale")) + "\n";
				presetDataString += (PresetEncode("data_underrefractionspeed")) + "\n";
				presetDataString += (PresetEncode("data_underbluramount")) + "\n";
				presetDataString += (PresetEncode("data_underwaterfogdist")) + "\n";
				presetDataString += (PresetEncode("data_underwaterfogspread")) + "\n";
				presetDataString += (PresetEncode("data_underDarkRange")) + "\n";

				//save data
				sw = new StreamWriter(pdir+"/SUIMONO_PRESET_"+presetFiles[ppos]+".txt");
				sw.AutoFlush = true;
				sw.Write(presetDataString);
			    sw.Close();

				Debug.Log("Preset '"+presetFiles[ppos]+"' has been saved!");
			}
		#endif
		#endif
		}




		public void PresetLoad( int ppos ){
			if (presetIndex >= 0){
				pdir = dir + "/SUIMONO_PRESETS_"+presetDirs[presetFileIndex];
				sr = new StreamReader(pdir+"/SUIMONO_PRESET_"+presetFiles[ppos]+".txt");
			    presetDataString = sr.ReadToEnd();
			    sr.Close();

			    presetDataArray = presetDataString.Split("\n"[0]);

				//Decode Data
				for (dx = 0; dx < presetDataArray.Length; dx++){
					if (presetDataArray[dx] != "" && presetDataArray[dx] != "\n"){
						pFrom = presetDataArray[dx].IndexOf("<") + "<".Length;
						pTo = presetDataArray[dx].LastIndexOf(">");
						key = presetDataArray[dx].Substring(pFrom, pTo - pFrom);

						pFrom = presetDataArray[dx].IndexOf("(") + "(".Length;
						pTo = presetDataArray[dx].LastIndexOf(")");
						dat = presetDataArray[dx].Substring(pFrom, pTo - pFrom);

						SetTemporaryPresetData();
						PresetDecode(key,dat);
					}
				}
			}
		}




		void PresetLoadBuild( string fName, string pName ){
			#if !UNITY_WEBPLAYER
			datFile = Resources.Load("SUIMONO_PRESETS_"+fName+"/SUIMONO_PRESET_"+pName) as TextAsset;
			presetDataString = datFile.text;
			presetDataArray = presetDataString.Split("\n"[0]);

				//Decode Data
				for (dx = 0; dx < presetDataArray.Length; dx++){
					if (presetDataArray[dx] != "" && presetDataArray[dx] != "\n"){
						pFrom = presetDataArray[dx].IndexOf("<") + "<".Length;
						pTo = presetDataArray[dx].LastIndexOf(">");
						key = presetDataArray[dx].Substring(pFrom, pTo - pFrom);

						pFrom = presetDataArray[dx].IndexOf("(") + "(".Length;
						pTo = presetDataArray[dx].LastIndexOf(")");
						dat = presetDataArray[dx].Substring(pFrom, pTo - pFrom);

						PresetDecode(key,dat);
					}
				}
			#endif
		}




		void PresetDecode( string key, string dat ){
			dataS = dat.Split(","[0]);

			if (presetTimer > 1f) presetTimer = 1f;

			if (key == "color_depth") depthColor = Color.Lerp(temp_depthColor, DecodeColor(dataS), presetTimer);
			if (key == "color_shallow") shallowColor = Color.Lerp(temp_shallowColor, DecodeColor(dataS), presetTimer);
			if (key == "color_blend") blendColor = Color.Lerp(temp_blendColor, DecodeColor(dataS), presetTimer);
			if (key == "color_overlay") overlayColor = Color.Lerp(temp_overlayColor, DecodeColor(dataS), presetTimer);
			if (key == "color_caustics") causticsColor = Color.Lerp(temp_causticsColor, DecodeColor(dataS), presetTimer);
			if (key == "color_reflection") reflectionColor = Color.Lerp(temp_reflectionColor, DecodeColor(dataS), presetTimer);
			if (key == "color_specular") specularColor = Color.Lerp(temp_specularColor, DecodeColor(dataS), presetTimer);
			if (key == "color_sss") sssColor = Color.Lerp(temp_sssColor, DecodeColor(dataS), presetTimer);
			if (key == "color_foam") foamColor = Color.Lerp(temp_foamColor, DecodeColor(dataS), presetTimer);
			if (key == "color_underwater") underwaterColor = Color.Lerp(temp_underwaterColor, DecodeColor(dataS), presetTimer);

			if (key == "data_beaufort") beaufortScale = Mathf.Lerp(temp_beaufortScale, DecodeFloat(dataS), presetTimer);
			if (key == "data_flowdir") flowDirection = Mathf.Lerp(temp_flowDirection, DecodeFloat(dataS), presetTimer);
			if (key == "data_flowspeed") flowSpeed = Mathf.Lerp(temp_flowSpeed, DecodeFloat(dataS), presetTimer);
			if (key == "data_wavescale") waveScale = Mathf.Lerp(temp_waveScale, DecodeFloat(dataS), presetTimer);
			if (key == "data_waveheight") waveHeight = Mathf.Lerp(temp_waveHeight, DecodeFloat(dataS), presetTimer);
			if (key == "data_heightprojection") heightProjection = Mathf.Lerp(temp_heightProjection, DecodeFloat(dataS), presetTimer);
			if (key == "data_turbulence") turbulenceFactor = Mathf.Lerp(temp_turbulenceFactor, DecodeFloat(dataS), presetTimer);
			if (key == "data_lgwaveheight") lgWaveHeight = Mathf.Lerp(temp_lgWaveHeight, DecodeFloat(dataS), presetTimer);
			if (key == "data_lgwavescale") lgWaveScale = Mathf.Lerp(temp_lgWaveScale, DecodeFloat(dataS), presetTimer);
			if (key == "data_shorelineheight") shorelineHeight = Mathf.Lerp(temp_shorelineHeight, DecodeFloat(dataS), presetTimer);
			if (key == "data_shorelinefreq") shorelineFreq = Mathf.Lerp(temp_shorelineFreq, DecodeFloat(dataS), presetTimer);
			if (key == "data_shorelinescale") shorelineScale = Mathf.Lerp(temp_shorelineScale, DecodeFloat(dataS), presetTimer);
			if (key == "data_shorelinespeed") shorelineSpeed = Mathf.Lerp(temp_shorelineSpeed, DecodeFloat(dataS), presetTimer);
			if (key == "data_shorelinenorm") shorelineNorm = Mathf.Lerp(temp_shorelineNorm, DecodeFloat(dataS), presetTimer);
			if (key == "data_overallbright") overallBright = Mathf.Lerp(temp_overallBright, DecodeFloat(dataS), presetTimer);
			if (key == "data_overalltransparency") overallTransparency = Mathf.Lerp(temp_overallTransparency, DecodeFloat(dataS), presetTimer);
			if (key == "data_edgeamt") edgeAmt = Mathf.Lerp(temp_edgeAmt, DecodeFloat(dataS), presetTimer);
			if (key == "data_depthamt") depthAmt = Mathf.Lerp(temp_depthAmt, DecodeFloat(dataS), presetTimer);
			if (key == "data_shallowamt") shallowAmt = Mathf.Lerp(temp_shallowAmt, DecodeFloat(dataS), presetTimer);
			if (key == "data_refractstrength") refractStrength = Mathf.Lerp(temp_refractStrength, DecodeFloat(dataS), presetTimer);
			if (key == "data_aberrationscale") aberrationScale = Mathf.Lerp(temp_aberrationScale, DecodeFloat(dataS), presetTimer);
			if (key == "data_causticsfade") causticsFade = Mathf.Lerp(temp_causticsFade, DecodeFloat(dataS), presetTimer);
			if (key == "data_reflectprojection") reflectProjection = Mathf.Lerp(temp_reflectProjection, DecodeFloat(dataS), presetTimer);
			if (key == "data_reflectblur") reflectBlur = Mathf.Lerp(temp_reflectBlur, DecodeFloat(dataS), presetTimer);
			if (key == "data_reflectterm") reflectTerm = Mathf.Lerp(temp_reflectTerm, DecodeFloat(dataS), presetTimer);
			if (key == "data_reflectsharpen") reflectSharpen = Mathf.Lerp(temp_reflectSharpen, DecodeFloat(dataS), presetTimer);
			if (key == "data_roughness") roughness = Mathf.Lerp(temp_roughness, DecodeFloat(dataS), presetTimer);
			if (key == "data_roughness2") roughness2 = Mathf.Lerp(temp_roughness2, DecodeFloat(dataS), presetTimer);
			if (key == "data_foamscale") foamScale = Mathf.Lerp(temp_foamScale, DecodeFloat(dataS), presetTimer);
			if (key == "data_foamspeed") foamSpeed = Mathf.Lerp(temp_foamSpeed, DecodeFloat(dataS), presetTimer);
			if (key == "data_edgefoamamt") edgeFoamAmt = Mathf.Lerp(temp_edgeFoamAmt, DecodeFloat(dataS), presetTimer);
			if (key == "data_shallowfoamamt") shallowFoamAmt = Mathf.Lerp(temp_shallowFoamAmt, DecodeFloat(dataS), presetTimer);
			if (key == "data_heightfoamamt") heightFoamAmt = Mathf.Lerp(temp_heightFoamAmt, DecodeFloat(dataS), presetTimer);
			if (key == "data_hfoamheight") hFoamHeight = Mathf.Lerp(temp_hFoamHeight, DecodeFloat(dataS), presetTimer);
			if (key == "data_hfoamspread") hFoamSpread = Mathf.Lerp(temp_hFoamSpread, DecodeFloat(dataS), presetTimer);
			if (key == "data_underlightfactor") underLightFactor = Mathf.Lerp(temp_underLightFactor, DecodeFloat(dataS), presetTimer);
			if (key == "data_underrefractionamount") underRefractionAmount = Mathf.Lerp(temp_underRefractionAmount, DecodeFloat(dataS), presetTimer);
			if (key == "data_underrefractionscale") underRefractionScale = Mathf.Lerp(temp_underRefractionScale, DecodeFloat(dataS), presetTimer);
			if (key == "data_underrefractionspeed") underRefractionSpeed = Mathf.Lerp(temp_underRefractionSpeed, DecodeFloat(dataS), presetTimer);
			if (key == "data_underbluramount") underBlurAmount = Mathf.Lerp(temp_underBlurAmount, DecodeFloat(dataS), presetTimer);
			if (key == "data_underwaterfogdist") underwaterFogDist = Mathf.Lerp(temp_underwaterFogDist, DecodeFloat(dataS), presetTimer);
			if (key == "data_underwaterfogspread") underwaterFogSpread = Mathf.Lerp(temp_underwaterFogSpread, DecodeFloat(dataS), presetTimer);
			if (key == "data_underDarkRange") underDarkRange = Mathf.Lerp(temp_underDarkRange, DecodeFloat(dataS), presetTimer);

			if (key == "data_customwaves") customWaves = DecodeBool(dataS);
			if (key == "data_enablefoam") enableFoam = DecodeBool(dataS);
			if (key == "data_enableunderdebris") enableUnderDebris = DecodeBool(dataS);
		}	


		public Color DecodeColor(string[] data){
			return new Color(DecodeSingleFloat(data[0]), DecodeSingleFloat( data[1]), DecodeSingleFloat( data[2]), DecodeSingleFloat( data[3]));
		}
		public float DecodeFloat(string[] data){
			return DecodeSingleFloat(data[0]);
		}

		public int DecodeInt(string[] data){
			return int.Parse(data[0]);
		}
		public bool DecodeBool(string[] data){
			retVal = false;
			if (data[0] == "True") retVal = true;
			return retVal;
		}

		public float DecodeSingleFloat( string data ) {
			return float.Parse( data,
					  System.Globalization.NumberStyles.Float,
					  System.Globalization.CultureInfo.InvariantCulture );
		}







		public string PresetEncode( string key ){
			retData = "";

			if (key == "color_depth") retData = EncodeColor( depthColor );
			if (key == "color_shallow") retData = EncodeColor( shallowColor );
			if (key == "color_blend") retData = EncodeColor( blendColor );
			if (key == "color_overlay") retData = EncodeColor( overlayColor );
			if (key == "color_caustics") retData = EncodeColor( causticsColor );
			if (key == "color_reflection") retData = EncodeColor( reflectionColor );
			if (key == "color_specular") retData = EncodeColor( specularColor );
			if (key == "color_sss") retData = EncodeColor( sssColor );
			if (key == "color_foam") retData = EncodeColor( foamColor );
			if (key == "color_underwater") retData = EncodeColor( underwaterColor );
			if (key == "data_customwaves") retData = "(" + customWaves.ToString().Replace( " ", "" ) + ")";
			if (key == "data_enableunderdebris") retData = "(" + enableUnderDebris.ToString().Replace( " ", "" ) + ")";
			if (key == "data_enablefoam") retData = "(" + enableFoam.ToString().Replace( " ", "" ) + ")";
			if (key == "data_beaufort") retData = "("+EncodeSingleFloat( beaufortScale )+")";
			if (key == "data_flowdir") retData = "("+EncodeSingleFloat( flowDirection )+")";
			if (key == "data_flowspeed") retData = "("+EncodeSingleFloat( flowSpeed )+")";
			if (key == "data_wavescale") retData = "("+EncodeSingleFloat( waveScale )+")";
			if (key == "data_waveheight") retData = "("+EncodeSingleFloat( waveHeight )+")";
			if (key == "data_heightprojection") retData = "("+EncodeSingleFloat( heightProjection )+")";
			if (key == "data_turbulence") retData = "("+EncodeSingleFloat( turbulenceFactor )+")";
			if (key == "data_lgwaveheight") retData = "("+EncodeSingleFloat( lgWaveHeight )+")";
			if (key == "data_lgwavescale") retData = "("+EncodeSingleFloat( lgWaveScale )+")";
			if (key == "data_shorelineheight") retData = "("+EncodeSingleFloat( shorelineHeight )+")";
			if (key == "data_shorelinefreq") retData = "("+EncodeSingleFloat( shorelineFreq )+")";
			if (key == "data_shorelinescale") retData = "("+EncodeSingleFloat( shorelineScale )+")";
			if (key == "data_shorelinespeed") retData = "("+EncodeSingleFloat( shorelineSpeed )+")";
			if (key == "data_shorelinenorm") retData = "("+EncodeSingleFloat( shorelineNorm )+")";
			if (key == "data_overallbright") retData = "("+EncodeSingleFloat( overallBright )+")";
			if (key == "data_overalltransparency") retData = "("+EncodeSingleFloat( overallTransparency )+")";
			if (key == "data_edgeamt") retData = "("+EncodeSingleFloat( edgeAmt )+")";
			if (key == "data_depthamt") retData = "("+EncodeSingleFloat( depthAmt )+")";
			if (key == "data_shallowamt") retData = "("+EncodeSingleFloat( shallowAmt )+")";
			if (key == "data_refractstrength") retData = "("+EncodeSingleFloat( refractStrength )+")";
			if (key == "data_aberrationscale") retData = "("+EncodeSingleFloat( aberrationScale )+")";
			if (key == "data_causticsfade") retData = "("+EncodeSingleFloat( causticsFade )+")";
			if (key == "data_reflectprojection") retData = "("+EncodeSingleFloat( reflectProjection )+")";
			if (key == "data_reflectblur") retData = "("+EncodeSingleFloat( reflectBlur )+")";
			if (key == "data_reflectterm") retData = "("+EncodeSingleFloat( reflectTerm )+")";
			if (key == "data_reflectsharpen") retData = "("+EncodeSingleFloat( reflectSharpen )+")";
			if (key == "data_roughness") retData = "("+EncodeSingleFloat( roughness )+")";
			if (key == "data_roughness2") retData = "("+EncodeSingleFloat( roughness2 )+")";
			if (key == "data_foamscale") retData = "("+EncodeSingleFloat( foamScale )+")";
			if (key == "data_foamspeed") retData = "("+EncodeSingleFloat( foamSpeed )+")";
			if (key == "data_edgefoamamt") retData = "("+EncodeSingleFloat( edgeFoamAmt )+")";
			if (key == "data_shallowfoamamt") retData = "("+EncodeSingleFloat( shallowFoamAmt )+")";
			if (key == "data_heightfoamamt") retData = "("+EncodeSingleFloat( heightFoamAmt )+")";
			if (key == "data_hfoamheight") retData = "("+EncodeSingleFloat( hFoamHeight )+")";
			if (key == "data_hfoamspread") retData = "("+EncodeSingleFloat( hFoamSpread )+")";
			if (key == "data_underlightfactor") retData = "("+EncodeSingleFloat( underLightFactor )+")";
			if (key == "data_underrefractionamount") retData = "("+EncodeSingleFloat( underRefractionAmount )+")";
			if (key == "data_underrefractionscale") retData = "("+EncodeSingleFloat( underRefractionScale )+")";
			if (key == "data_underrefractionspeed") retData = "("+EncodeSingleFloat( underRefractionSpeed )+")";
			if (key == "data_underbluramount") retData = "("+EncodeSingleFloat( underBlurAmount )+")";
			if (key == "data_underwaterfogdist") retData = "("+EncodeSingleFloat( underwaterFogDist )+")";
			if (key == "data_underwaterfogspread") retData = "("+EncodeSingleFloat( underwaterFogSpread )+")";
			if (key == "data_underDarkRange") retData = "("+EncodeSingleFloat( underDarkRange )+")";

			retData = "<"+key+">" + retData;
			return retData;
		}


		public string EncodeSingleFloat( float data ) {
			return data.ToString( System.Globalization.CultureInfo.InvariantCulture );
		}

		public string EncodeColor( Color data ) {
			return string.Format( "({0},{1},{2},{3})", EncodeSingleFloat( data.r ), EncodeSingleFloat( data.g ), EncodeSingleFloat( data.b ), EncodeSingleFloat( data.a ) );
		}




		//PRESET TRANSITION FUNCTION OPTIONS:

		//Option 1: (string)preset name, (float)transition time in seconds
		//it is assumed that preset is in the "Built-In Presets" folder.
		public void SuimonoTransitionPreset(string pName, float transitionTime){
			presetTimer = 0f;
			presetTransitionTime = transitionTime;
			presetStartTransition = true;
			currentPresetFolder = "Built-In Presets";
			currentPresetName = pName;
			SetTemporaryPresetData();
		}
		//Option 2: (string)preset folder name, (string)preset name, (float)transition time in seconds
		//For accessing presets in a custom folder name.
		public void SuimonoTransitionPreset(string fName, string pName, float transitionTime){
			presetTimer = 0f;
			presetTransitionTime = transitionTime;
			presetStartTransition = true;
			currentPresetFolder = fName;
			currentPresetName = pName;
			SetTemporaryPresetData();
		}
		//Option 3: (string)preset folder name, (string)from preset name, (string)to preset name, (float)transition time in seconds
		//For transitioning directly from one preset to another, when presets are in same folder.
		public void SuimonoTransitionPreset(string fName, string pName0, string pName1, float transitionTime){
			SuimonoSetPreset(fName, pName0);
			presetTimer = 0f;
			presetTransitionTime = transitionTime;
			presetStartTransition = true;
			currentPresetFolder = fName;
			currentPresetName = pName1;
			SetTemporaryPresetData();
		}
		//Option 4: (string)from preset folder name, (string)from preset name, (string)to preset folder name, (string)to preset name, (float)transition time in seconds
		//For transitioning directly from one preset to another, when presets are in different folders.
		public void SuimonoTransitionPreset(string fName0, string pName0, string fName1, string pName1, float transitionTime){
			SuimonoSetPreset(fName0, pName0);
			presetTimer = 0f;
			presetTransitionTime = transitionTime;
			presetStartTransition = true;
			currentPresetFolder = fName1;
			currentPresetName = pName1;
			SetTemporaryPresetData();
		}	



		//Caches current data for usage in lerping preset transitions
		void SetTemporaryPresetData(){

			temp_depthColor = depthColor;
			temp_shallowColor = shallowColor;
			temp_blendColor = blendColor;
			temp_overlayColor = overlayColor;
			temp_causticsColor = causticsColor;
			temp_reflectionColor = reflectionColor;
			temp_specularColor = specularColor;
			temp_sssColor = sssColor;
			temp_foamColor = foamColor;
			temp_underwaterColor = underwaterColor;

			temp_beaufortScale = beaufortScale;
			temp_flowDirection = flowDirection;
			temp_flowSpeed = flowSpeed;
			temp_waveScale = waveScale;
			temp_waveHeight = waveHeight;
			temp_heightProjection = heightProjection;
			temp_turbulenceFactor = turbulenceFactor;
			temp_lgWaveHeight = lgWaveHeight;
			temp_lgWaveScale = lgWaveScale;
			temp_shorelineHeight = shorelineHeight;
			temp_shorelineFreq = shorelineFreq;
			temp_shorelineScale = shorelineScale;
			temp_shorelineSpeed = shorelineSpeed;
			temp_shorelineNorm = shorelineNorm;
			temp_overallBright = overallBright;
			temp_overallTransparency = overallTransparency;
			temp_edgeAmt = edgeAmt;
			temp_depthAmt = depthAmt;
			temp_shallowAmt = shallowAmt;
			temp_refractStrength = refractStrength;
			temp_aberrationScale = aberrationScale;
			temp_causticsFade = causticsFade;
			temp_reflectProjection = reflectProjection;
			temp_reflectBlur = reflectBlur;
			temp_reflectTerm = reflectTerm;
			temp_reflectSharpen = reflectSharpen;
			temp_roughness = roughness;
			temp_roughness2 = roughness2;
			temp_foamScale = foamScale;
			temp_foamSpeed = foamSpeed;
			temp_edgeFoamAmt = edgeFoamAmt;
			temp_shallowFoamAmt = shallowFoamAmt;
			temp_heightFoamAmt = heightFoamAmt;
			temp_hFoamHeight = hFoamHeight;
			temp_hFoamSpread = hFoamSpread;
			temp_underLightFactor = underLightFactor;
			temp_underRefractionAmount = underRefractionAmount;
			temp_underRefractionScale = underRefractionScale;
			temp_underRefractionSpeed = underRefractionSpeed;
			temp_underBlurAmount = underBlurAmount;
			temp_underwaterFogDist = underwaterFogDist;
			temp_underwaterFogSpread = underwaterFogSpread;
			temp_underDarkRange = underDarkRange;
		}




	}
}