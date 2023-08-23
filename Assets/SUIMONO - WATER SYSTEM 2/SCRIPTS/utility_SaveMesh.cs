//
//  This Utility will convert a 'mesh-in memory' into a mesh stored in your project for use elsehwere.
// 	Simply attach this script to your mesh game object, add a name to the 'use name' setting, and click Save.
//


using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
	using UnityEditor;
#endif


[ExecuteInEditMode]
public class utility_SaveMesh : MonoBehaviour {


	//PUBLIC VARIABLES
	public bool saveAsset = false;
	public string useName = "";

	//PRIVATE VARIABLES
	//private Mesh mesh = new Mesh();


	void Start () {
	
	}
	

	void LateUpdate () {
		#if UNITY_EDITOR
			if (saveAsset && useName != ""){
				saveAsset = false;
				SaveAsset();
			}
		#endif
	}


	void SaveAsset () {
		#if UNITY_EDITOR
			Mesh mesh = new Mesh();
			mesh = GetComponent<MeshFilter>().sharedMesh;
			mesh.name = useName;
			mesh.RecalculateNormals();
			//mesh.Optimize();

			if (mesh != null && useName != null && useName !=""){ 
				AssetDatabase.CreateAsset(mesh, "Assets/SUIMONO - WATER SYSTEM 2/MESH/"+useName+".asset");
				Debug.Log("Asset Created at: "+AssetDatabase.GetAssetPath(mesh)+"!");
			}

		#endif
		}


}
