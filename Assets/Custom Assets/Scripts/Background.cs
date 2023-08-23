using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, Finished,
        CurtainDownFinished, CurtainUpFinished
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    Animator curtainAnim_Cp;

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

        InitVariables();

        gameState = GameState_En.Inited;
    }

    //------------------------------
    void InitComponents()
    {
        
    }

    //------------------------------
    void InitVariables()
    {
        curtainAnim_Cp.gameObject.SetActive(true);

        curtainAnim_Cp.gameObject.GetComponent<Image>().enabled = true;
    }

    #endregion

    //------------------------------
    public void CurtainDown()
    {
        curtainAnim_Cp.SetInteger("flag", 1);
    }

    public void CurtainDownFinished()
    {
        gameState = GameState_En.CurtainDownFinished;
    }

    //------------------------------
    public void CurtainUp()
    {
        curtainAnim_Cp.SetInteger("flag", 2);
    }

    public void CurtainUpFinished()
    {
        gameState = GameState_En.CurtainUpFinished;
    }

}
