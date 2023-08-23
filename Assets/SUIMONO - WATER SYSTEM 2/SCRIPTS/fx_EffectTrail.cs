using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Suimono.Core
{

	public class SuimonoTrailSection{
		public Vector3 point;
		public Vector3 upDir;
		public float time;
		public Color vCol;
	}


	public class fx_EffectTrail : MonoBehaviour {

/*
		public int numberSections = 0;
		public float startWidth = 1.0f;
		public float endWidth = 2.0f;
		public float time = 2.0f;
		public float minDistance = 0.1f;
		public float setHeight = 0.0f;
		public float maxHeight = 1.8f;
		public float normalAmt = 1.0f;
		public float foamAmt = 1.0f;
		public float heightAmt = 1.0f;
		public float depthRange1 = 1.0f;
		public float depthRange2 = 1.0f;
		public float depthStretch = 1.0f;

		private	float getPos = 0.0f;
		private Vector3 savePos;
		private List<int> sections = new List<int>();
		private Suimono.Core.SuimonoTrailSection section;
		private Suimono.Core.SuimonoModule suimonoModule;
		private bool isUnderwater = false;
		private Color setBlendColor = new Color(1f,1f,1f,1f);
		private Color fadeColor = new Color(1f,1f,1f,1f);
		private float depth = 0.0f;
		private float surfaceY = 10.2f;
		private float[] heightData;
		private Renderer trailRenderer;
		private float resetTime = 0.0f;
		private Color startColor;
		private Color endColor = new Color(0f,0f,0f,0f);






		void Start () {
			//get objects
			if (GameObject.Find("SUIMONO_Module") != null){
				suimonoModule = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
			}
			trailRenderer = this.gameObject.GetComponent<Renderer>();
		}
		



		void LateUpdate(){
			if (suimonoModule != null){
				
				//force layer
				gameObject.layer = suimonoModule.layerScreenFXNum;

				transform.localEulerAngles = new Vector3(0f,90f,0f);
				foamAmt = Mathf.Clamp01(foamAmt);
				normalAmt = Mathf.Clamp01(normalAmt);
				heightAmt = Mathf.Clamp01(heightAmt);

				EffectTrailUpdate();
			}
		}


		void UnderwaterCheck(){

			if (suimonoModule != null){
				//getPos = suimonoModule.SuimonoGetHeight(transform.position,"baseLevel");

				//underwater check
				//heightData = suimonoModule.SuimonoGetHeightAll(this.transform.position);
				
				depth = 0.5f;
				surfaceY = heightData[2];

				if (heightData[3] > 0.0f && heightData[3] <= maxHeight && heightData[4] == 1.0f){
					isUnderwater = true;
				} else {
					isUnderwater = false;
				}
				
				setBlendColor = Color.Lerp(new Color(0f,0f,0f,0f),new Color(1f,0f,0f,1f),(depth-depthRange1) / depthStretch);
				setBlendColor = Color.Lerp(setBlendColor,new Color(setBlendColor.r,1f,0f,1f),(depth-depthRange2) / depthStretch);
				setBlendColor.b = 1.0f;
			}
		}





		void EffectTrailUpdate() {

			numberSections = sections.Count;

			//reset blending color when not moving
			if (Vector3.Distance(new Vector3(transform.position.x,0.0f,transform.position.z),new Vector3(savePos.x,0.0f,savePos.z)) > (0.5f)){
				resetTime = 0.0f;
				savePos = transform.position;
			} else {
				resetTime += Time.deltaTime;
			}

			fadeColor = Color.Lerp(new Color(1f,1f,1f,1f),new Color(0f,0f,0f,0f),resetTime);
			if (resetTime >= time) sections = null;

			if (sections == null) sections = new List<int>();
			Vector3 position = transform.position;
			float now = Time.time;


			UnderwaterCheck();

			// Add a new trail section
			if (isUnderwater){
				section = new SuimonoTrailSection();
				section.point = position;

				setHeight = surfaceY;
				if (setHeight > 0.0f){
					section.point.y = setHeight;
				}
				section.upDir = (new Vector3(transform.forward.x,0f,transform.forward.z));
				section.time = now;

				//set vertex color based on water depth
				section.vCol = setBlendColor;
				//sections.Unshift(section);
			}


			// Rebuild the mesh
			Mesh mesh = GetComponent<MeshFilter>().mesh;
			mesh.Clear();
			
			// We need at least 2 sections to create the line
			if (sections.Count < 2)
				return;

			var vertices = new Vector3[sections.Count * 2];
			var colors = new Color[sections.Count * 2];
			var uv = new Vector2[sections.Count * 2];
			int previousSection = sections[0];
			int currentSection = sections[0];

			// Use matrix instead of transform.TransformPoint for performance reasons
			var localSpaceTransform = transform.worldToLocalMatrix;

			// Generate vertex, uv and colors
			for (int i = 0; i < sections.Count; i++)
			{
				previousSection = currentSection;
				currentSection = sections[i];
				
				// Calculate u for texture uv and color interpolation
				var u = 0.0f;		
				//if (i != 0) u = Mathf.Clamp01((Time.time - currentSection.time) / time);
				
				// Calculate upwards direction
				//Vector3 upDir = currentSection.upDir;

				// Generate vertices
				float spreadAmt = (startWidth+((endWidth-startWidth)*u));
				var raiseAmt = Vector3.up * Mathf.Lerp(0.0f,0.0f,u);
				vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(currentSection.point + (upDir * spreadAmt) - raiseAmt);
				vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(currentSection.point - (upDir * spreadAmt) - raiseAmt);

				uv[i * 2 + 0] = Vector2(u, 0);
				uv[i * 2 + 1] = Vector2(u, 1);
				
				// fade colors out over time
				startColor.r = foamAmt;
				startColor.g = foamAmt;
				startColor.b = heightAmt;
				startColor.a = normalAmt * 4f;
				var interpolatedColor = Color.Lerp(startColor, endColor, u*0.5f);
				colors[i * 2 + 0] = interpolatedColor * currentSection.vCol;
				colors[i * 2 + 1] = interpolatedColor * currentSection.vCol;

				if (u >= 1.0){
					sections.RemoveAt(i);
				}
			}

			// Generate triangles indices
			var triangles = new int[(sections.length - 1) * 2 * 3];
			for (i = 0 ; i < triangles.length / 6; i++)
			{
				triangles[i * 6 + 0] = i * 2;
				triangles[i * 6 + 1] = i * 2 + 1;
				triangles[i * 6 + 2] = i * 2 + 2;

				triangles[i * 6 + 3] = i * 2 + 2;
				triangles[i * 6 + 4] = i * 2 + 1;
				triangles[i * 6 + 5] = i * 2 + 3;
			}

			// Assign to mesh	
			mesh.vertices = vertices;
			mesh.colors = colors;
			mesh.uv = uv;
			mesh.triangles = triangles;
		}

*/


	}
}