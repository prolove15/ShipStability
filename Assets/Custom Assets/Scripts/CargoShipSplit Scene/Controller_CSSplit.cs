using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controller_CSSplit : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, PreparedFinish, Finished
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    Background bgd_Cp;
    
    [SerializeField]
    CargoShip_CSSplit ship_Cp;

    [SerializeField]
    Animator shipAnim_Cp, titleAnim_Cp, infoAnim_Cp, evaluateAnim_Cp;

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

    #endregion

    //////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////
    // Methods
    //////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////

    //------------------------------ Start is called before the first frame update
    void Start()
    {
        Init();
    }

    //------------------------------ Update is called once per frame
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
        StartCoroutine(CorouInit());
    }

    IEnumerator CorouInit()
    {
        InitComponents();

        InitVariables();

        gameState = GameState_En.Inited;

        bgd_Cp.CurtainUp();
        yield return new WaitUntil(() => bgd_Cp.gameState == Background.GameState_En.CurtainUpFinished);

        PlayCargoShipSplitScenario();
    }

    //------------------------------
    void InitComponents()
    {
        
    }

    //------------------------------
    void InitVariables()
    {
        bgd_Cp.Init();

        ship_Cp.Init();

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
    public void PlayCargoShipSplitScenario()
    {
        titleAnim_Cp.SetInteger("flag", 1);
    }

    //------------------------------
    void ShowShipAnimation()
    {
        shipAnim_Cp.SetInteger("flag", 1);
    }

    //------------------------------
    void OnSteelShow()
    {
        ship_Cp.UpdateSteelOutline(true);
        infoAnim_Cp.SetInteger("flag", 1);
    }

    //------------------------------
    void OnSteelShowed()
    {
        ship_Cp.UpdateSteelOutline(false);
        infoAnim_Cp.SetInteger("flag", -1);
    }

    //------------------------------
    void OnWoodShow()
    {
        ship_Cp.UpdateWoodOutline(true);
        infoAnim_Cp.SetInteger("flag", 2);
    }

    //------------------------------
    void OnWoodShowed()
    {
        ship_Cp.UpdateWoodOutline(false);
        infoAnim_Cp.SetInteger("flag", -2);
    }

    //------------------------------
    void ShipShowed()
    {
        LoadNextScene();
    }

    //------------------------------
    void OnBaggageAllShowed()
    {
        evaluateAnim_Cp.SetInteger("flag", 1);
    }

    //------------------------------
    void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "TitleShowed":
                ShowShipAnimation();
                break;
            case "SteelShow":
                OnSteelShow();
                break;
            case "SteelShowed":
                OnSteelShowed();
                break;
            case "WoodShow":
                OnWoodShow();
                break;
            case "WoodShowed":
                OnWoodShowed();
                break;
            case "ShipShowed":
                ShipShowed();
                break;
            case "InfoShowed":
                break;
            case "BaggageAllShowed":
                OnBaggageAllShowed();
                break;
        }
    }

    //////////////////////////////////////////////////////////////////////
    // Initialize
    //////////////////////////////////////////////////////////////////////
    #region Finish

    //------------------------------
    public void PrepareFinish()
    {
        StartCoroutine(CorouPrepareFinish());
    }

    IEnumerator CorouPrepareFinish()
    {

        yield return null;

        gameState = GameState_En.PreparedFinish;
    }

    //------------------------------
    public void LoadNextScene()
    {
        StartCoroutine(CorouLoadNextScene());
    }

    IEnumerator CorouLoadNextScene()
    {
        PrepareFinish();
        yield return new WaitUntil(() => gameState == GameState_En.PreparedFinish);

        bgd_Cp.CurtainDown();
        yield return new WaitUntil(() => bgd_Cp.gameState == Background.GameState_En.CurtainDownFinished);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            ApplicationQuit();
        }
        else
        {
            LoadScene(nextSceneIndex);
        }
    }

    //------------------------------
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    //------------------------------
    public void ApplicationQuit()
    {
        Application.Quit();
        return;
    }

    #endregion
}
