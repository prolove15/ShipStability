//
//  Attach this to a scene object, then press the 'convert' option to convert the object to a Suimono Surface.
//	Your original object will stay in the scene but will be automatically disabled.
//

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
	using UnityEditor;
#endif



[ExecuteInEditMode]
public class utility_ConvertToSurface : MonoBehaviour {

	public bool convertToSuimono = false;

	private Suimono.Core.SuimonoModule moduleObject;
	private Suimono.Core.SuimonoObject surfaceComponent;
	private GameObject mainObj;
	private Transform surfaceObj;
	private Transform scaleObj;
	private Renderer objRenderer;
	private MeshFilter objMeshFilter;
	private Mesh objMesh;


	void Start () {
		#if UNITY_EDITOR
			moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
		#endif
	}
	

	void LateUpdate () {
	#if UNITY_EDITOR

		if (convertToSuimono){
			convertToSuimono = false;
			if (CheckAllResources()){
				
				if (moduleObject != null){

					if (moduleObject.suimonoModuleLibrary != null){
						if (moduleObject.suimonoModuleLibrary.surfaceObject != null){

							//Disconnect from prefab;
							#if UNITY_EDITOR
							#if !UNITY_2018_3_OR_NEWER
								PrefabUtility.DisconnectPrefabInstance(this.gameObject);
							#endif
							#endif

							Debug.Log("Passed!");

							//instantiate new surface object with current object settings
							mainObj = Instantiate(moduleObject.suimonoModuleLibrary.surfaceObject, transform.position, transform.rotation) as GameObject;
							mainObj.GetComponent<Transform>().localScale = transform.localScale;
							mainObj.name = "SUIMONO_Surface_"+gameObject.name;

							//Set Surface properties
							surfaceObj = mainObj.GetComponent<Transform>().Find("Suimono_Object");
							surfaceComponent = mainObj.GetComponent<Suimono.Core.SuimonoObject>() as Suimono.Core.SuimonoObject;
							surfaceComponent.enableCustomMesh = true;
							surfaceComponent.customMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
							surfaceObj.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

							//Set Scale properties
							scaleObj = mainObj.GetComponent<Transform>().Find("Suimono_ObjectScale");
							scaleObj.gameObject.SetActive(false);

							//Remove original object
							gameObject.SetActive(false);


						}
					}
				}
				else {
					Debug.Log("Suimono Module not found!");
				}
			}
		}

	#endif
	}




	private bool CheckAllResources(){

		bool passComponents = true;

		if (gameObject.GetComponent<Renderer>() == null){
			passComponents = false;
			Debug.Log("GameObject requires a <Renderer> Component!");
		}
		if (gameObject.GetComponent<MeshFilter>() == null){
			passComponents = false;
			Debug.Log("GameObject requires a <MeshFilter> Component!");
		} else {
			if (gameObject.GetComponent<MeshFilter>().sharedMesh == null){
				passComponents = false;
				Debug.Log("MeshFilter requires a Mesh!");	
			}
		}

		return passComponents;
	}
}
