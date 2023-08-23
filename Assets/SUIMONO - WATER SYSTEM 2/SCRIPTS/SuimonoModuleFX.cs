using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Sui_FX_ClampType{
		none,atSurface,belowSurface,aboveSurface
		}



namespace Suimono.Core
{

	[ExecuteInEditMode]
	public class SuimonoModuleFX : MonoBehaviour {



		//PUBLIC VARIABLES
		public string[] effectsLabels;
		public Transform[] effectsSystems;
		public Sui_FX_ClampType systemClampType;
		public Transform[] fxObjects;
		public ParticleSystem[] fxParticles;
		public int[] clampIndex;
		public List<string> clampOptions = new List<string>(){"No Clamp","Clamp to Surface","Keep Below Surface","Keep Above Surface"};
		public List<ParticleSystem.Particle> particleReserve = new List<ParticleSystem.Particle>();
	
		//PRIVATE VARIABLES
		private Transform fxParentObject;
		private Suimono.Core.SuimonoModule moduleObject;
		private int fx;
		private int px;
		private float currPXWaterPos;
		private ParticleSystem useParticleComponent;
		private ParticleSystem.Particle[] setParticles;
		private Transform[] tempSystems;
		private int[] tempClamp;
		private int aR;
		private int efx;
		private int epx;
		private int sx;
		private int endLP;
		private int setInt;
		public List<string> sysNames = new List<string>();
		public int sN;
		public int s;
		public string setName;


		// create a simple cheap "smear" effect on the InvokeRepeating processor load.
		// by shifting the work into rough groups via a simple static int, that we loop over.
		 
		// our global counter
		static int staggerOffset = 0;
		 
		// our loop, we chose groups of roughly 20
		static int staggerModulus = 20;
		 
		// our actual stagger value 
		private float stagger;




		void Start () {
	
			//set objects
			fxParentObject = this.transform.Find("_particle_effects");
			moduleObject = (Suimono.Core.SuimonoModule) FindObjectOfType(typeof(Suimono.Core.SuimonoModule));
			
			//instantiate systems
			if (Application.isPlaying){
			if (effectsSystems.Length > 0 && fxParentObject != null){
				var instPos = new Vector3(transform.position.x,-10000.0f,transform.position.z);
				fxObjects = new Transform[effectsSystems.Length];
				fxParticles = new ParticleSystem[effectsSystems.Length];

				for (int fx = 0; fx < (effectsSystems.Length); fx++){
					var fxObjectPrefab = Instantiate(effectsSystems[fx], instPos, transform.rotation) as Transform;
					fxObjectPrefab.transform.parent = fxParentObject.transform;
					fxObjects[fx] = (fxObjectPrefab);
					fxParticles[fx] = fxObjectPrefab.gameObject.GetComponent<ParticleSystem>();
				}
			}
			}
			
			//do clamp checks at 6fps
		    staggerOffset++;
		    stagger = (staggerOffset+0f) * 0.05f  ;
		    staggerOffset = staggerOffset % staggerModulus;
		    
			var clampSpeed = 1.0f/4.0f;
			InvokeRepeating("ClampSystems",0.15f+stagger,clampSpeed);
			InvokeRepeating("UpdateSystems",0.2f+stagger,1.0f);
		}

		


		void LateUpdate () {

			//get objects while in editor mode
			#if UNITY_EDITOR
			if (!Application.isPlaying){
				if (moduleObject == null){
					if (GameObject.Find("SUIMONO_Module")){
						moduleObject = GameObject.Find("SUIMONO_Module").GetComponent<Suimono.Core.SuimonoModule>() as Suimono.Core.SuimonoModule;
					}
				}
			}
			#endif
			

			if (!Application.isPlaying){
				sysNames = new List<string>();	
				sysNames.Add("None");
				for (sN = 0; sN < effectsSystems.Length; sN++){
					setName = "---";
					if (effectsSystems[sN] != null) setName = effectsSystems[sN].transform.name;
						for (s = 0; s < sN; s++){
							setName += " ";
						}
					sysNames.Add(setName);
				}
			}
		}





		void UpdateSystems(){

			if (Application.isPlaying){	
				sysNames = new List<string>();
				sysNames.Add("None");
				for (sN = 0; sN < effectsSystems.Length; sN++){
					setName = "---";
					if (effectsSystems[sN] != null) setName = effectsSystems[sN].transform.name;
						for (s = 0; s < sN; s++){
							setName += " ";
						}
					sysNames.Add(setName);
				}
			}
		}




