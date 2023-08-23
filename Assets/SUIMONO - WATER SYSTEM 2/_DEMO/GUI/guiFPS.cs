using UnityEngine;
using System.Collections;



public class guiFPS : MonoBehaviour {

	//Public Variables
	public Texture2D showLabel;
	public Vector2 labelOffset = new Vector2(0.5f,0.5f);
	public Color labelColor = new Color(1f,1f,1f,1f);


	//Private variales
	private float updateInterval = 0.3f;
	private string GuiMsg = "---";
	private string VerMsg = "---";
	private float accum = 0.0f;
	private float frames = 0f;
	private float timeleft;

	private Suimono.Core.SuimonoModule moduleObject;
	private Suimono.Core.SuimonoObject oceanObject;

	private string displayMode = "---";
	private bool inputKeyMode = false;

	private string displayPreset = "---";
	private bool inputKeyPreset = false;
	private int currentPreset;
	private int usePreset;



	void Awake(){
		if (GameObject.Find("SUIMONO_Module") != null){
			moduleObject = GameObject.Find("SUIMONO_Module").gameObject.GetComponent<Suimono.Core.SuimonoModule>() as Suimono.Core.SuimonoModule;
		}
		if (GameObject.Find("SUIMONO_Surface_Ocean") != null){
			oceanObject = GameObject.Find("SUIMONO_Surface_Ocean").gameObject.GetComponent<Suimono.Core.SuimonoObject>() as Suimono.Core.SuimonoObject;
		}
	}


	void Start () {
		currentPreset = 0;
	}




	void LateUpdate () {

		//GET Version Number
		if (moduleObject != null){
			VerMsg = "|  Ver. "+moduleObject.suimonoVersionNumber;
		}

		// CALCULATE FPS
	    timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	    ++frames;
	   
	    // Interval ended - update GUI text and start new interval
	    if( timeleft <= 0.0 )
	    {
	        // display two fractional digits (f2 format)
	        GuiMsg = "FPS: "+(accum/frames).ToString("f0");
	        timeleft = updateInterval;
	        accum = 0.0f;
	        frames = 0;
	    }



		//---------------------------
		//####  MODE SWITCHING  ####
		//---------------------------
		//Get Keys
		inputKeyMode = Input.GetKeyUp("1");
		
		if (oceanObject != null){
			if (!oceanObject.enableTess) displayMode = "DX9";
			if (oceanObject.enableTess) displayMode = "DX11";

			//Handle Mode Switch
			if (inputKeyMode){
				oceanObject.enableTess = !oceanObject.enableTess;
			}

		}
		
		
		//----------------------------------
		//####  OCEAN PRESET SWITCHING  ####
		//----------------------------------
		//Get Keys
		inputKeyPreset = Input.GetKeyUp("2");

		if (usePreset == 0) displayPreset = "Blue Ocean with Waves";
		if (usePreset == 1) displayPreset = "Calm Carribean Blue";
		if (usePreset == 2) displayPreset = "Calm Clear";
		if (usePreset == 3) displayPreset = "Deep Dark Ocean";
		if (usePreset == 4) displayPreset = "Hot Spring Murky";
		if (usePreset == 5) displayPreset = "Mirror Reflection";
		if (usePreset == 6) displayPreset = "Mud Thick Brown";
		if (usePreset == 7) displayPreset = "Swimming Pool";

		//Handle Mode Switch
		if (inputKeyPreset){
			currentPreset += 1;
			if (currentPreset > 7) currentPreset = 0;
		}
		if (usePreset != currentPreset){
			usePreset=currentPreset;
			if (usePreset == 0) oceanObject.SuimonoSetPreset("Built-In Presets","Blue Ocean with Waves");
			if (usePreset == 1) oceanObject.SuimonoSetPreset("Built-In Presets","Calm Carribean Blue");
			if (usePreset == 2) oceanObject.SuimonoSetPreset("Built-In Presets","Calm Clear");
			if (usePreset == 3) oceanObject.SuimonoSetPreset("Built-In Presets","Deep Dark Ocean");
			if (usePreset == 4) oceanObject.SuimonoSetPreset("Built-In Presets","Hot Spring Murky");
			if (usePreset == 5) oceanObject.SuimonoSetPreset("Built-In Presets","Mirror Reflection");
			if (usePreset == 6) oceanObject.SuimonoSetPreset("Built-In Presets","Mud Thick Brown");
			if (usePreset == 7) oceanObject.SuimonoSetPreset("Built-In Presets","Swimming Pool");
		}
		
		
	}



	void OnGUI(){

		GUI.color = new Color(0.0f,0.0f,0.0f,1.0f);
		GUI.Label (new Rect(15f, 10f, 600f, 20f), "SUIMONO 2.0 - Interactive Water System for Unity");
		GUI.Label (new Rect(323f, 10f, 200f, 20f), VerMsg);
		
		GUI.color = new Color(1.0f,0.45f,0.0f,1.0f);
		GUI.Label (new Rect(15f, 26f, 100f, 20f), GuiMsg);

		GUI.color = new Color(1.0f,0.45f,0.0f,1.0f);
		GUI.Label (new Rect(90f, 26f, 300f, 20f), "Preset: "+displayPreset);

		GUI.color = new Color(1.0f,0.45f,0.0f,1.0f);
		GUI.Label (new Rect(290f, 26f, 300f, 20f), "Mode: "+displayMode);
		
		if (showLabel != null){
			GUI.color = labelColor;
			GUI.Label(new Rect(15f,53f, showLabel.width,showLabel.height), showLabel);
		}
		 			 		
	}




}