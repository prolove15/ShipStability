using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controller_ProblemSolving : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Prepared, Playing, PreparedFinish, Finished
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    Background bgdManager_Cp;

    [SerializeField]
    public UIManager_ProblemSolving uiManager_Cp;

    [SerializeField]
    public CameraHandler_ProblemSolving cameraHandler_Cp;

    [SerializeField]
    public CargoShip_ProblemSolving ship_Cp;

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
        bgdManager_Cp.Init();

        uiManager_Cp.Init();

        cameraHandler_Cp.Init();

        ship_Cp.Init();

        gameState = GameState_En.Inited;

        Prepare();

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
    public void Prepare()
    {
        StartCoroutine(CorouPrepare());
    }

    IEnumerator CorouPrepare()
    {
        bgdManager_Cp.CurtainUp();
        yield return new WaitUntil(() => bgdManager_Cp.gameState == Background.GameState_En.CurtainUpFinished);

        gameState = GameState_En.Prepared;

        Play();
    }

    //------------------------------
    public void Play()
    {
        gameState = GameState_En.Playing;

        PlayProblemSolvingScenario();
    }

    //------------------------------
    public void PlayProblemSolvingScenario()
    {
        StartCoroutine(CorouPlayProblemSolvingScenario());
    }

    IEnumerator CorouPlayProblemSolvingScenario()
    {
        cameraHandler_Cp.ShootCamera();
        yield return new WaitUntil(() => cameraHandler_Cp.gameState ==
            CameraHandler_ProblemSolving.GameState_En.ShootFinished);

        uiManager_Cp.ShowProblemPanel();
        yield return new WaitForSeconds(1f);

        ship_Cp.PlayShipScenario();
        yield return new WaitUntil(() => ship_Cp.gameState == CargoShip_ProblemSolving.GameState_En.ShipScenarioFinished);

        yield return new WaitForSeconds(10f);
        uiManager_Cp.ShowSolutionPanel();

        yield return new WaitForSeconds(10f);
        LoadNextScene();
    }

    //------------------------------
    public void PrepareFinish()
    {
        StartCoroutine(CorouPrepareFinish());
    }

    IEnumerator CorouPrepareFinish()
    {
        bgdManager_Cp.CurtainDown();
        yield return new WaitUntil(() => bgdManager_Cp.gameState == Background.GameState_En.CurtainUpFinished);

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

        LoadScene("Scene 1");
    }

    //------------------------------
    public void LoadScene(string sceneName)
    {
        Application.Quit();
        return;
        //SceneManager.LoadScene(sceneName);
    }

}
