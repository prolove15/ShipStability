using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, Finished
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    GameObject waterBox_GO;

    [SerializeField]
    CargoShip sndShip_Cp;

    [SerializeField]
    Animator sndShipImageAnim_Cp;

    [SerializeField]
    RectTransform sndShipImage_RT;

    [SerializeField]
    Transform sndMassCenter_Tf;

    [SerializeField]
    GameObject baggageTypeText_GO;

    [SerializeField]
    RuntimeAnimatorController sndCamAnim_Cp;

    [SerializeField]
    Transform cameraParent_Tf;

    [SerializeField]
    Transform sndCamPos_Tf;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    public int curSceneIndex;

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

    //////////////////////////////////////////////////////////////////////
    // Initialize
    //////////////////////////////////////////////////////////////////////
    #region Initialize

    //------------------------------
    public void Init()
    {
        InitComponents();

        InitVariables();
    }

    //------------------------------
    void InitComponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();
    }

    //------------------------------
    void InitVariables()
    {
        if(curSceneIndex == 0)
        {
            sndShip_Cp.gameObject.SetActive(false);
            sndShipImage_RT.gameObject.SetActive(false);
        }
    }

    #endregion

    //------------------------------
    public void HandleScene()
    {
        if(curSceneIndex == 0)
        {
            controller_Cp.cargoShip_Cp.gameObject.SetActive(false);
            controller_Cp.uiManager_Cp.shipImage_RT.gameObject.SetActive(false);

            sndShip_Cp.gameObject.SetActive(true);
            sndShipImage_RT.gameObject.SetActive(true);

            controller_Cp.cargoShip_Cp = sndShip_Cp;
            controller_Cp.waterBox_GO = waterBox_GO;
            controller_Cp.uiManager_Cp.shipImageAnim_Cp = sndShipImageAnim_Cp;
            controller_Cp.uiManager_Cp.shipImage_RT = sndShipImage_RT;
            controller_Cp.cameraHandler_Cp.target_Tf = sndMassCenter_Tf;
            baggageTypeText_GO.SetActive(false);
            controller_Cp.cameraHandler_Cp.cameraAnim_Cp.runtimeAnimatorController = sndCamAnim_Cp;
            cameraParent_Tf.SetPositionAndRotation(sndCamPos_Tf.position, sndCamPos_Tf.rotation);

            curSceneIndex++;

            controller_Cp.Init();
        }
        else if(curSceneIndex == 1)
        {
            controller_Cp.LoadNextScene(1);
        }
    }

}