		void ClampSystems(){

			for (fx = 0; fx < fxObjects.Length; fx++){
				if (fxObjects[fx] != null){
				if (clampIndex[fx] != 0){

					currPXWaterPos = 0.0f;
					
					//get particles
					useParticleComponent = fxParticles[fx];
					if (setParticles == null) setParticles = new ParticleSystem.Particle[useParticleComponent.particleCount];
					useParticleComponent.GetParticles(setParticles);
					//set particles
					if (useParticleComponent.particleCount > 0.0f){
					for (px = 0; px < useParticleComponent.particleCount; px++){

						currPXWaterPos = moduleObject.SuimonoGetHeight(setParticles[px].position,"surfaceLevel");

						//Clamp to Surface
						if (clampIndex[fx] == 1){
							setParticles[px].position = new Vector3(setParticles[px].position.x,currPXWaterPos+0.2f,setParticles[px].position.z);
						}
						//Clamp Under Water
						if (clampIndex[fx] == 2){
							if (setParticles[px].position.y > currPXWaterPos-0.2f){
								setParticles[px].position = new Vector3(setParticles[px].position.x,currPXWaterPos-0.2f,setParticles[px].position.z);
							}
						}
						//Clamp Above Water
						if (clampIndex[fx] == 3){
							if (setParticles[px].position.y < currPXWaterPos+0.2f){
								setParticles[px].position = new Vector3(setParticles[px].position.x,currPXWaterPos+0.2f,setParticles[px].position.z);
							}
						}
					}
					useParticleComponent.SetParticles(setParticles,setParticles.Length);
					useParticleComponent.Play();
					}
				}
				}
			}
		}



		public void AddSystem(){

			tempSystems  = effectsSystems;
			tempClamp  = clampIndex;
			
			effectsSystems = new Transform[tempSystems.Length+1];
			clampIndex = new int[tempClamp.Length+1];
				
			for (aR = 0; aR < tempSystems.Length; aR++){
				effectsSystems[aR] = tempSystems[aR];
				clampIndex[aR] = tempClamp[aR];
			}
			effectsSystems[tempSystems.Length] = null;
			clampIndex[tempClamp.Length] = 0;
		}




		public void AddParticle( ParticleSystem.Particle particleData ){
			particleReserve.Add(particleData);
		}



		IEnumerator updateFX(){
			
			//EMIT New Particles
			for (efx = 0; efx < effectsSystems.Length; efx++){
				for (epx = 0; epx < particleReserve.Count; epx++){
					if (Mathf.Floor(particleReserve[epx].angularVelocity) == efx){
						fxParticles[fx].Emit(1);
					}
				}				
			}
			
			//SET NEW Particle position and behaviors
			for (fx = 0; fx < effectsSystems.Length; fx++){
				for (px = 0; px < particleReserve.Count; px++){
					if (Mathf.Floor(particleReserve[px].angularVelocity) == fx){

						//get particles
						useParticleComponent = fxParticles[fx];
						if (setParticles == null) setParticles = new ParticleSystem.Particle[useParticleComponent.particleCount];
						useParticleComponent.GetParticles(setParticles);
						//set particles
						for (sx = (useParticleComponent.particleCount-1); sx < useParticleComponent.particleCount; sx++){
							//set position
							setParticles[px].position = particleReserve[px].position;
							
							//set variables
							#if UNITY_5_4_OR_NEWER
								setParticles[px].startSize = particleReserve[px].startSize;
							#else
								setParticles[px].size = particleReserve[px].size;
							#endif

							setParticles[px].rotation = particleReserve[px].rotation;
							setParticles[px].velocity = particleReserve[px].velocity;
						}
						useParticleComponent.SetParticles(setParticles,setParticles.Length);
					}
				}						
			}

			yield return null;
			if (particleReserve == null) particleReserve = new List<ParticleSystem.Particle>();
		}



		public void DeleteSystem(int sysNum){

			tempSystems  = effectsSystems;
		 	tempClamp  = clampIndex;
		 	
			endLP = tempSystems.Length-1;
			if (endLP <= 0){
				endLP = 0;
				
				if (effectsSystems == null) effectsSystems = new Transform[tempSystems.Length+1];
				if (clampIndex == null) clampIndex = new int[tempSystems.Length+1];
				

			} else {

				//if (effectsSystems == null) effectsSystems = new Transform[endLP];
				//if (clampIndex == null) clampIndex = new int[endLP];

				tempSystems = new Transform[endLP];
				setInt = 0;
				
				for (aR = 0; aR < endLP+1; aR++){

					
					//if (setInt < tempSystems.Length){
					//	effectsSystems[aR] = tempSystems[setInt];
					//	clampIndex[aR] = tempClamp[setInt];
					//}

					if (aR != sysNum){
						tempSystems[setInt] = effectsSystems[aR];
						setInt += 1;
					}

				}
				
				effectsSystems = tempSystems;
				//Debug.Log("current:"+tempSystems.Length);

			}


		}




		void OnApplicationQuit(){
			for (fx=0; fx < (effectsSystems.Length); fx++){
				Destroy(fxObjects[fx]);
			}
		}




	}
}