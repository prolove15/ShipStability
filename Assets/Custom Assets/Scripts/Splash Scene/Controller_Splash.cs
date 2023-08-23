using Suimono.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class Controller_Splash : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, PreparedFinish, Finished,
        TitleAnimFinished, SubTitleAnimFinished, CargoShipAnimFinished, OilTankerAnimFinished
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
    public Sea_Splash sea_Cp;

    [SerializeField]
    Animator titleAnim_Cp;

    [SerializeField]
    Animator[] subTitleAnim_Cps;

    [SerializeField]
    Animator cargoShipAnim_Cp, oilTankerAnim_Cp;

    [SerializeField]
    FileManager fileManager_Cp;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    //-------------------------------------------------- private fields
    public int operateStack;

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
        // Init();
    }

    //------------------------------ Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Init();
        }
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
        //
        InitComponents();

        InitVariables();

        gameState = GameState_En.Inited;

        //
        bgd_Cp.CurtainUp();
        yield return new WaitUntil(() => bgd_Cp.gameState == Background.GameState_En.CurtainUpFinished);

        PlaySplashScenario();
    }

    //------------------------------
    void InitComponents()
    {
        
    }

    //------------------------------
    void InitVariables()
    {
        //
        bgd_Cp.Init();

        sea_Cp.Init();

        //
        string filePath = Path.Combine(Application.streamingAssetsPath, "Description.txt");
        fileManager_Cp.LoadFile(filePath);

        fileManager_Cp.LoadFont();

        ReplaceDescription();
    }

    //------------------------------
    void ReplaceDescription()
    {
        foreach(Text text_Cp_tp in FindObjectsOfType<Text>())
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
    public void PlaySplashScenario()
    {
        StartCoroutine(CorouPlaySplashScenario());
    }

    IEnumerator CorouPlaySplashScenario()
    {
        //
        titleAnim_Cp.SetInteger("flag", 1);
        cargoShipAnim_Cp.SetInteger("flag", 1);
        oilTankerAnim_Cp.SetInteger("flag", 1);

        yield return new WaitUntil(() => operateStack == 3);
        operateStack = 0;

        //
        for (int i = 0; i < subTitleAnim_Cps.Length; i++)
        {
            subTitleAnim_Cps[i].SetInteger("flag", 1);
        }

        yield return new WaitUntil(() => operateStack == subTitleAnim_Cps.Length);
        operateStack = 0;

        //
        yield return new WaitForSeconds(3f);

        //
        for (int i = 0; i < subTitleAnim_Cps.Length; i++)
        {
            subTitleAnim_Cps[i].SetInteger("flag", 2);
        }

        yield return new WaitUntil(() => operateStack == subTitleAnim_Cps.Length);
        operateStack = 0;

        //
        cargoShipAnim_Cp.SetInteger("flag", 2);
        oilTankerAnim_Cp.SetInteger("flag", 2);
        titleAnim_Cp.SetInteger("flag", 2);

        yield return new WaitUntil(() => operateStack == 3);
        operateStack = 0;

        //
        LoadNextScene();
    }

    //------------------------------
    public void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "TitleShowed":
                gameState = GameState_En.TitleAnimFinished;
                operateStack++;
                break;
            case "CargoShipShowed":
                gameState = GameState_En.CargoShipAnimFinished;
                operateStack++;
                break;
            case "OilTankerShowed":
                gameState = GameState_En.OilTankerAnimFinished;
                operateStack++;
                break;
            case "SubTitleShowed":
                gameState = GameState_En.SubTitleAnimFinished;
                operateStack++;
                break;
            case "TitleHided":
                gameState = GameState_En.TitleAnimFinished;
                operateStack++;
                break;
            case "CargoShipHided":
                gameState = GameState_En.CargoShipAnimFinished;
                operateStack++;
                break;
            case "OilTankerHided":
                gameState = GameState_En.OilTankerAnimFinished;
                operateStack++;
                break;
            case "SubTitleHided":
                gameState = GameState_En.SubTitleAnimFinished;
                operateStack++;
                break;
        }
    }

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
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
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

}
