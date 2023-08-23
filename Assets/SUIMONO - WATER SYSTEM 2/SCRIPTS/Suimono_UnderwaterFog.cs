using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Suimono.Core
{

	[AddComponentMenu("Image Effects/Suimono/UnderwaterFX")]
	public class Suimono_UnderwaterFog : MonoBehaviour {

		//Public Variables
		public bool showScreenMask = false;
		public bool doTransition = false;
		public bool cancelTransition = false;
		public bool useUnderSurfaceView = false;
		public bool distanceFog = true;
		public bool useRadialDistance = true;
		public bool heightFog = false;
		public float height = 1.0f;
		public float heightDensity = 2.0f;
		public float startDistance = 0.0f;
		public float fogStart = 0.0f;
		public float fogEnd = 20.0f;
		public float refractAmt = 0.005f;
		public float refractSpd = 1.5f;
		public float refractScale = 0.5f;
		public float lightFactor = 1.0f;
		public Color underwaterColor;
		public float dropsTime = 2.0f;
		public float wipeTime = 1.0f;
		public float transitionStrength = 1.0f;
		public int iterations = 2;
		public float blurSpread = 1.0f;
		public float darkRange = 40.0f;
		public float heightDepth = 1.0f;
		public float hFac = 0.0f;
		public Texture distortTex;
		//public Texture mask1Tex;
		public Texture mask2Tex;
		public Shader fogShader = null;
		public Material fogMaterial = null;


		//Private Variables
		private Suimono.Core.SuimonoModule moduleObject;
		private Suimono.Core.SuimonoModuleLib moduleLibrary;
		private float trans1Time = 1.1f;
		private float trans2Time = 1.1f;
		private int randSeed;
		private Suimono.Core.Random dropRand;
		private Vector2 dropOff;
		private Camera cam;
		private Transform camtr;
		private int pass;
		private int rtW;
		private int rtH;
		private RenderTexture buffer;
		private int i = 0;
		private RenderTexture buffer2;
		private Vector3 camPos;
		private float FdotC;
		private float paramK;
		private float sceneStart;
		private float sceneEnd;
		private Vector4 sceneParams;
		private float diff;
		private float invDiff;
		private Matrix4x4 frustumCorners;
		private float fovWHalf;
		private Vector3 toRight;
		private Vector3 toTop;
		private Vector3 topLeft;
		private float camScale;
		private Vector3 topRight;
		private Vector3 bottomRight;
		private Vector3 bottomLeft;
		private float offc;
		private float off;
		private Transform trackobject;

		private float _deltaTime;

		void Start () {

			cam = gameObject.GetComponent<Camera>();
			camtr = cam.transform;

			if (GameObject.Find("SUIMONO_Module") != null){
				moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
				moduleLibrary = (SuimonoModuleLib) FindObjectOfType(typeof(SuimonoModuleLib));
			    //moduleLibrary = GameObject.Find("SUIMONO_Module").GetComponent<SuimonoModuleLib>();
			}
			
		    if (moduleLibrary != null){
		    	distortTex = moduleLibrary.texNormalC;
		    	//mask1Tex = moduleLibrary.texHeightC;
		    	mask2Tex = moduleLibrary.texDrops;
			}

			randSeed = System.Environment.TickCount;
			dropRand = new Suimono.Core.Random(randSeed);

		    fogShader = Shader.Find("Hidden/SuimonoUnderwaterFog");
		    fogMaterial = new Material(fogShader);
		}



		void LateUpdate () {

			//update random reference
			if (dropRand == null) dropRand = new Suimono.Core.Random(randSeed);

			//update timing
			_deltaTime = Time.deltaTime;
			
			//Handle Transition Settings
			if (cancelTransition){
				doTransition = false;
				cancelTransition = false;
				trans1Time = 1.1f;
				trans2Time = 1.1f;
			}

			if (doTransition){
				doTransition = false;
				trans1Time = 0.0f;
				trans2Time = 0.0f;
				dropOff = new Vector2( dropRand.Next(0.0f,1.0f), dropRand.Next(0.0f,1.0f));
			}

			trans1Time += (_deltaTime*0.7f*wipeTime);
			trans2Time += (_deltaTime*0.1f*dropsTime);
		}






		//[ImageEffectOpaque]
		void OnRenderImage (RenderTexture source, RenderTexture destination){

			Graphics.Blit (source, destination);

			frustumCorners = Matrix4x4.identity;
			fovWHalf = cam.fieldOfView * 0.5f;
			toRight = camtr.right * cam.nearClipPlane * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * cam.aspect;
			toTop = camtr.up * cam.nearClipPlane * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);
			topLeft = (camtr.forward * cam.nearClipPlane - toRight + toTop);
			camScale = topLeft.magnitude * cam.farClipPlane/cam.nearClipPlane;

			topLeft.Normalize();
			topLeft *= camScale;

			topRight = (camtr.forward * cam.nearClipPlane + toRight + toTop);
			topRight.Normalize();
			topRight *= camScale;

			bottomRight = (camtr.forward * cam.nearClipPlane + toRight - toTop);
			bottomRight.Normalize();
			bottomRight *= camScale;

			bottomLeft = (camtr.forward * cam.nearClipPlane - toRight - toTop);
			bottomLeft.Normalize();
			bottomLeft *= camScale;

			frustumCorners.SetRow (0, topLeft);
			frustumCorners.SetRow (1, topRight);
			frustumCorners.SetRow (2, bottomRight);
			frustumCorners.SetRow (3, bottomLeft);


			//set default values based on water surface height
			if (heightFog && this.transform.parent != null){
				height = this.transform.parent.transform.position.y + 1.0f;
				heightDensity = 2.0f;
			}

			camPos = camtr.position;
			FdotC = camPos.y-height;
			paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
			sceneStart = fogStart;
			sceneEnd = fogEnd;

			diff = sceneEnd - sceneStart;
			invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
			sceneParams.x = 0.0f;
			sceneParams.y = 0.0f;
			sceneParams.z = -invDiff;
			sceneParams.w = sceneEnd * invDiff;


			if (fogMaterial != null){
				fogMaterial.SetMatrix ("_FrustumCornersWS", frustumCorners);
				fogMaterial.SetVector ("_CameraWS", camPos);
				fogMaterial.SetVector ("_HeightParams", new Vector4 (height, FdotC, paramK, heightDensity*0.5f));
				fogMaterial.SetVector ("_DistanceParams", new Vector4 (-Mathf.Max(startDistance,0f), 0f, 0f, 0f));

				fogMaterial.SetVector ("_SceneFogParams", sceneParams);
				fogMaterial.SetVector ("_SceneFogMode", new Vector4(1f, useRadialDistance ? 1f : 0f, 0f, 0f));
				fogMaterial.SetColor ("_underwaterColor", underwaterColor);

				if (distortTex != null){
			    	fogMaterial.SetTexture("_underwaterDistort",distortTex);
			    	fogMaterial.SetFloat("_distortAmt",refractAmt);
			    	fogMaterial.SetFloat("_distortSpeed",refractSpd);
			    	fogMaterial.SetFloat("_distortScale",refractScale);
					fogMaterial.SetFloat("_lightFactor",lightFactor);
				}
				if (distortTex != null){
					fogMaterial.SetTexture("_distort1Mask",distortTex);
				}
				if (mask2Tex != null){
					fogMaterial.SetTexture("_distort2Mask",mask2Tex);
				}


				fogMaterial.SetFloat("_trans1",trans1Time);
				fogMaterial.SetFloat("_trans2",trans2Time);
				fogMaterial.SetFloat("_transStrength", transitionStrength);
				fogMaterial.SetFloat("_dropOffx",dropOff.x);
				fogMaterial.SetFloat("_dropOffy",dropOff.y);
				
				fogMaterial.SetFloat("_showScreenMask",showScreenMask ? 1f : 0f);

				blurSpread = Mathf.Clamp01(blurSpread);
				fogMaterial.SetFloat("_blur",blurSpread);


				//calculate heightDepth for underwater darkening
				if (moduleObject != null){
					if (moduleObject.setTrack != null){
						trackobject = moduleObject.setTrack.transform;
					} else {
						if (moduleObject.setCamera != null){
							trackobject = moduleObject.setCamera.transform;
						}
					}

					if (trackobject != null){
						hFac = Mathf.Clamp((11.5f) - trackobject.localPosition.y, 0.0f,500.0f);
					}
					
					heightDepth = hFac;
					hFac = Mathf.Clamp01(Mathf.Lerp(-0.2f,1f,Mathf.Clamp01(hFac/darkRange)));
					fogMaterial.SetFloat("_hDepth", hFac);

					fogMaterial.SetFloat("_enableUnderwater",moduleObject.enableUnderwaterFX ? 1f : 0f);
				}


				//blur
			    // Copy source to the 4x4 smaller texture.
			    rtW = source.width/4;
			    rtH = source.height/4;
			    buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
			    DownSample4x (source, buffer);

			    // Blur the small texture
			    for(i = 0; i < iterations; i++){
			        buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
			        FourTapCone (buffer, buffer2, i);
			        RenderTexture.ReleaseTemporary(buffer);
			        buffer = buffer2;
			    }

			    Graphics.Blit(buffer, destination);
			    RenderTexture.ReleaseTemporary(buffer);

					pass = 0;
					if (distanceFog && heightFog)
						pass = 0; // distance + height
					else if (distanceFog)
						pass = 1; // distance only
					else
						pass = 2; // height only

				CustomGraphicsBlit (source, destination, fogMaterial, pass);
			}

		}



		void CustomGraphicsBlit (RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr){
			RenderTexture.active = dest;
			fxMaterial.SetTexture ("_MainTex", source);

			GL.PushMatrix ();
			GL.LoadOrtho ();
			fxMaterial.SetPass (passNr);

			GL.Begin (GL.QUADS);
			GL.MultiTexCoord2 (0, 0.0f, 0.0f);
			GL.Vertex3 (0.0f, 0.0f, 3.0f); // BL
			GL.MultiTexCoord2 (0, 1.0f, 0.0f);
			GL.Vertex3 (1.0f, 0.0f, 2.0f); // BR
			GL.MultiTexCoord2 (0, 1.0f, 1.0f);
			GL.Vertex3 (1.0f, 1.0f, 1.0f); // TR
			GL.MultiTexCoord2 (0, 0.0f, 1.0f);
			GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL
			GL.End ();
			GL.PopMatrix ();
		}





		// Performs one blur iteration.
		void FourTapCone (RenderTexture source, RenderTexture dest, int iteration){
		    offc = 0.5f + iteration*blurSpread*2;
		    Graphics.BlitMultiTap (source, dest, fogMaterial,
		                           new Vector2(-offc, -offc),
		                           new Vector2(-offc,  offc),
		                           new Vector2( offc,  offc),
		                           new Vector2( offc, -offc)
		        );
		}

		// Downsamples the texture to a quarter resolution.
		void DownSample4x (RenderTexture source, RenderTexture dest){
		    off = 1.0f;
		    Graphics.BlitMultiTap (source, dest, fogMaterial,
		                           new Vector2(-off, -off),
		                           new Vector2(-off,  off),
		                           new Vector2( off,  off),
		                           new Vector2( off, -off)
		        );
		}








	}
}
