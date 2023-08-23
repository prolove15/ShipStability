using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour
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
    SceneHandler sceneHandler_Cp;

    [SerializeField]
    Background bgdManager_Cp;

    [SerializeField]
    public UIManager uiManager_Cp;

    [SerializeField]
    public CargoShip cargoShip_Cp;

    [SerializeField]
    public CameraHandler cameraHandler_Cp;

    [SerializeField]
    public Sea sea_Cp;

    [SerializeField]
    public GameObject waterBox_GO, waterPlane_GO;

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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            sea_Cp.SetWaveAmplitude(2f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            sea_Cp.SetWaveAmplitude(0.5f);
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

        InitVariables();

        gameState = GameState_En.Inited;

        Play();
    }

    //------------------------------
    void InitComponents()
    {
          
    }

    //------------------------------
    void InitVariables()
    {
        bgdManager_Cp.Init();

        sceneHandler_Cp.Init();

        uiManager_Cp.Init();

        cargoShip_Cp.Init();

        sea_Cp.Init();
    }

    #endregion

    //------------------------------
    public void Play()
    {
        StartCoroutine(CorouPlay());
    }

    IEnumerator CorouPlay()
    {
        bgdManager_Cp.CurtainUp();

        yield return new WaitUntil(() => bgdManager_Cp.gameState == Background.GameState_En.CurtainUpFinished);

        PlayCargoShipScenario();
    }

    //------------------------------
    public void PlayCargoShipScenario()
    {
        StartCoroutine(CorouPlayCargoShipScenario());
    }

    IEnumerator CorouPlayCargoShipScenario()
    {
        yield return new WaitForSeconds(3f);

        cameraHandler_Cp.StartCameraOrbit();
        yield return new WaitUntil(() => cameraHandler_Cp.gameState == CameraHandler.GameState_En.Orbited);

        cargoShip_Cp.SplitBaggage1FromShip();
        yield return new WaitUntil(() => cargoShip_Cp.gameState == CargoShip.GameState_En.Baggage1Splited);

        uiManager_Cp.ShowHideShipImage(true);
        yield return new WaitUntil(() => uiManager_Cp.gameState == UIManager.GameState_En.ShipImageShowed);

        if (waterBox_GO)
        {
            waterBox_GO.SetActive(false);
        }
        if (waterPlane_GO)
        {
            waterPlane_GO.SetActive(false);
        }

        sea_Cp.StartWaveScenario();

        cargoShip_Cp.ShipSimulate();
    }

    //------------------------------
    public void PrepareFinish()
    {
        StartCoroutine(CorouPrepareFinish());
    }

    IEnumerator CorouPrepareFinish()
    {
        bgdManager_Cp.CurtainDown();
        yield return new WaitUntil(() => bgdManager_Cp.gameState == Background.GameState_En.CurtainDownFinished);

        DOTween.KillAll();
        yield return new WaitForSeconds(Time.deltaTime);

        cargoShip_Cp.PrepareFinish();
        yield return new WaitUntil(() => cargoShip_Cp.gameState == CargoShip.GameState_En.PreparedFinish);

        sea_Cp.PrepareFinish();
        yield return new WaitUntil(() => sea_Cp.gameState == Sea.GameState_En.PreparedFinish);
        yield return new WaitForSeconds(5 * Time.deltaTime);

        gameState = GameState_En.PreparedFinish;
    }

    //------------------------------
    public void LoadNextScene(int sceneIndex = 0)
    {
        StartCoroutine(CorouLoadNextScene(sceneIndex));
    }

    IEnumerator CorouLoadNextScene(int sceneIndex)
    {
        PrepareFinish();
        yield return new WaitUntil(() => gameState == GameState_En.PreparedFinish);

        if(sceneIndex == 1)
        {
            LoadScene("ProblemSolving");
        }
        else if(sceneIndex == 0)
        {
            sceneHandler_Cp.HandleScene();
        }
    }

    //------------------------------
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
