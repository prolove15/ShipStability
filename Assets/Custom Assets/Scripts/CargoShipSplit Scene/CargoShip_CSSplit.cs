using cakeslice;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CargoShip_CSSplit : MonoBehaviour
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
    Transform steelParent_Tf;

    [SerializeField]
    Transform woodParent_Tf;

    //-------------------------------------------------- public fields
    public GameState_En gameState;

    //-------------------------------------------------- private fields
    List<Outline> steelOutlines = new List<Outline>();

    List<Outline> woodOutlines = new List<Outline>();

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
    }

    //------------------------------
    void InitComponents()
    {
        
    }

    //------------------------------
    void InitVariables()
    {
        for (int i = 0; i < steelParent_Tf.childCount; i++)
        {
            steelOutlines.Add(steelParent_Tf.GetChild(i).AddComponent<Outline>());
            steelOutlines[i].color = 0;
        }
        UpdateSteelOutline(false);

        for (int i = 0; i < woodParent_Tf.childCount; i++)
        {
            woodOutlines.Add(woodParent_Tf.GetChild(i).AddComponent<Outline>());
            woodOutlines[i].color = 0;
        }
        UpdateWoodOutline(false);
    }

    #endregion

    //------------------------------
    public void UpdateSteelOutline(bool flag)
    {
        for(int i = 0; i < steelOutlines.Count; i++)
        {
            steelOutlines[i].enabled = flag;
        }
    }

    //------------------------------
    public void UpdateWoodOutline(bool flag)
    {
        for(int i = 0; i < woodOutlines.Count; i++)
        {
            woodOutlines[i].enabled = flag;
        }
    }

}
