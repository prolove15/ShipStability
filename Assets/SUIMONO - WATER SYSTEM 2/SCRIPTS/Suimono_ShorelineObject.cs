using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
	using UnityEditor;
#endif


namespace Suimono.Core
{

	[ExecuteInEditMode]
	public class Suimono_ShorelineObject : MonoBehaviour {

		//PUBLIC VARIABLES
		public int lodIndex;
		public int shorelineModeIndex;
		public List<string> shorelineModeOptions = new List<string>(){"Auto-Generate","Custom Texture"};
		public int shorelineRunIndex;
		public List<string> shorelineRunOptions = new List<string>(){"At Start","Continuous"};
		public Transform attachToSurface;
		public bool autoPosition = true;
		public float maxDepth = 25.0f;
		public float sceneDepth = 14.5f;
		public float shoreDepth = 27.7f;
		public bool debug = false;
		public string suimonoVersionNumber;
		public int depthLayer = 2;
		public List<string> suiLayerMasks = new List<string>();
		public Texture2D customDepthTex;
		public int useResolution = 512;

		//PRIVATE VARIABLES
		private Material useMat;
		private Texture reflTex;
		private Texture envTex;
		private Matrix4x4 MV;
		private Camera CamInfo;
		private Suimono.Core.cameraTools CamTools;
		private Suimono.Core.SuimonoCamera_depth CamDepth;

		private float curr_sceneDepth;
		private float curr_shoreDepth;
		private float curr_foamDepth;
		private float curr_edgeDepth;

		private Vector3 currPos;
		private Vector3 currScale;
		private Quaternion currRot;

		private Vector4 camCoords = new Vector4(1f,1f,0f,0f);
		private Material localMaterial;
		private Renderer renderObject;
		private MeshFilter meshObject;
		private Material matObject;
		public Suimono.Core.SuimonoModule moduleObject;
		private float maxScale;

		private int i;
		private string layerName;
		private bool hasRendered = false;
		private bool renderPass = true;
		private int saveMode = -1;
		private Vector3 gizPos;


		void OnDrawGizmos (){
			gizPos = transform.position;
			gizPos.y += 0.03f;
			Gizmos.DrawIcon(gizPos, "gui_icon_shore.psd", true);
			gizPos.y -= 0.03f;
		}


		void Start () {

			//DISCONNECT FROM PREFAB
			#if UNITY_EDITOR
			#if !UNITY_2018_3_OR_NEWER
				PrefabUtility.DisconnectPrefabInstance(this.gameObject);
			#endif
			#endif
			
			//turn off debig at start
			if (Application.isPlaying){
				debug = false;
			}

			//get main object
			if (GameObject.Find("SUIMONO_Module") != null){
				moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
				suimonoVersionNumber = moduleObject.suimonoVersionNumber;
			}

			//setup camera
			CamInfo = transform.Find("cam_LocalShore").gameObject.GetComponent<Camera>();
			CamInfo.depthTextureMode = DepthTextureMode.DepthNormals;
			CamTools = transform.Find("cam_LocalShore").gameObject.GetComponent<Suimono.Core.cameraTools>() as Suimono.Core.cameraTools;
			CamDepth = transform.Find("cam_LocalShore").gameObject.GetComponent<Suimono.Core.SuimonoCamera_depth>() as Suimono.Core.SuimonoCamera_depth;

			//setup renderer
			renderObject = gameObject.GetComponent<Renderer>() as Renderer;
			meshObject = gameObject.GetComponent<MeshFilter>() as MeshFilter;

			//find parent surface
			if (transform.parent){
				if (transform.parent.gameObject.GetComponent<Suimono.Core.SuimonoObject>() != null){
					attachToSurface = transform.parent;
				}
			}

			//turn on surface
			if (attachToSurface != null){
				attachToSurface.Find("Suimono_Object").gameObject.GetComponent<Renderer>().enabled = true;
			}

			//setup material
			matObject = new Material(Shader.Find("Suimono2/Suimono2_FX_ShorelineObject"));
			renderObject.material = matObject;

			//set rendering flag
			hasRendered = false;

		}
		



