using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_CSSimulate : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, Finished,
        CameraRoundFinished, TargetFollowing
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    Transform cam_Tf;

    [SerializeField]
    Transform target_Tf;

    [SerializeField]
    Animator camAnim_Cp;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    //-------------------------------------------------- private fields
    Controller_CSSimulate controller_Cp;

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Properties
    //////////////////////////////////////////////////////////////////////
    #region Properties

    //-------------------------------------------------- public properties

    //-------------------------------------------------- private properties

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

    //------------------------------ LateUpdate is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale *= 2f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale *= 0.5f;
        }
    }

    //////////////////////////////////////////////////////////////////////
    // Initialize
    //////////////////////////////////////////////////////////////////////
    #region Initialize

    //------------------------------
    public void Init()
    {
        InitComponents();
    }

    //------------------------------
    void InitComponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller_CSSimulate>();
    }

    #endregion

    //------------------------------
    public void StartCameraRoundScenario()
    {
        camAnim_Cp.SetInteger("flag", 1);
    }

    //------------------------------
    public void OnStartCameraRoundScenarioFinished()
    {
        gameState = GameState_En.CameraRoundFinished;
    }

    //------------------------------
    public void StartShipFollow()
    {
        camAnim_Cp.SetInteger("flag", 2);
        gameState = GameState_En.TargetFollowing;
    }

}
