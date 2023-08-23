using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sea : MonoBehaviour
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
    OceanAdvanced ocean;

    [SerializeField]
    public float curOceanAmplitude, firstOceanAmplitude, lastOceanAmplitude;

    [SerializeField]
    float oceanAmplitudeChangeDuration;

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

        InitWaveAmplitude();

        gameState = GameState_En.Inited;
    }

    //------------------------------
    void InitComponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();
    }

    //------------------------------
    void InitWaveAmplitude()
    {
        curOceanAmplitude = firstOceanAmplitude;

        SetWaveAmplitude(firstOceanAmplitude);
    }

    #endregion

    //------------------------------
    public void SetWaveAmplitude(float value)
    {
        //ocean.SetWaveAmplitude(value);
    }

    //------------------------------
    public void StartWaveScenario()
    {
        DOTween.To(() => curOceanAmplitude, x => curOceanAmplitude = x, lastOceanAmplitude,
            oceanAmplitudeChangeDuration)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdateWaveAmplitudeValue)
            .OnComplete(CompleteTween);
    }

    void UpdateWaveAmplitudeValue()
    {
        SetWaveAmplitude(curOceanAmplitude);
    }

    void CompleteTween()
    {
        
    }

    //------------------------------
    public void PrepareFinish()
    {
        SetWaveAmplitude(1f);

        gameState = GameState_En.PreparedFinish;
    }

}
