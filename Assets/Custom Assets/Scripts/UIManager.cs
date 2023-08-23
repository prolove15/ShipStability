using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    //////////////////////////////////////////////////////////////////////
    // Types
    //////////////////////////////////////////////////////////////////////
    #region Types

    public enum GameState_En
    {
        Nothing, Inited, Playing, Finished,
        ShipImageShowed, ShipImageHided
    }

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Fields
    //////////////////////////////////////////////////////////////////////
    #region Fields

    //-------------------------------------------------- serialize fields
    [SerializeField]
    public Animator shipImageAnim_Cp;

    [SerializeField]
    public RectTransform shipImage_RT;

    [SerializeField]
    Transform graph_Tf;

    [SerializeField]
    Transform formula_Tf;

    [SerializeField]
    Text waveAmplitudeText_Cp;

    [SerializeField]
    Text windStrengthText_Cp;

    [SerializeField]
    public RectTransform[] waterImage_RTs;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    //-------------------------------------------------- private fields
    bool fixWaterImageFlag;

    Vector3 waterImageInitPos = new Vector3();

    Vector3 waterImageInitRot = new Vector3();

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

    //------------------------------
    void LateUpdate()
    {
        if(fixWaterImageFlag)
        {
            for(int i = 0; i < waterImage_RTs.Length; i++)
            {
                float waterParentRotZ = Mathf.Abs(waterImage_RTs[i].parent.rotation.eulerAngles.z);
                if(waterParentRotZ > 180f)
                {
                    waterParentRotZ = 360f - waterParentRotZ;
                }

                waterImage_RTs[i].position = waterImageInitPos -
                    new Vector3(0f, waterParentRotZ * 1f, 0f);

                // waterImage_RTs[i].rotation = Quaternion.Euler(waterImageInitRot);
                waterImage_RTs[i].rotation = Quaternion.RotateTowards(waterImage_RTs[i].rotation, Quaternion.identity,
                    20f * Time.deltaTime);
                // waterImage_RTs[i].rotation = Quaternion.Euler(0f, 0f, 0f);
            }
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
    }

    void InitComponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller>();
    }

    void InitVariables()
    {
        //
        if (waterImage_RTs.Length > 0)
        {
            waterImageInitPos = waterImage_RTs[0].position;
            waterImageInitRot = waterImage_RTs[0].rotation.eulerAngles;

            FixWaterImage();
        }

        //
        if(shipImage_RT)
        {
            shipImage_RT.localScale = new Vector3(0f, 1f, 1f);
        }
        if(graph_Tf)
        {
            graph_Tf.localScale = new Vector3(0f, 1f, 1f);
        }
        if(formula_Tf)
        {
            formula_Tf.localScale = new Vector3(0f, 1f, 1f);
        }
    }

    #endregion

    //------------------------------
    public void ShowHideShipImage(bool flag)
    {
        StartCoroutine(CorouShowHideShipImage(flag));
    }

    IEnumerator CorouShowHideShipImage(bool flag)
    {
        //
        if (shipImage_RT)
        {
            shipImage_RT.localScale = new Vector3(1f, 1f, 1f);
        }
        if (graph_Tf)
        {
            graph_Tf.localScale = new Vector3(1f, 1f, 1f);
        }
        if (formula_Tf)
        {
            formula_Tf.localScale = new Vector3(1f, 1f, 1f);
        }

        //
        if (flag)
        {
            shipImageAnim_Cp.SetInteger("flag", 1);
        }
        else
        {
            shipImageAnim_Cp.SetInteger("flag", -1);
        }

        yield return new WaitForSeconds(shipImageAnim_Cp.GetCurrentAnimatorStateInfo(0).length);

        gameState = GameState_En.ShipImageShowed;
    }

    //------------------------------
    public void RotateShipImage(float angle)
    {
        Vector3 shipEulerAngles = shipImage_RT.rotation.eulerAngles;
        shipImage_RT.rotation = Quaternion.Euler(shipEulerAngles.x, shipEulerAngles.y, angle);
    }

    //------------------------------
    public void SetWindStrength(string value)
    {
        windStrengthText_Cp.text = value;
    }

    public void SetWaveAmplitude(string value)
    {
        waveAmplitudeText_Cp.text = value;
    }

    //------------------------------
    public void FixWaterImage()
    {
        fixWaterImageFlag = true;
    }

}
