using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CargoShip : MonoBehaviour
{

    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, PreparedFinish, Finished,
        Baggage1Spliting, Baggage1Splited, Baggage2Splited, ShipSimulating
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    FloatingGameEntityRealist floatingGameEntity_Cp;

    [SerializeField]
    Animator cargoShipAnim_Cp;

    [SerializeField]
    Transform waterBox_Tf;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    public float centerOfMassX;

    public float windStrength;

    public float waveAmplitude;

    //-------------------------------------------------- private fields
    UIManager uiManager_Cp;

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
        if (waterBox_Tf)
        {
            waterBox_Tf.rotation = Quaternion.identity;
        }
    }

    private void LateUpdate()
    {
        if(gameState == GameState_En.ShipSimulating)
        {
            UpdateShipImageRotation();
        }

        //transform.position = new Vector3(transform.position.x, 1.74f, transform.position.z);
    }

    //////////////////////////////////////////////////////////////////////
    // Initialize
    //////////////////////////////////////////////////////////////////////
    #region Initialize

    //------------------------------
    public void Init()
    {
        InitComponents();

        InitVariabels();

        // 
        gameState = GameState_En.Inited;
    }

    //------------------------------
    void InitComponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();
        uiManager_Cp = controller_Cp.uiManager_Cp;
    }

    void InitVariabels()
    {
        UpdateWaveAmplitude();

        UpdateWindStrength();
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Ship animate
    //////////////////////////////////////////////////////////////////////
    #region Ship animate

    //------------------------------ 
    public void SplitShip()
    {
        cargoShipAnim_Cp.SetInteger("cargoShipState", 1);
    }

    public void ShipSplited()
    {
        
    }

    //------------------------------
    public void MergeShip()
    {
        cargoShipAnim_Cp.SetInteger("cargoShipState", -1);
    }

    public void ShipMerged()
    {
        
    }

    //------------------------------
    public void SplitBaggage1()
    {
        cargoShipAnim_Cp.SetInteger("cargoShipState", 2);
    }

    public void Baggage1Splited()
    {
        gameState = GameState_En.Baggage1Splited;
    }

    //------------------------------
    public void MergeBaggage1()
    {
        cargoShipAnim_Cp.SetInteger("cargoShipState", -2);
    }

    public void Baggage1Merged()
    {
        
    }

    //------------------------------
    public void SplitBaggage2()
    {
        cargoShipAnim_Cp.SetInteger("cargoShipState", 3);
    }

    public void Baggage2Splited()
    {
        gameState = GameState_En.Baggage2Splited;
    }

    //------------------------------
    public void MergeBaggage2()
    {
        cargoShipAnim_Cp.SetInteger("cargoShipState", -3);
    }

    public void Baggage2Merged()
    {
        
    }

    #endregion

    //------------------------------
    public void SplitBaggage1FromShip()
    {
        SplitBaggage1();
    }

    //------------------------------
    public void SplitBaggage2FromShip()
    {
        SplitBaggage2();
    }

    //------------------------------
    public void ShipSimulate()
    {
        gameState = GameState_En.ShipSimulating;

        //
        DOTween.To(() => waveAmplitude, x => waveAmplitude = x, 10f, 10f)
            .OnUpdate(UpdateWaveAmplitude);

        //
        DOTween.To(() => windStrength, x => windStrength = x, 20f, 10f)
            .OnUpdate(UpdateWindStrength);

        //
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(DOTween.To(() => centerOfMassX, x => centerOfMassX = x, 0.5f, 8f));
        mySequence.Append(DOTween.To(() => centerOfMassX, x => centerOfMassX = x, 1f, 8f));
        mySequence.Append(DOTween.To(() => centerOfMassX, x => centerOfMassX = x, 2f, 4f));
        mySequence.Append(DOTween.To(() => centerOfMassX, x => centerOfMassX = x, 3f, 4f));
        mySequence.Append(DOTween.To(() => centerOfMassX, x => centerOfMassX = x, 4f, 4f));
        mySequence.Append(DOTween.To(() => centerOfMassX, x => centerOfMassX = x, 5f, 4f));

        mySequence.OnUpdate(UpdateShipSimulate);
        mySequence.OnComplete(ShipSimulateFinished);

        mySequence.Play();
    }

    void ShipSimulateFinished()
    {
        StartCoroutine(CorouShipSimulateFinished());
    }
    IEnumerator CorouShipSimulateFinished()
    {
        yield return new WaitForSeconds(5f);

        controller_Cp.LoadNextScene();
    }

    //------------------------------
    void UpdateShipSimulate()
    {
        Vector3 offset = floatingGameEntity_Cp.centerOfMassOffset;
        offset.x = centerOfMassX;
        floatingGameEntity_Cp.centerOfMassOffset = offset;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
   }

    //------------------------------
    void UpdateShipImageRotation()
    {
        uiManager_Cp.RotateShipImage(transform.rotation.eulerAngles.z);
    }

    //------------------------------
    void UpdateWaveAmplitude()
    {
        string waveAmplitudeText = waveAmplitude.ToString();
        int subStringLength = waveAmplitudeText.Length > 4 ? 4 : waveAmplitudeText.Length;
        uiManager_Cp.SetWaveAmplitude(waveAmplitudeText.Substring(0, subStringLength) + "m");
    }

    //------------------------------
    void UpdateWindStrength()
    {
        string windStrengthText = windStrength.ToString();
        int subStringLength = windStrengthText.Length > 4 ? 4 : windStrengthText.Length;
        uiManager_Cp.SetWindStrength(windStrengthText.Substring(0, subStringLength) + "m/s");
    }

    //------------------------------
    public void PrepareFinish()
    {

        gameState = GameState_En.PreparedFinish;
    }

}
