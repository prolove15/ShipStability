using Suimono.Core;
using UnityEngine;

/// <summary>
/// Usage of "Event Trigger" tab of fx_EffectObject.
/// Attach this to fx_EffectObject and watch log.
/// </summary>

public class utility_EventTrigger : MonoBehaviour {

    fx_EffectObject target;

    void Start(){

        target = GetComponent<fx_EffectObject>();
        
        // subscribe event
        if (target != null){
            target.OnTrigger += OnTrigger; //our new event trigger!
        } else {
            Debug.Log("#EffectTriggerUsage# Can't find fx_EffectObject on " + transform.name, gameObject);
        }
    }
    
    private void OnTrigger(Vector3 position, Quaternion rotatoin) {
        Debug.LogFormat(gameObject, "#EffectTriggerUsage# Trigger, position={0}, rotation={1}", position, rotatoin.eulerAngles);
    }
}