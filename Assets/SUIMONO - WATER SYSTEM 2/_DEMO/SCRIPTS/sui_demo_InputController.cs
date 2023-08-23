using UnityEngine;
using System.Collections;



public class sui_demo_InputController : MonoBehaviour {




	[HideInInspector] public bool inputMouseKey0 = false;
	[HideInInspector] public bool inputKeySHIFTL = false;
	[HideInInspector] public bool inputKeySPACE = false;
	[HideInInspector] public bool inputKeyW = false;
	[HideInInspector] public bool inputKeyS = false;
	[HideInInspector] public bool inputKeyA = false;
	[HideInInspector] public bool inputKeyD = false;
	[HideInInspector] public bool inputKeyF = false;
	[HideInInspector] public bool inputKeyQ = false;
	[HideInInspector] public bool inputKeyE = false;
	[HideInInspector] public bool inputKeyESC = false;
	[HideInInspector] public bool inputMouseKey1 = false;
	[HideInInspector] public float inputMouseX = 0.0f;
	[HideInInspector] public float inputMouseY = 0.0f;
	[HideInInspector] public float inputMouseWheel = 0.0f;
		


	void Update () {
		
		//---------------------------------
		//  GET KEYBOARD AND MOUSE INPUTS
		//---------------------------------

		//"WASD" MOVEMENT KEYS
		inputKeyW = Input.GetKey("w");
		inputKeyS = Input.GetKey("s");
		inputKeyA = Input.GetKey("a");
		inputKeyD = Input.GetKey("d");
		
		//"QE" KEYS
		inputKeyQ = Input.GetKey("q");
		inputKeyE = Input.GetKey("e");
			
		//LEFT MOUSE BUTTON
		inputMouseKey0 = Input.GetKey("mouse 0");

		//RIGHT MOUSE BUTTON
		inputMouseKey1 = Input.GetKey("mouse 1");

		//GET MOUSE MOVEMENT and SCROLLWHEEL
		inputMouseX = Input.GetAxisRaw("Mouse X");
		inputMouseY = Input.GetAxisRaw("Mouse Y");
		inputMouseWheel = Input.GetAxisRaw("Mouse ScrollWheel");
		
		//EXTRA KEYS
		inputKeySHIFTL = Input.GetKey("left shift");
		inputKeySPACE = Input.GetKey("space");
		inputKeyF = Input.GetKey("f");
		inputKeyESC = Input.GetKey("escape");

	}

}