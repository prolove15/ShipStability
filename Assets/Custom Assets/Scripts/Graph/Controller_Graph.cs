using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controller_Graph : MonoBehaviour
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
    Background bgd_Cp;

    [SerializeField]
    Animator graphAnim_Cp, formulaAnim_Cp;

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
        StartCoroutine(CorouInit());
    }

    IEnumerator CorouInit()
    {
        InitComponents();

        bgd_Cp.CurtainUp();
        yield return new WaitUntil(() => bgd_Cp.gameState == Background.GameState_En.CurtainUpFinished);

        PlayScenario();
    }

    //------------------------------
    void InitComponents()
    {
        bgd_Cp.Init();

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
    public void PlayScenario()
    {
        StartCoroutine(CorouPlayScenario());
    }

    IEnumerator CorouPlayScenario()
    {
        graphAnim_Cp.SetInteger("flag", 1);

        formulaAnim_Cp.SetInteger("flag", 1);

        yield return new WaitForSeconds(10f);

        LoadNextScene();
    }

    //------------------------------

    //------------------------------
    #region Finish
    
    //------------------------------
    public void LoadNextScene()
    {
        StartCoroutine(CorouLoadNextScene());
    }

    IEnumerator CorouLoadNextScene()
    {
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
