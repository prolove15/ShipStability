using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
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

    //-------------------------------------------------- static fields
    public static Dictionary<string, string> words = new Dictionary<string, string>();

    public static Font font;

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
    }

    //------------------------------
    void InitComponents()
    {
        
    }

    #endregion

    //------------------------------
    public void LoadFile(string filePath)
    {
        // load per line of txt file.
        // per line has 2 section which seperates by ':' character
        // first section is origin words and second section is replace words
        // first section is given in the originWords variable above
        // find second section that responding to the first section in the file text lines.
        // if it is discovered, insert first section and last section to the words dictionary variable above
        // that's it. write code here.
        // Make sure the file path is valid before proceeding

        // Check if the directory exists, and create it if not
        string directoryPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directoryPath))
        {
            Debug.Log("Directory does not exist. Creating the directory.");
            Directory.CreateDirectory(directoryPath);
        }

        // Check if the file exists, and create it if not
        if (!File.Exists(filePath))
        {
            Debug.Log("File does not exist. Creating the file.");
            File.WriteAllText(filePath, ""); // Create an empty file
        }

        // Read all lines from the text file
        string[] lines = File.ReadAllLines(filePath);

        // Clear the existing words dictionary
        words.Clear();

        // Loop through each line in the file
        foreach (string line in lines)
        {
            // Split the line using the ':' character as separator
            string[] sections = line.Split(':');

            // Make sure there are exactly 2 sections in the line
            if (sections.Length != 2)
            {
                Debug.LogWarning("Invalid line format: " + line);
                continue;
            }

            // Trim whitespace from both sections
            string origin = sections[0].Trim();
            string replacement = sections[1].Trim();

            words[origin] = replacement;
        }
    }

    //------------------------------
    public void LoadFont()
    {
        font = Resources.Load<Font>("Fonts/Font");
    }

}
