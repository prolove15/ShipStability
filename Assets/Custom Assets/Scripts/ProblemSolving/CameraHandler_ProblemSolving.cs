using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraHandler_ProblemSolving : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, Finished,
        ShootStarted, ShootFinished
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    Animator cameraAnim_Cp;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

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
    }

    //------------------------------
    void InitComponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();

        ReplaceDescription();
    }

    //------------------------------
    void ReplaceDescription()
    {
        foreach (Text text_Cp_tp in FindObjectsOfType<Text>())
        {
            if (FileManager.words.ContainsKey(text_Cp_tp.text))
            {
                text_Cp_tp.text = FileManager.words[text_Cp_tp.text];
            }

            int fontSize = text_Cp_tp.fontSize;
            text_Cp_tp.font = FileManager.font;
            text_Cp_tp.fontSize = fontSize;
        }
    }

    #endregion

    //------------------------------
    public void ShootCamera()
    {
        gameState = GameState_En.ShootStarted;

        cameraAnim_Cp.SetInteger("flag", 1);
    }

    void ShootCameraFinished()
    {
        gameState = GameState_En.ShootFinished;
    }

}
