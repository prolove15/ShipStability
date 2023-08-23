

using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
[CustomEditor(typeof(Suimono.Core.Suimono_ShorelineObject))]
public class suimono_shorelineobject_editor : Editor {

	public override void OnInspectorGUI() {




	
	//string renName = "";
	//int setRename = 0;
	
	//int localPresetIndex = -1;
	int basePos = 0;
 
 	Texture logoTex;
	Texture divTex;
	//Texture divRevTex;
	//Texture divVertTex;
	Texture divHorizTex;
	//Texture bgPreset;
	//Texture bgPresetSt;
	//Texture bgPresetNd;
			
 	Color colorEnabled = new Color(1.0f,1.0f,1.0f,1.0f);
	//Color colorDisabled = new Color(1.0f,1.0f,1.0f,0.25f);
	//Color colorWarning = new Color(0.9f,0.5f,0.1f,1.0f);

	//Color highlightColor2 = new Color(0.7f,1f,0.2f,0.6f);
	//Color highlightColor = new Color(1f,0.5f,0f,0.9f);




	Suimono.Core.Suimono_ShorelineObject script = (Suimono.Core.Suimono_ShorelineObject) target;	
    Undo.RecordObject(target, "Changed Area Of Effect");
		

    	

    	
    	//load textures
	 	logoTex = Resources.Load("textures/gui_tex_suimonologo_i") as Texture;
		divTex = Resources.Load("textures/gui_tex_suimonodiv_i") as Texture;

		divHorizTex = Resources.Load("textures/gui_tex_suimono_divhorz") as Texture;


		if (EditorGUIUtility.isProSkin == true){
			divTex = Resources.Load("textures/gui_tex_suimonodiv") as Texture;
			logoTex = Resources.Load("textures/gui_tex_suimonologoshore") as Texture;
		}



		
		//SET SCREEN WIDTH
		int setWidth = (int)EditorGUIUtility.currentViewWidth-220;
		if (setWidth < 120) setWidth = 120;

		//SUIMONO LOGO
		GUIContent buttonText = new GUIContent(""); 
		GUIStyle buttonStyle = GUIStyle.none; 
		Rect rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
		int margin = 15;

		//start menu
        GUI.contentColor = new Color(1.0f,1.0f,1.0f,0.4f);
		EditorGUI.LabelField(new Rect(rt.x+margin+2, rt.y+37, 50, 18),"Version");
		GUI.contentColor = new Color(1.0f,1.0f,1.0f,0.6f);
		
		Rect linkVerRect = new Rect(rt.x+margin+51, rt.y+37, 90, 18);
		EditorGUI.LabelField(linkVerRect,script.suimonoVersionNumber);

		GUI.contentColor = new Color(1.0f,1.0f,1.0f,1.0f);
	    GUI.contentColor = new Color(1.0f,1.0f,1.0f,0.4f);
	    Rect linkHelpRect = new Rect(rt.x+margin+165, rt.y+37, 28, 18);
	    Rect linkBugRect = new Rect(rt.x+margin+165+42, rt.y+37, 65, 18);
	    Rect linkURLRect = new Rect(rt.x+margin+165+120, rt.y+37, 100, 18);
	    
		if (Event.current.type == EventType.MouseUp && linkHelpRect.Contains(Event.current.mousePosition)) Application.OpenURL("http://www.tanukidigital.com/forum/");
		if (Event.current.type == EventType.MouseUp && linkBugRect.Contains(Event.current.mousePosition)) Application.OpenURL("http://www.tanukidigital.com/forum/");
		if (Event.current.type == EventType.MouseUp && linkURLRect.Contains(Event.current.mousePosition)) Application.OpenURL("http://www.tanukidigital.com/suimono/");

		EditorGUI.LabelField(new Rect(rt.x+margin+165+30, rt.y+37, 220, 18),"|");
		EditorGUI.LabelField(new Rect(rt.x+margin+165+110, rt.y+37, 220, 18),"|");
		
		GUI.contentColor = new Color(1.0f,1.0f,1.0f,0.4f);
		EditorGUI.LabelField(linkHelpRect,"help");
		EditorGUI.LabelField(linkBugRect,"report bug");
		EditorGUI.LabelField(linkURLRect,"tanukidigital.com");
		// end menu
		
		
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,36),logoTex);
        GUILayout.Space(40.0f);
        
        
		//int tSpace = 0;





        // GENERAL SETTINGS
        GUI.contentColor = colorEnabled;
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
       
        script.debug = EditorGUI.Toggle(new Rect (rt.x+margin+5, rt.y+5, 20, 20), "", script.debug);
        GUI.Label (new Rect(rt.x+margin+25, rt.y+5, 300, 20), new GUIContent ("Debug Mode"));

        GUI.Label (new Rect(rt.x+margin+5, rt.y+25, 120, 18), new GUIContent ("Attach to Surface"));
        script.attachToSurface = EditorGUI.ObjectField(new Rect(rt.x+margin+130, rt.y+25, setWidth+35, 15), script.attachToSurface, typeof(Transform), true) as Transform;

		
		EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+50,372,1),divHorizTex);

        EditorGUI.LabelField(new Rect(rt.x+margin+5, rt.y+60, 120, 18),"Rendering Mode");
        script.shorelineModeIndex = EditorGUI.Popup(new Rect(rt.x+margin+130, rt.y+60, 100, 18),"", script.shorelineModeIndex, script.shorelineModeOptions.ToArray());
  		

 
  		// MODE = AUTOMATIC
  		if (script.shorelineModeIndex == 0){

  			EditorGUI.LabelField(new Rect(rt.x+margin+5, rt.y+80, 120, 18),"Generate Mode");
        	script.shorelineRunIndex = EditorGUI.Popup(new Rect(rt.x+margin+130, rt.y+80, 100, 18),"",script.shorelineRunIndex, script.shorelineRunOptions.ToArray());
  	
  			GUI.Label (new Rect (rt.x+margin+5, rt.y+100, 120, 20), new GUIContent ("Auto Y-Position"));
	        script.autoPosition = EditorGUI.Toggle(new Rect (rt.x+margin+130, rt.y+100, 20, 20), "", script.autoPosition);

			EditorGUI.LabelField(new Rect(rt.x+margin+5, rt.y+120, 130, 18),"Max Depth Range");
			script.maxDepth = EditorGUI.Slider(new Rect(rt.x+margin+130, rt.y+120, setWidth+35, 18),"",script.maxDepth,0.0f,100.0f);

			EditorGUI.LabelField(new Rect(rt.x+margin+5, rt.y+140, 130, 18),"Depth Range(R)");
			script.sceneDepth = EditorGUI.Slider(new Rect(rt.x+margin+130, rt.y+140, setWidth+35, 18),"",script.sceneDepth,0.0f,50.0f);

			EditorGUI.LabelField(new Rect(rt.x+margin+5, rt.y+160, 130, 18),"Shore Range(G)");
			script.shoreDepth = EditorGUI.Slider(new Rect(rt.x+margin+130, rt.y+160, setWidth+35, 18),"",script.shoreDepth,0.0f,50.0f);

			
			EditorGUI.LabelField(new Rect(rt.x+margin+5, rt.y+basePos+180, 180, 18),"Calculate Layers");
			if (script.moduleObject != null){
				script.depthLayer = EditorGUI.MaskField(new Rect(rt.x+margin+130, rt.y+basePos+180, 90, 18),"", script.depthLayer, script.suiLayerMasks.ToArray());
			}


			EditorGUI.LabelField(new Rect(rt.x+margin+5, rt.y+200, 130, 18),"Texture Resolution");
			script.useResolution = EditorGUI.IntField(new Rect(rt.x+margin+130, rt.y+200, setWidth+35, 18),"",script.useResolution);



			GUILayout.Space(200f);
		}

		// MODE = CUSTOM TEXTURE
		if (script.shorelineModeIndex == 1){
         	GUI.Label (new Rect (rt.x+margin+5, rt.y+80, 120, 15), new GUIContent ("Depth Texture"));
        	script.customDepthTex = EditorGUI.ObjectField(new Rect(rt.x+margin+130, rt.y+80, setWidth, 35), script.customDepthTex, typeof(Texture2D), true) as Texture2D;

        	GUILayout.Space(120f);
		}


        GUILayout.Space(10.0f);
  

	EditorGUILayout.Space();


    if (GUI.changed) EditorUtility.SetDirty(target);
    }
    
    
    
    
    
    
}