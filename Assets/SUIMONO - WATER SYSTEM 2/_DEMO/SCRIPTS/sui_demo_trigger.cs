using UnityEngine;
using System.Collections;



public class sui_demo_trigger : MonoBehaviour {




	public enum Sui_Demo_TriggerType{
			switchtovehicle,
			watersurface
			}

	public bool requireLineOfSight = true;
	public Sui_Demo_TriggerType triggerType =  Sui_Demo_TriggerType.switchtovehicle;
	public Texture2D showDot;
	public Texture2D showIcon;
	public Texture2D backgroundImage;
	public string label = "";
	public Color labelColor = new Color(0f,0f,0f,1f);
	public Vector2 dotOffset = new Vector2(0.5f,0.5f);
	public Vector2 labelOffset = new Vector2(0.5f,0.5f);
	public string actionKey = "f";
	public bool requireReset = true;
	public Transform trackObject;
	public float fadeSpeed = 0.0f;
	public float checkDistance = 200.0f;


	private sui_demo_ControllerMaster CM;
	private bool isInRange = false;
	private bool onAction = false;
	private string useLabel = "";
	private GUISkin style;
	private float fadeTimer = 0.0f;
	private bool isInSight = false;
	private bool enableAction = false;
	private Vector3 savedPos = new Vector3(0f,0f,0f);




	void Start () {

		CM = GameObject.Find("_CONTROLLER").GetComponent<sui_demo_ControllerMaster>();
	}





	void FixedUpdate () {
		
		//set default label
		useLabel = label;

		//CHECK LINE OF SIGHT
		if (Camera.main != null){
			if (savedPos != Camera.main.transform.position){
				savedPos = Camera.main.transform.position;
				isInSight = CheckLineOfSight();
			}
		}
		
		//CHECK RANGE
		isInRange = false;
		if (Vector3.Distance(this.transform.position,trackObject.transform.position) <= (checkDistance*0.75f)) isInRange = true;
		
		//ENABLE ACTION
		enableAction = false;
		if (isInRange && !requireLineOfSight){
			enableAction = true;
		} else if (isInRange && requireLineOfSight && isInSight){
			enableAction = true;
		}
			
		//CHECK FOR ACTION KEY
		onAction = false;
		if (Input.GetKeyUp(actionKey) && enableAction){
			onAction = true;
		}
		
		
		//PERFORM TRIGGER ACTIONS
		if (onAction){
			useLabel = "";
			//onAction= false;
			//enableAction = false;
			//if (requireReset) resetTrigger = true;

			//switch controller type
			if (triggerType == Sui_Demo_TriggerType.switchtovehicle){
			if (CM != null){
				if (CM.currentControllerType == sui_demo_ControllerMaster.Sui_Demo_ControllerType.character){
					CM.currentControllerType = sui_demo_ControllerMaster.Sui_Demo_ControllerType.boat;
				} else if (CM.currentControllerType == sui_demo_ControllerMaster.Sui_Demo_ControllerType.boat){
					CM.currentControllerType = sui_demo_ControllerMaster.Sui_Demo_ControllerType.character;
				}
			}
			}
			
			
			
		}
		

		if (enableAction == true){
			fadeTimer = Mathf.Lerp(fadeTimer,0.8f,Time.deltaTime * fadeSpeed * 1.0f);
		} else {
			fadeTimer = Mathf.Lerp(fadeTimer,0.0f,Time.deltaTime * fadeSpeed * 1.0f);
		}
			
		if (isInRange == true){
			if ( GetComponent<Renderer>()) GetComponent<Renderer>().material.SetColor("_TintColor",new Color(0f,1f,0f,0.1f));
		} else {
			if ( GetComponent<Renderer>()) GetComponent<Renderer>().material.SetColor("_TintColor",new Color(0.5f,0f,0f,0.1f));
		}

	}






	public bool CheckLineOfSight(){
		bool retBool = false;
		
		if (requireLineOfSight && Camera.main != null){
		
			//get character distance
			float charDistance = 0.0f;
			RaycastHit[] chits;
			Ray cray = new Ray();;

			cray.origin = Camera.main.transform.position;
			cray.direction = Camera.main.transform.forward;

			chits = Physics.RaycastAll(cray,1000.0f);
			for (int c = 0;c < chits.Length; c++) {
				RaycastHit chit  = chits[c];
				Collider ccoll = chit.collider;
				if (ccoll) {
					if ( ccoll == trackObject.GetComponent<Collider>() ) charDistance = chit.distance;
				}
			}
			
			//get trigger distance
			RaycastHit[] hits;
			hits = Physics.RaycastAll(cray,checkDistance+charDistance);
			
			for (int i = 0;i < hits.Length; i++) {
				RaycastHit hit = hits[i];
				Collider coll = hit.collider;
				if (coll) {
					if ( coll == this.GetComponent<Collider>() ) retBool = true;
				}
			}
		}

		return retBool;
	}





	void OnGUI(){


		if (fadeTimer > 0.0f){

		 	if (useLabel != ""){
	
		 		int texLength = (useLabel.Length * 6)+5;
		 		GUI.color = new Color(0f,0f,0f,fadeTimer);
		 		GUI.Label(new Rect((Screen.width*labelOffset.x)-(texLength*0.5f)+8f, Screen.height*labelOffset.y+21f, texLength, 20f), useLabel);
		 		GUI.color = new Color(labelColor.r,labelColor.g,labelColor.b,fadeTimer);
		 		GUI.Label(new Rect((Screen.width*labelOffset.x)-(texLength*0.5f)+7f, Screen.height*labelOffset.y+20f, texLength, 20f), useLabel);
		 		
		 		if (showIcon != null){
		 			GUI.color = new Color(labelColor.r,labelColor.g,labelColor.b,fadeTimer);
		 			GUI.Label(new Rect((Screen.width*labelOffset.x)-(texLength*0.8f)+7f, Screen.height*labelOffset.y+16f, showIcon.width,showIcon.height), showIcon);
		 			
		 			GUI.color = new Color(0f,0f,0f,fadeTimer);
		 			GUI.Label(new Rect((Screen.width*labelOffset.x)-(texLength*0.8f)+16f, Screen.height*labelOffset.y+20f, 20f, 30f), actionKey.ToUpper());
		 		}
		 	}
		}
	}


}