		void LateUpdate () {
		

			if (moduleObject != null){

				//version number
				suimonoVersionNumber = moduleObject.suimonoVersionNumber;

				//set layers and tags
				gameObject.layer = moduleObject.layerDepthNum;
				CamInfo.gameObject.layer = moduleObject.layerDepthNum;
				CamInfo.farClipPlane = maxDepth;
				
				gameObject.tag = "Untagged";
				CamInfo.gameObject.tag = "Untagged";
				


				//---------
				//set layer mask array
				suiLayerMasks = new List<string>();
				for (i = 0; i < 32; i++){
					layerName = LayerMask.LayerToName(i);
					suiLayerMasks.Add(layerName);
				}


				if (!Application.isPlaying){
					if (attachToSurface != null){
						if (debug){
							attachToSurface.Find("Suimono_Object").gameObject.GetComponent<Renderer>().enabled = false;
						} else {
							attachToSurface.Find("Suimono_Object").gameObject.GetComponent<Renderer>().enabled = true;
						}
					}
				}


				if (shorelineModeIndex == 0){
					// set camera culling
					if (CamInfo != null){
						CamInfo.enabled = true;
						CamInfo.cullingMask = depthLayer;
					}
				} else {
					if (CamInfo != null) CamInfo.enabled = false;
				}


				//Handle Debug Mode
				if (debug){
					if (renderObject != null) renderObject.hideFlags = HideFlags.None;
					if (meshObject != null) meshObject.hideFlags = HideFlags.None;
					if (matObject != null) matObject.hideFlags = HideFlags.None;
					if (shorelineModeIndex == 0){
						if (CamInfo != null) CamInfo.gameObject.hideFlags = HideFlags.None;
						if (CamTools != null){
							CamTools.executeInEditMode = true;
							CamTools.CameraUpdate();
						}
					}
					if (renderObject != null) renderObject.enabled = true;
				} else {
					if (renderObject != null) renderObject.hideFlags = HideFlags.HideInInspector;
					if (meshObject != null) meshObject.hideFlags = HideFlags.HideInInspector;
					if (matObject != null) matObject.hideFlags = HideFlags.HideInInspector;
					if (shorelineModeIndex == 0){
						if (CamInfo != null) CamInfo.gameObject.hideFlags = HideFlags.HideInHierarchy;
						if (CamTools != null) CamTools.executeInEditMode = false;
					}
					if (!Application.isPlaying && renderObject != null){
						renderObject.enabled = false;
					} else {
						renderObject.enabled = true;
					}
				}
				//---------

				

				//flag mode setting
				if (saveMode != shorelineModeIndex){
					saveMode = shorelineModeIndex;
					hasRendered = false;
				}
				

				//CALCULATE RENDER PASS FLAG
				renderPass = true;
				if (shorelineModeIndex == 0){
					if (shorelineRunIndex == 0 && hasRendered && Application.isPlaying) renderPass = false;
					if (shorelineRunIndex == 1) renderPass = true;
				}
				if (shorelineModeIndex == 1 && hasRendered && Application.isPlaying) renderPass = false;




				//RENDER
				if (!renderPass){

					if (CamInfo != null) CamInfo.enabled = false;
					if (CamTools != null) CamTools.enabled = false;

				} else {

					if (CamInfo != null) CamInfo.enabled = true;
					if (CamTools != null) CamTools.enabled = true;
					if (CamDepth != null) CamDepth.enabled = true;

					//set Depth Thresholds
					if (shorelineModeIndex == 0){
						CamDepth._sceneDepth = sceneDepth;
						CamDepth._shoreDepth = shoreDepth;
					}

					if (attachToSurface != null){

						//force y height
						transform.localScale = new Vector3(transform.localScale.x,1f,transform.localScale.z);

						//force y position based on surface attachment
						if (attachToSurface != null && autoPosition){
							transform.position = new Vector3(transform.position.x,attachToSurface.position.y,transform.position.z);
						}


						//AUTO GENERATION MODE --------------------------------------------------
						if (shorelineModeIndex == 0){

							//Set object and camera Projection Size
							maxScale = Mathf.Max(transform.localScale.x,transform.localScale.z);
							CamInfo.orthographicSize = maxScale * 20f;
							if (transform.localScale.x < transform.localScale.z){
								camCoords = new Vector4(transform.localScale.x/transform.localScale.z,
									1.0f,
									0.5f-((transform.localScale.x/transform.localScale.z)*0.5f),
									0.0f);
							} else if (transform.localScale.x > transform.localScale.z){
								camCoords = new Vector4(1f,
									transform.localScale.z/transform.localScale.x,
									0.0f,
									0.5f-((transform.localScale.z/transform.localScale.x)*0.5f));
							}
							CamTools.surfaceRenderer.sharedMaterial.SetColor("_Mult",camCoords);

							//Update when moved,rotated, or scaled, or edited
							if (CamTools != null){
								if (currPos != transform.position){
									currPos = transform.position;
									CamTools.CameraUpdate();
								}
								if (currScale != transform.localScale){
									currScale = transform.localScale;
									CamTools.CameraUpdate();
								}
								if (currRot != transform.rotation){
									currRot = transform.rotation;
									CamTools.CameraUpdate();
								}

								if (curr_sceneDepth != sceneDepth){
									curr_sceneDepth = sceneDepth;
									CamTools.CameraUpdate();
								}
								if (curr_shoreDepth != shoreDepth){
									curr_shoreDepth = shoreDepth;
									CamTools.CameraUpdate();
								}

								if (Application.isPlaying) CamTools.CameraUpdate();

							}
						}


						//CUSTOM TEXTURE MODE --------------------------------------------------
						if (shorelineModeIndex == 1){
							if (customDepthTex != null){
								if (renderObject != null){
									renderObject.sharedMaterial.SetColor("_Mult",new Vector4(1f,1f,0f,0f));
									renderObject.sharedMaterial.SetTexture("_MainTex",customDepthTex);
								}
							}
							
						}


						if (Application.isPlaying && Time.time > 1f) hasRendered = true;

					}
				}
			}


		}
	}
}