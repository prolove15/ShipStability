using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFinished : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnAnimationEvent(string eventName)
    {
        GameObject controller_GO = GameObject.FindWithTag("GameController");

        if(controller_GO)
        {
            controller_GO.SendMessage("OnAnimationEvent", eventName);
        }
    }
}
