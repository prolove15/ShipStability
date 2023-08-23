
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    
    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, Orbited, Finished
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    Transform camera_Tf;

    [SerializeField]
    public Animator cameraAnim_Cp;

    [SerializeField]
    Vector3 cameraLastPos;

    //-------------------------------------------------- public fields
    // [HideInInspector]
    public GameState_En gameState;

    public Transform target_Tf;

    //-------------------------------------------------- private fields

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Properties
    //////////////////////////////////////////////////////////////////////
    #region Properties

    //-------------------------------------------------- public properties

    //-------------------------------------------------- private properties
    Controller controller_Cp;

    #endregion

    //////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////
    // Methods
    //////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////

    //------------------------------ Start is called before the first frame update
    void Start()
    {
        
    }

    //------------------------------ Update is called once per frame
    void Update()
    {

    }

    //------------------------------
    void LateUpdate()
    {
        LookAtTarget();
    }
    
    //////////////////////////////////////////////////////////////////////
    // Initialize
    //////////////////////////////////////////////////////////////////////
    #region Initialize

    //------------------------------
    public void Init()
    {
        InitVariables();
    }

    //------------------------------
    void InitVariables()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();
    }

    #endregion

    //------------------------------
    public void StartCameraOrbit()
    {
        gameState = GameState_En.Playing;

        cameraAnim_Cp.SetInteger("flag", 1);
    }

    //------------------------------
    public void CameraOrbitFinished()
    {
        camera_Tf.localPosition = cameraLastPos;

        gameState = GameState_En.Orbited;
    }

    //------------------------------
    void LookAtTarget()
    {
        camera_Tf.LookAt(target_Tf);
    }

}
