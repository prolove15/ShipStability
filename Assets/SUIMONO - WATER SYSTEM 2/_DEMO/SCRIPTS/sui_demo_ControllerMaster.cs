using UnityEngine;
using System.Collections;



public class sui_demo_ControllerMaster : MonoBehaviour {


	public Transform cameraObject;
	public enum Sui_Demo_ControllerType{
			none,character, boat, orbit
			}
	public Sui_Demo_ControllerType currentControllerType =  Sui_Demo_ControllerType.character;


	private sui_demo_ControllerCharacter characterController;
	private sui_demo_ControllerBoat boatController;
	private sui_demo_ControllerOrbit orbitController;
	private bool resetController = false;
	private Sui_Demo_ControllerType useController = Sui_Demo_ControllerType.character;




	void Start () {
		characterController = this.gameObject.GetComponent<sui_demo_ControllerCharacter>() as sui_demo_ControllerCharacter;
		boatController = this.gameObject.GetComponent<sui_demo_ControllerBoat>() as sui_demo_ControllerBoat;
		orbitController = this.gameObject.GetComponent<sui_demo_ControllerOrbit>() as sui_demo_ControllerOrbit;
	}




	void LateUpdate () {

		//check for reset
		if (currentControllerType != useController){
			resetController = true;
		} else {
			resetController = false;
		}
		
		
		//set controller to none
		if (currentControllerType == Sui_Demo_ControllerType.none){
			if (characterController != null) characterController.isActive = false;
			if (boatController != null) boatController.isActive = false;
			if (orbitController != null) orbitController.isActive = false;
		}
		
		//set controller to character
		if (currentControllerType == Sui_Demo_ControllerType.character){
			if (boatController != null) boatController.isActive = false;
			if (orbitController != null) orbitController.isActive = false;
			if (characterController != null) characterController.isActive = true;
		}

		//set controller to boat
		if (currentControllerType == Sui_Demo_ControllerType.boat){
			if (characterController != null) characterController.isActive = false;
			if (orbitController != null) orbitController.isActive = false;
			if (boatController != null) boatController.isActive = true;
		}

		//set controller to orbit
		if (currentControllerType == Sui_Demo_ControllerType.orbit){
			if (characterController != null) characterController.isActive = false;
			if (boatController != null) boatController.isActive = false;
			if (orbitController != null) orbitController.isActive = true;
		}




		//Place Character in Boat Object
		if (characterController != null){
			if (currentControllerType == Sui_Demo_ControllerType.boat){
				characterController.isInBoat = true;
				characterController.cameraTarget.transform.position = boatController.targetAnimator.playerPosition.transform.position;
				characterController.cameraTarget.transform.rotation = boatController.targetAnimator.playerPosition.transform.rotation;
				characterController.cameraTarget.gameObject.GetComponent<Collider>().enabled = false;
				characterController.cameraTarget.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			}
			if (currentControllerType == Sui_Demo_ControllerType.character && resetController){
				characterController.isInBoat = false;
				characterController.cameraTarget.transform.position = boatController.targetAnimator.playerExit.transform.position;
				characterController.cameraTarget.gameObject.GetComponent<Collider>().enabled = true;
				characterController.cameraTarget.gameObject.GetComponent<Rigidbody>().useGravity = true;
				characterController.cameraTarget.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			}
		}
		
		//reset
		if (resetController){
			resetController = false;
			useController = currentControllerType;
		}
	}

}