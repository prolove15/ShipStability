using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controller_CSSimulate : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, PreparedFinish, Finished,
        ShipMoving
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
    public UI_CSSimulate uiManager_Cp;

    [SerializeField]
    public Sea_Splash sea_Cp;

    [SerializeField]
    public Camera_CSSimulate camera_Cp;

    [SerializeField]
    public Transform ship_Tf;

    [SerializeField]
    Animator shipAnim_Cp;

    [SerializeField]
    Animator titleAnim_Cp, shipImageAnim_Cp, envAnim_Cp, evaluateAnim_Cp;

    [SerializeField]
    float curWindStrength, minWindStrength, maxWindStrength;

    [SerializeField]
    float curWaveHeight, minWaveHeight, maxWaveHeight;

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
        Init();
    }

    //------------------------------ LateUpdate is called once per frame
    void LateUpdate()
    {
        if(gameState == GameState_En.ShipMoving)
        {
            HandleShipImage();

            PlayWaveScenario();
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
        //
        InitComponents();

        InitVariables();

        gameState = GameState_En.Inited;

        //
        bgd_Cp.CurtainUp();
        yield return new WaitUntil(() => bgd_Cp.gameState == Background.GameState_En.CurtainUpFinished);

        PlayCargoShipSimulateScenario();
    }

    //------------------------------
    void InitComponents()
    {

    }

    //------------------------------
    void InitVariables()
    {
        bgd_Cp.Init();

        uiManager_Cp.Init();

        sea_Cp.Init();

        camera_Cp.Init();

        curWindStrength = minWindStrength;
        OnUpdateWindStrength();

        curWaveHeight = minWaveHeight;
        OnUpdateWaveHeight();

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
    public void PlayCargoShipSimulateScenario()
    {
        StartCoroutine(CorouPlayCargoShipSimulateScenario());
    }

    IEnumerator CorouPlayCargoShipSimulateScenario()
    {
        titleAnim_Cp.SetInteger("flag", 1);
        envAnim_Cp.SetInteger("flag", 1);

        camera_Cp.StartCameraRoundScenario();
        yield return new WaitUntil(() => camera_Cp.gameState == Camera_CSSimulate.GameState_En.CameraRoundFinished);

        shipImageAnim_Cp.SetInteger("flag", 1);

        PlayShipMoving();

        uiManager_Cp.gameState = UI_CSSimulate.GameState_En.Playing;

        gameState = GameState_En.ShipMoving;

        camera_Cp.StartShipFollow();

        StartWindStrengthScenario(maxWindStrength);

        StartWaveHeightScenario(maxWaveHeight);
    }

    //------------------------------
    void PlayShipMoving()
    {
        shipAnim_Cp.SetInteger("flag", 1);
    }

    //------------------------------
    void HandleShipImage()
    {
        float shipRotationZValue = ship_Tf.rotation.eulerAngles.z;

        uiManager_Cp.HandleShipImage(shipRotationZValue);
    }

    //------------------------------
    public void StartWindStrengthScenario(float maxWindStr_pr)
    {
        DOTween.To(() => curWindStrength, x => curWindStrength = x, maxWindStr_pr, 20f)
            .OnUpdate(OnUpdateWindStrength)
            .OnComplete(OnCompleteWindStrengthChange);
    }

    void OnUpdateWindStrength()
    {
        string windText = curWindStrength.ToString();
        int windStrLength = windText.Length;
        windText = windText.Substring(0, windStrLength > 4 ? 4 : windStrLength) + " m/s";

        uiManager_Cp.SetWindStrength(windText);
    }

    void OnCompleteWindStrengthChange()
    {
        float randMaxWindStr = maxWindStrength + Random.Range(-1f, 1f);

        StartWindStrengthScenario(randMaxWindStr);
    }

    //------------------------------
    void StartWaveHeightScenario(float maxWaveHeight_pr)
    {
        DOTween.To(() => curWaveHeight, x => curWaveHeight = x, maxWaveHeight_pr, 20f)
            .OnUpdate(OnUpdateWaveHeight)
            .OnComplete(OnCompleteWaveHeightChange);
    }

    void OnUpdateWaveHeight()
    {
        string waveHeightText = curWaveHeight.ToString();
        int waveHeightTextLength = waveHeightText.Length;
        waveHeightText = waveHeightText.Substring(0, waveHeightTextLength > 4 ? 4 : waveHeightTextLength) + " m";

        uiManager_Cp.SetWaveHeightText(waveHeightText);
    }

    void OnCompleteWaveHeightChange()
    {
        float maxWaveHeight_tp = maxWaveHeight + Random.Range(-0.5f, 0.5f);

        StartWaveHeightScenario(maxWaveHeight_tp);
    }

    //------------------------------
    void PlayWaveScenario()
    {
        float realWaveHeight = curWaveHeight * (uiManager_Cp.maxWaveHeight / maxWaveHeight);
        uiManager_Cp.SetWaveHeight(realWaveHeight);
    }

    //------------------------------
    void OnShipMoveFinished()
    {
        evaluateAnim_Cp.SetInteger("flag", 1);
    }

    //------------------------------
    void OnEvaluateFinished()
    {
        LoadNextScene();
    }

    //------------------------------
    public void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "CameraAnimationFinished":
                camera_Cp.OnStartCameraRoundScenarioFinished();
                break;
            case "ShipMoveFinished":
                OnShipMoveFinished();
                break;
            case "EvaluateFinished":
                OnEvaluateFinished();
                break;
        }
    }

    //------------------------------
    #region Finish
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
