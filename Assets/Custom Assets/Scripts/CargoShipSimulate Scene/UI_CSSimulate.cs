using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CSSimulate : MonoBehaviour
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
    RectTransform shipImage_RT;

    [SerializeField]
    Text windStrengthText_Cp;

    [SerializeField]
    Text waveHeightText_Cp;

    [SerializeField]
    Slider waveSlider;

    [SerializeField]
    RectTransform CB_RT;

    [SerializeField]
    RectTransform CG_RT;

    [SerializeField]
    float CBMoveCoef = 1f;

    [SerializeField]
    RectTransform[] water_RTs;

    [SerializeField]
    bool movingCGFlag;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    public float minWaveHeight, maxWaveHeight;

    //-------------------------------------------------- private fields
    Vector3 originCGEulerAngles;

    float originCBanchoredPosX;

    float originCGanchoredPosX;

    float shipRotationAngle;

    Vector3[] waterOriginPos;

    Quaternion waterOriginRot;

    #endregion

    //////////////////////////////////////////////////////////////////////
    // Properties
    //////////////////////////////////////////////////////////////////////
    #region Properties

    //-------------------------------------------------- public properties

    //-------------------------------------------------- private properties
    Controller_CSSimulate controller_Cp;

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
    private void OnGUI()
    {
        if(gameState == GameState_En.Playing)
        {
            //
            float shipRotationAngle_tp = shipRotationAngle > 180f ? shipRotationAngle - 360f: shipRotationAngle;
            
            CG_RT.rotation = Quaternion.Euler(originCGEulerAngles);

            if (movingCGFlag)
            {
                CG_RT.anchoredPosition = new Vector2(originCGanchoredPosX - shipRotationAngle_tp * CBMoveCoef * 0.6f,
                    CG_RT.anchoredPosition.y);
            }

            CB_RT.anchoredPosition = new Vector2(originCBanchoredPosX - shipRotationAngle_tp * CBMoveCoef,
                CB_RT.anchoredPosition.y);

            //
            if(water_RTs != null)
            {
                if (water_RTs.Length > 0)
                {
                    for (int i = 0; i < water_RTs.Length; i++)
                    {
                        water_RTs[i].position = new Vector2(waterOriginPos[i].x,
                            waterOriginPos[i].y - Mathf.Abs(shipRotationAngle_tp * CBMoveCoef * 0.6f));

                        water_RTs[i].rotation = waterOriginRot;
                    }
                }
            }
        }
    }

    //------------------------------
    private void LateUpdate()
    {
        if (gameState == GameState_En.Playing)
        {
            //
            float shipRotationAngle_tp = shipRotationAngle > 180f ? shipRotationAngle - 360f : shipRotationAngle;

            CG_RT.rotation = Quaternion.Euler(originCGEulerAngles);

            if(movingCGFlag)
            {
                CG_RT.anchoredPosition = new Vector2(originCGanchoredPosX - shipRotationAngle_tp * CBMoveCoef * 1.5f,
                    CG_RT.anchoredPosition.y);
            }

            CB_RT.anchoredPosition = new Vector2(originCBanchoredPosX - shipRotationAngle_tp * CBMoveCoef,
                CB_RT.anchoredPosition.y);

            //
            if (water_RTs != null)
            {
                if (water_RTs.Length > 0)
                {
                    for (int i = 0; i < water_RTs.Length; i++)
                    {
                        water_RTs[i].position = new Vector2(waterOriginPos[i].x,
                           waterOriginPos[i].y - Mathf.Abs(shipRotationAngle_tp * CBMoveCoef * 0.6f));

                        water_RTs[i].rotation = waterOriginRot;
                    }
                }
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
    }

    //------------------------------
    void InitComponents()
    {
        controller_Cp = GameObject.FindWithTag("GameController").GetComponent<Controller_CSSimulate>();
    }

    //------------------------------
    void InitVariables()
    {
        maxWaveHeight = waveSlider.maxValue;
        minWaveHeight = waveSlider.minValue;

        originCGEulerAngles = CG_RT.transform.eulerAngles;
        originCGanchoredPosX = CG_RT.anchoredPosition.x;
        originCBanchoredPosX = CB_RT.anchoredPosition.x;

        if(water_RTs != null)
        {
            if (water_RTs.Length > 0)
            {
                waterOriginPos = new Vector3[water_RTs.Length];
                for (int i = 0; i < waterOriginPos.Length; i++)
                {
                    waterOriginPos[i] = water_RTs[i].position;
                }
                waterOriginRot = water_RTs[0].rotation;
            }
        }
    }

    #endregion

    //------------------------------
    public void HandleShipImage(float rotateAngle)
    {
        shipRotationAngle = rotateAngle;

        shipImage_RT.rotation = Quaternion.Euler(shipImage_RT.rotation.eulerAngles.x, shipImage_RT.rotation.eulerAngles.y,
            rotateAngle);
    }

    //------------------------------
    public void SetWindStrength(string value)
    {
        windStrengthText_Cp.text = value;
    }

    //------------------------------
    public void SetWaveHeightText(string value)
    {
        waveHeightText_Cp.text = value;
    }

    //------------------------------
    public void SetWaveHeight(float value)
    {
        waveSlider.value = value;
    }

}
