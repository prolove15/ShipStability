using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoShip_ProblemSolving : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, Finished,
        ShipScenarioStarted, ShipScenarioFinished
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    Animator shipAnim_Cp;

    [SerializeField]
    GameObject steel_GO, wood_GO;

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

        gameState = GameState_En.Inited;
    }

    //------------------------------
    void InitComponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();
    }

    #endregion

    //------------------------------
    public void PlayShipScenario()
    {
        gameState = GameState_En.ShipScenarioStarted;

        shipAnim_Cp.SetInteger("flag", 1);
    }

    void ShipScenarioFinished()
    {
        gameState = GameState_En.ShipScenarioFinished;
    }

    //------------------------------
    public void OnAnimationEvent(string eventName)
    {
        if(eventName == "ShipShow")
        {
            gameState = GameState_En.ShipScenarioFinished;
        }
        else if (eventName == "BaggageShow")
        {
            steel_GO.AddComponent<Outline>();
            wood_GO.AddComponent<Outline>();
        }
    }

}
