using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(Suimono.Core.SuimonoModule))]
public class suimono_module_editor : Editor {

	public override void OnInspectorGUI() {


		//bool isPro = true;
		//bool showErrors = false;
		//bool showUnderwater = false;
		Color colorEnabled = new Color(1.0f,1.0f,1.0f,1.0f);
		Color colorDisabled = new Color(1.0f,1.0f,1.0f,0.25f);
		//Color colorWarning = new Color(0.9f,0.5f,0.1f,1.0f);
		Texture logoTexb;
		Texture divTex;
		//Texture divRevTex;
		//Texture divVertTex;
		Texture divHorizTex;
		
	 	//bool showCaustic = false;
	 	int verAdd = 0;
	 	
	 	Suimono.Core.SuimonoModule script = (Suimono.Core.SuimonoModule) target;
    		
    	Undo.RecordObject(target, "Changed Area Of Effect");
    	
        //load textures
		logoTexb = Resources.Load("textures/gui_tex_suimonologob_i") as Texture;
		divTex = Resources.Load("textures/gui_tex_suimonodiv_i") as Texture;
		//divRevTex = Resources.Load("textures/gui_tex_suimonodivrev") as Texture;
		//divVertTex = Resources.Load("textures/gui_tex_suimono_divvert") as Texture;
		divHorizTex = Resources.Load("textures/gui_tex_suimono_divhorz") as Texture;



		if (EditorGUIUtility.isProSkin == true){
			divTex = Resources.Load("textures/gui_tex_suimonodiv") as Texture;
			logoTexb = Resources.Load("textures/gui_tex_suimonologob") as Texture;
		}



		//int setWidth = Screen.width-220;
		//if (setWidth < 120) setWidth = 120;
		
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
		EditorGUI.LabelField( new Rect(rt.x+margin+2, rt.y+37, 50, 18),"Version");
		GUI.contentColor = new Color(1.0f,1.0f,1.0f,0.6f);
		
		Rect linkVerRect = new Rect(rt.x+margin+51, rt.y+37, 90, 18);
		EditorGUI.LabelField(linkVerRect,script.suimonoVersionNumber);
		//if (Event.current.type == EventType.MouseUp && linkVerRect.Contains(Event.current.mousePosition)) Application.OpenURL("http://www.tanukidigital.com/suimono/");
		
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

		GUI.contentColor  = new Color(1.0f,1.0f,1.0f,1.0f);
		

        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,36),logoTexb);
        GUILayout.Space(40.0f);
        
      
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        

        verAdd = 0;
        GUI.contentColor = colorEnabled;
		GUI.Label (new Rect (rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("CONFIGURATION"));


		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+25, 180, 18),"Camera Mode");
		script.cameraTypeIndex = EditorGUI.Popup(new Rect(rt.x+margin+165, rt.y+25, 150, 18),"", script.cameraTypeIndex, script.cameraTypeOptions.ToArray());
		if (script.cameraTypeIndex == 0){
        	GUI.contentColor = colorDisabled;
        	GUI.backgroundColor = colorDisabled;
		}

		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+verAdd+45, 140, 18),"Scene Camera Object");
		script.manualCamera = EditorGUI.ObjectField(new Rect(rt.x+margin+165, rt.y+verAdd+45, setWidth, 18),"",script.manualCamera, typeof(Transform), true) as Transform;

        GUI.contentColor = colorEnabled;
        GUI.backgroundColor = colorEnabled;
		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+verAdd+75, 140, 18),"Scene Track Object");
		script.setTrack = EditorGUI.ObjectField(new Rect(rt.x+margin+165, rt.y+verAdd+75, setWidth, 18),"",script.setTrack, typeof(Transform), true) as Transform;
		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+verAdd+95, 140, 18),"Scene Light Object");
		script.setLight = EditorGUI.ObjectField(new Rect(rt.x+margin+165, rt.y+verAdd+95, setWidth, 18),"",script.setLight, typeof(Light), true) as Light;

		EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+verAdd+120, 140, 18),"Set Automatic Layers");
		script.autoSetLayers = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+verAdd+120, 140, 18),"", script.autoSetLayers);

		EditorGUI.LabelField(new Rect(rt.x+margin+220, rt.y+verAdd+120, 140, 18),"Set Automatic FX");
		script.autoSetCameraFX = EditorGUI.Toggle(new Rect(rt.x+margin+200, rt.y+verAdd+120, 140, 18),"", script.autoSetCameraFX);

		GUILayout.Space(130.0f+verAdd);
		






		rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,407,24),divTex);
		script.showGeneral = EditorGUI.Foldout(new Rect (rt.x+margin+3, rt.y+5, 10, 20), script.showGeneral, "");
       	GUI.Label (new Rect (rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("GENERAL SETTINGS"));
       	
       	GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 20),"")) script.showGeneral = !script.showGeneral;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);

       	if (script.showGeneral){
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+30, 140, 18),"Enable Sounds");
			script.playSounds = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+30, setWidth, 18),"", script.playSounds);
			if (!script.playSounds){
				GUI.contentColor = colorDisabled;
				GUI.backgroundColor = colorDisabled;
			}
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 140, 18),"Max Sound Volume");
			script.maxVolume = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+50, setWidth, 18),"",script.maxVolume,0.0f,1.0f);
			GUI.contentColor = colorEnabled;
			GUI.backgroundColor = colorEnabled;

				EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+70, 160, 18),"Enable Underwater Sound");
				script.playSoundBelow = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+70, 20, 18),"", script.playSoundBelow);
				EditorGUI.LabelField(new Rect(rt.x+margin+220, rt.y+70, 160, 18),"Enable Above-Water Sound");
				script.playSoundAbove = EditorGUI.Toggle(new Rect(rt.x+margin+200, rt.y+70, 20, 18),"", script.playSoundAbove);

			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+95,372,1),divHorizTex);

			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+100, 140, 18),"Enable Underwater FX");
			script.enableUnderwaterFX = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+100, 130, 18),"", script.enableUnderwaterFX);
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+120, 140, 18),"Enable Transition FX");		
			script.enableTransition = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+120, 130, 18),"", script.enableTransition);

			script.transitionStrength = EditorGUI.Slider(new Rect(rt.x+margin+170, rt.y+120, 200, 18),"",script.transitionStrength,0.0f,2.0f);




			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+140, 110, 18),"Enable Interaction");
			script.enableInteraction = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+140, 130, 18),"", script.enableInteraction);

			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+160, 380, 18),"Disable MSAA (fixes display errors in Forward Rendering)");
			script.disableMSAA = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+160, 130, 18),"", script.disableMSAA);

			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+180, 380, 18),"Underwater Transition Threshold");
			script.underwaterThreshold = EditorGUI.FloatField(new Rect(rt.x+margin+210, rt.y+180, 50, 18),"",script.underwaterThreshold);

			


			GUILayout.Space(180.0f);
		}
		GUILayout.Space(10.0f);
		
		



		rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
       	script.showPerformance = EditorGUI.Foldout(new Rect (rt.x+margin+3, rt.y+5, 10, 20), script.showPerformance, "");
       	GUI.Label (new Rect (rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("ADVANCED WATER SETTINGS"));
       	
       	GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 20),"")) script.showPerformance = !script.showPerformance;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);


       	if (script.showPerformance){




        	GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
			script.enableTransparency = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+30, 20, 18),"", script.enableTransparency);
			if (!script.enableTransparency){
				GUI.contentColor = colorDisabled;
				GUI.backgroundColor = colorDisabled;
			}
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+30, 160, 18),"WATER TRANSPARENCY");

			if (!script.enableTransparency){
				GUI.contentColor = colorDisabled;
				GUI.backgroundColor = colorDisabled;
			}
			EditorGUI.LabelField(new Rect(rt.x+margin+230, rt.y+47, 180, 18),"Render Layers");
			if (script.gameObject.activeInHierarchy){
				script.transLayer = EditorGUI.MaskField(new Rect(rt.x+margin+230, rt.y+67, 150, 18),"", script.transLayer, script.suiLayerMasks.ToArray());
			}
			EditorGUI.LabelField(new Rect(rt.x+margin+110, rt.y+47, 180, 18),"Use Resolution");
			if (script.gameObject.activeInHierarchy){
				script.transResolution = EditorGUI.Popup(new Rect(rt.x+margin+110, rt.y+67, 100, 18),"", script.transResolution, script.resOptions.ToArray());
			}
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+47, 100, 18),"Distance");
	        script.transRenderDistance = EditorGUI.FloatField(new Rect(rt.x+margin+30, rt.y+67, 60, 18),"",script.transRenderDistance);



			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+95,372,1),divHorizTex);
			script.enableReflections = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+103, 20, 18),"", script.enableReflections);
			if (!script.enableReflections){
				GUI.contentColor = colorDisabled;
				GUI.backgroundColor = colorDisabled;
			}
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+103, 160, 18),"WATER REFLECTIONS");
			EditorGUI.LabelField(new Rect(rt.x+margin+50, rt.y+123, 170, 18),"Enable Dynamic Reflections");
			script.enableDynamicReflections = EditorGUI.Toggle(new Rect(rt.x+margin+30, rt.y+123, setWidth, 18),"", script.enableDynamicReflections);
        	GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;




        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+145,372,1),divHorizTex);
			script.enableCaustics = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+150, 20, 18),"", script.enableCaustics);

			if (!script.enableCaustics){
				GUI.contentColor = colorDisabled;
        		GUI.backgroundColor = colorDisabled;
			}

			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+150, 160, 18),"CAUSTIC FX");

			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+170, 140, 18),"FPS");
			script.suimonoModuleLibrary.causticObject.causticFPS = EditorGUI.IntField(new Rect(rt.x+margin+30, rt.y+185, 30, 18),"",script.suimonoModuleLibrary.causticObject.causticFPS);

			EditorGUI.LabelField(new Rect(rt.x+margin+90, rt.y+170, 140, 18),"Caustic Tint");
			script.suimonoModuleLibrary.causticObject.causticTint = EditorGUI.ColorField(new Rect(rt.x+margin+90, rt.y+187, 120, 14),"",script.suimonoModuleLibrary.causticObject.causticTint);
			
			EditorGUI.LabelField(new Rect(rt.x+margin+230, rt.y+170, 100, 18),"Render Layers");
			if (script.gameObject.activeInHierarchy){
				script.causticLayer = EditorGUI.MaskField(new Rect(rt.x+margin+230, rt.y+187, 155, 18),"", script.causticLayer, script.suiLayerMasks.ToArray());
			}

			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+210, 100, 18),"Bright");
			script.suimonoModuleLibrary.causticObject.causticIntensity = EditorGUI.Slider(new Rect(rt.x+margin+90, rt.y+210, 120, 18),"",script.suimonoModuleLibrary.causticObject.causticIntensity,0.0f,3.0f);

			EditorGUI.LabelField(new Rect(rt.x+margin+230, rt.y+210, 80, 18),"Scale");
			script.suimonoModuleLibrary.causticObject.causticScale = EditorGUI.Slider(new Rect(rt.x+margin+275, rt.y+210, 115, 18),"",script.suimonoModuleLibrary.causticObject.causticScale,0.5f,15.0f);

			if (script.setLight == null){
				GUI.contentColor = colorDisabled;
        		GUI.backgroundColor = colorDisabled;
			}
			script.suimonoModuleLibrary.causticObject.inheritLightColor = EditorGUI.Toggle(new Rect(rt.x+margin+30, rt.y+235, 120, 18),"", script.suimonoModuleLibrary.causticObject.inheritLightColor);
			EditorGUI.LabelField(new Rect(rt.x+margin+50, rt.y+235, 140, 18),"Inherit Light Color");
			script.suimonoModuleLibrary.causticObject.inheritLightDirection = EditorGUI.Toggle(new Rect(rt.x+margin+200, rt.y+235, 120, 18),"", script.suimonoModuleLibrary.causticObject.inheritLightDirection);
			EditorGUI.LabelField(new Rect(rt.x+margin+220, rt.y+235, 140, 18),"Inherit Light Direction");
        	
        	if (script.enableCaustics){
	        	GUI.contentColor = colorEnabled;
	        	GUI.backgroundColor = colorEnabled;
	        }
			script.enableCausticsBlending = EditorGUI.Toggle(new Rect(rt.x+margin+30, rt.y+255, 120, 18),"", script.enableCausticsBlending);
			EditorGUI.LabelField(new Rect(rt.x+margin+50, rt.y+255, 320, 18),"Enable Advanced Caustic FX (effects performance)");

        	GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;






        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+285,372,1),divHorizTex);
			script.enableAdvancedDistort = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+290, 20, 18),"", script.enableAdvancedDistort);
			if (!script.enableAdvancedDistort){
				GUI.contentColor = colorDisabled;
        		GUI.backgroundColor = colorDisabled;
			}
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+290, 340, 18),"ADVANCED WAKE AND DISTORTION EFFECTS");
        	GUI.contentColor = colorDisabled;
        	GUI.backgroundColor = colorDisabled;
        	EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+305, 340, 18),"Enables rendering of advanced scene effects such as wake");
        	EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+317, 340, 18),"and boat trail generation and water ripple distortion fx.");
			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;



        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+340,372,1),divHorizTex);
			script.enableAutoAdvance = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+345, 20, 18),"", script.enableAutoAdvance);
			if (!script.enableAutoAdvance){
				GUI.contentColor = colorDisabled;
        		GUI.backgroundColor = colorDisabled;
			}
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+345, 340, 18),"AUTO-ADVANCE SYSTEM TIMER");
	        script.systemTime = EditorGUI.FloatField(new Rect(rt.x+margin+260, rt.y+345, 120, 18),"",script.systemTime);
			

        	GUI.contentColor = colorDisabled;
        	GUI.backgroundColor = colorDisabled;
        	EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+360, 340, 18),"the 'systemTime' variable is automatically advanced by");
        	EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+372, 340, 18),"default.  This variable can be shared across a network to");
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+384, 340, 18),"sync wave positions between client and server computers.");


			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;


			
			GUILayout.Space(390.0f);
				
		}
       	GUILayout.Space(10.0f);
			

				






       	if (script.useTenkoku == 1.0f){
			rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
	        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
	       	script.showTenkoku = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showTenkoku, "");
	       	GUI.Label (new Rect (rt.x+margin+20, rt.y+5, 300, 20), new GUIContent ("TENKOKU SKY SYSTEM - INTEGRATION"));
	       	 	
	       	if (script.showTenkoku){

	       		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+30, 140, 18),"Use Wind Settings");
	       		script.tenkokuUseWind = EditorGUI.Toggle(new Rect(rt.x+margin+125, rt.y+30, setWidth, 18),"", script.tenkokuUseWind);
				
				EditorGUI.LabelField(new Rect(rt.x+margin+195, rt.y+30, 150, 18),"Calculate Sky Reflections");
	       		script.tenkokuUseReflect = EditorGUI.Toggle(new Rect(rt.x+margin+350, rt.y+30, setWidth, 18),"", script.tenkokuUseReflect);

			}
	       	GUILayout.Space(50.0f);
       	}
	    GUILayout.Space(10.0f);

        	
        if (GUI.changed)
            EditorUtility.SetDirty (target);


	}
}