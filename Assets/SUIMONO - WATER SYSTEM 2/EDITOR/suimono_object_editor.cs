using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[CustomEditor(typeof(Suimono.Core.SuimonoObject))]
public class suimono_object_editor : Editor {

	int setRename = 0;
	string renName = "";


	public override void OnInspectorGUI() {


	int localPresetIndex = -1;
	int basePos = 0;
	
 	Texture logoTex;
	Texture divTex;
	Texture divHorizTex;
	Texture bgPreset;
	Texture bgPresetSt;
	Texture bgPresetNd;
			
 	Color colorEnabled = new Color(1.0f,1.0f,1.0f,1.0f);
	Color colorDisabled = new Color(1.0f,1.0f,1.0f,0.25f);
	Color colorWarning = new Color(0.9f,0.5f,0.1f,1.0f);
	Color highlightColor2 = new Color(0.7f,1f,0.2f,0.6f);
	Color highlightColor = new Color(1f,0.5f,0f,0.9f);


	//load textures
	divHorizTex = Resources.Load("textures/gui_tex_suimono_divhorz") as Texture;
 	
 	logoTex = Resources.Load("textures/gui_tex_suimonologo_i") as Texture;
	divTex = Resources.Load("textures/gui_tex_suimonodiv_i") as Texture;
	bgPreset = Resources.Load("textures/gui_bgpreset_i") as Texture;
	bgPresetSt = Resources.Load("textures/gui_bgpresetSt_i") as Texture;
	bgPresetNd = Resources.Load("textures/gui_bgpresetNd_i") as Texture;
	highlightColor = new Color(0.0f,0.81f,0.9f,0.6f);

	 if (EditorGUIUtility.isProSkin == true){
		divTex = Resources.Load("textures/gui_tex_suimonodiv") as Texture;
		logoTex = Resources.Load("textures/gui_tex_suimonologo") as Texture;
		bgPreset = Resources.Load("textures/gui_bgpreset") as Texture;
		bgPresetSt = Resources.Load("textures/gui_bgpresetSt") as Texture;
		bgPresetNd = Resources.Load("textures/gui_bgpresetNd") as Texture;
		highlightColor = new Color(1f,0.5f,0f,0.9f);
	 }


	Suimono.Core.SuimonoObject script = (Suimono.Core.SuimonoObject) target;	
    Undo.RecordObject(target, "Changed Area Of Effect");




    	if (localPresetIndex == -1) localPresetIndex = script.presetUseIndex;






		//SET SCREEN WIDTH
		int setWidth = (int)EditorGUIUtility.currentViewWidth-220;
		if (setWidth < 222) setWidth = 222;
		
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
        

        
	int tSpace = 0;


        // GENERAL SETTINGS
        GUI.contentColor = colorEnabled;
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        script.showGeneral = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showGeneral, "");
        GUI.Label (new Rect(rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("GENERAL SETTINGS"));

        GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 250, 20),"")) script.showGeneral = !script.showGeneral;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);

        EditorGUI.LabelField(new Rect(rt.x+margin+240, rt.y+6, 80, 18),"Mode");
        script.editorIndex = EditorGUI.Popup(new Rect(rt.x+margin+280, rt.y+6, 100, 18),"",script.editorIndex, script.editorOptions.ToArray());
        if (script.showGeneral){
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+30, 80, 18),"Surface Type");
			script.typeIndex = EditorGUI.Popup(new Rect(rt.x+margin+100, rt.y+30, 145, 18),"",script.typeIndex, script.typeOptions.ToArray());
			
			if (script.typeIndex == 0){
				EditorGUI.LabelField(new Rect(rt.x+margin+260, rt.y+30, 80, 18),"Ocean Scale");
	        	script.oceanScale = EditorGUI.FloatField(new Rect(rt.x+margin+343, rt.y+30, 30, 18),"",script.oceanScale);
			}

			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 80, 18),"Surface LOD");
			if (script.enableCustomMesh && script.typeIndex != 0){
				GUI.contentColor = colorWarning;
        		GUI.backgroundColor = colorWarning;
				EditorGUI.LabelField(new Rect(rt.x+margin+100, rt.y+50, 275, 18),"NOTE: Not available while using custom mesh!");
				GUI.contentColor = colorEnabled;
        		GUI.backgroundColor = colorEnabled;
			} else {
				if (script.typeIndex == 0){
					script.lodIndex = 0;
					GUI.contentColor = colorDisabled;
        			GUI.backgroundColor = colorDisabled;
				}
				if (script.typeIndex == 2){
					script.lodIndex = 3;
					GUI.contentColor = colorDisabled;
        			GUI.backgroundColor = colorDisabled;
				}
				script.lodIndex = EditorGUI.Popup(new Rect(rt.x+margin+100, rt.y+50, 145, 18),"",script.lodIndex, script.lodOptions.ToArray());
				
				GUI.contentColor = colorEnabled;
        		GUI.backgroundColor = colorEnabled;
			}


			


			//ADVANCED FX SETTINGS
			//EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+76,372,1),divHorizTex);
			//EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+80, 260, 18),"ADVANCED FX SETTINGS");
    		//EditorGUI.LabelField(new Rect(rt.x+margin+37, rt.y+100, 150, 18),"Enable Underwater FX");
			//script.enableUnderwaterFX = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+100, 30, 18),"", script.enableUnderwaterFX);
    		//EditorGUI.LabelField(new Rect(rt.x+margin+220, rt.y+100, 150, 18),"Enable Caustic FX");
			//script.enableCausticFX = EditorGUI.Toggle(new Rect(rt.x+margin+200, rt.y+100, 30, 18),"", script.enableCausticFX);


			//SCENE REFLECTIONS
			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
        	basePos = 77;
        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+basePos,372,1),divHorizTex);
        	script.enableReflections = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+basePos+5, 20, 18),"", script.useEnableReflections);
			EditorGUI.LabelField(new Rect(rt.x+margin+31, rt.y+basePos+5, 160, 18),"SCENE REFLECTIONS");
        	GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;

			if (script.useEnableReflections){

				script.enableDynamicReflections = EditorGUI.Toggle(new Rect(rt.x+margin+27, rt.y+basePos+28, 20, 18),"", script.useEnableDynamicReflections);
				EditorGUI.LabelField(new Rect(rt.x+margin+45, rt.y+basePos+28, 160, 18),"Enable Dynamic Reflections");

				if (!script.useEnableDynamicReflections){
					GUI.contentColor = colorDisabled;
        			GUI.backgroundColor = colorDisabled;
				}
				EditorGUI.LabelField(new Rect(rt.x+margin+27, rt.y+basePos+48, 180, 18),"Reflect Layers");
				if (script.gameObject.activeInHierarchy){
					script.reflectLayer = EditorGUI.MaskField(new Rect(rt.x+margin+120, rt.y+basePos+48, 90, 18),"", script.reflectLayer, script.suiLayerMasks.ToArray());
				}
				EditorGUI.LabelField(new Rect(rt.x+margin+225, rt.y+basePos+48, 180, 18),"Resolution");
				if (script.gameObject.activeInHierarchy){
					script.reflectResolution = EditorGUI.Popup(new Rect(rt.x+margin+295, rt.y+basePos+48, 90, 18),"", script.reflectResolution, script.resOptions.ToArray());
				}

				//EditorGUI.LabelField(new Rect(rt.x+margin+27, rt.y+basePos+68, 180, 18),"Reflection Distance");
				//script.reflectionDistance = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+basePos+68, setWidth, 18),"",script.reflectionDistance,0.0,100000.0);
				//EditorGUI.LabelField(new Rect(rt.x+margin+27, rt.y+basePos+88, 180, 18),"Reflection Spread");
				//script.reflectionSpread = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+basePos+88, setWidth, 18),"",script.reflectionSpread,0.0,1.0);
				

				GUI.contentColor = colorEnabled;
        		GUI.backgroundColor = colorEnabled;

        		if (script.enableDynamicReflections && script.useEnableDynamicReflections){
					GUI.contentColor = colorDisabled;
        			GUI.backgroundColor = colorDisabled;
				}
				EditorGUI.LabelField(new Rect(rt.x+margin+27, rt.y+basePos+78, 180, 18),"Fallback Mode");
				if (script.gameObject.activeInHierarchy){
					script.reflectFallback = EditorGUI.Popup(new Rect(rt.x+margin+120, rt.y+basePos+78, 120, 18),"", script.reflectFallback, script.resFallbackOptions.ToArray());
				}
				if (script.reflectFallback == 2){
					script.customRefCubemap = EditorGUI.ObjectField(new Rect(rt.x+margin+250, rt.y+basePos+78, 136, 16), script.customRefCubemap, typeof(Texture), true) as Texture;
				}
				if (script.reflectFallback == 3){
					script.customRefColor = EditorGUI.ColorField(new Rect(rt.x+margin+250, rt.y+basePos+78, 136, 16), script.customRefColor);
				}

				GUI.contentColor = colorEnabled;
        		GUI.backgroundColor = colorEnabled;

				basePos += 102;
				tSpace += 92;

			} else {
				basePos += 25;
			}


			//TESSELLATION
			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+basePos,372,1),divHorizTex);
        	script.enableTess = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+basePos+5, 20, 18),"", script.enableTess);
			EditorGUI.LabelField(new Rect(rt.x+margin+31, rt.y+basePos+5, 160, 18),"TESSELLATION");

			if (script.enableTess){

		        GUI.contentColor = colorEnabled;
		        GUI.backgroundColor = colorEnabled;

				#if !UNITY_STANDALONE_OSX
				if (script.typeIndex == 2){
		  			GUI.contentColor = colorDisabled;
					GUI.backgroundColor = colorDisabled;
				}
					

		  		if (!script.enableTess){
		  			GUI.contentColor = colorDisabled;
					GUI.backgroundColor = colorDisabled;
				}

				#endif							

				#if UNITY_STANDALONE_OSX
		  			GUI.contentColor = colorDisabled;
					GUI.backgroundColor = colorDisabled;
				#endif

				EditorGUI.LabelField(new Rect(rt.x+margin+27, rt.y+basePos+25, 140, 18),"Tessellation Factor");
				script.waveTessAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+basePos+25, setWidth, 18),"",script.waveTessAmt,0.001f,100.0f);
				EditorGUI.LabelField(new Rect(rt.x+margin+27, rt.y+basePos+45, 140, 18),"Tessellation Start");
				script.waveTessMin = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+basePos+45, setWidth, 18),"",script.waveTessMin,0.0f,1.0f);
				EditorGUI.LabelField(new Rect(rt.x+margin+27, rt.y+basePos+65, 140, 18),"Tessellation Spread");
				script.waveTessSpread = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+basePos+65, setWidth, 18),"",script.waveTessSpread,0.0f,1.0f);
				GUI.contentColor = colorEnabled;
				GUI.backgroundColor = colorEnabled;

			}
			
			//dx11 warning messages
			GUI.contentColor = colorWarning;
        	GUI.backgroundColor = colorWarning;
			//if (script.unityVersionIndex == 0){
			//	EditorGUI.LabelField(new Rect(rt.x+margin+137, rt.y+basePos+5, 260, 18),"NOTE: only available on PC in dx11 mode!");
			//}
			#if UNITY_STANDALONE_OSX
				EditorGUI.LabelField(new Rect(rt.x+margin+137, rt.y+basePos+5, 260, 18),"NOTE: only available on PC in dx11 mode!");
			#endif
			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;

			if (script.enableTess){
				basePos += 95;
				tSpace += 70;
			} else {
				basePos += 25;
			}
			


			// INTERACTION
			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+basePos,372,1),divHorizTex);

        	GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
			EditorGUI.LabelField(new Rect(rt.x+margin+31, rt.y+basePos+5, 140, 18),"ENABLE INTERACTION");
  			script.enableInteraction = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+basePos+5, 20, 18),"", script.enableInteraction);
			basePos += 25;



			// CUSTOM TEXTURES
			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+basePos,372,1),divHorizTex);

        	GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
			EditorGUI.LabelField(new Rect(rt.x+margin+31, rt.y+basePos+5, 140, 18),"CUSTOM TEXTURES");
  			script.enableCustomTextures = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+basePos+5, 20, 18),"", script.enableCustomTextures);

  		
  			if (script.enableCustomTextures){

				GUI.contentColor = colorDisabled;
        		GUI.backgroundColor = colorDisabled;
  				GUI.Label (new Rect(rt.x+margin+38, rt.y+basePos+86, 100, 18), new GUIContent ("RGBA Normal"));
				GUI.contentColor = colorEnabled;
        		GUI.backgroundColor = colorEnabled;
  				GUI.Label (new Rect(rt.x+margin+34, rt.y+basePos+72, 100, 18), new GUIContent ("Shallow Waves"));
  				//GUI.Label (new Rect(rt.x+margin+92, rt.y+basePos+72, 100, 18), new GUIContent ("Height"));
        		script.customTexNormal1 = EditorGUI.ObjectField(new Rect(rt.x+margin+34, rt.y+basePos+24, 95, 45), script.customTexNormal1, typeof(Texture2D), true) as Texture2D;
        		//script.customTexHeight1 = EditorGUI.ObjectField(new Rect(rt.x+margin+88, rt.y+basePos+24, 45, 45), script.customTexHeight1, typeof(Texture2D), true) as Texture2D;

				GUI.contentColor = colorDisabled;
        		GUI.backgroundColor = colorDisabled;
				GUI.Label (new Rect(rt.x+margin+162, rt.y+basePos+86, 100, 18), new GUIContent ("RGBA Normal"));
				GUI.contentColor = colorEnabled;
        		GUI.backgroundColor = colorEnabled;
        		GUI.Label (new Rect(rt.x+margin+152, rt.y+basePos+72, 100, 18), new GUIContent ("Turbulent Waves"));
  				//GUI.Label (new Rect(rt.x+margin+211, rt.y+basePos+72, 100, 18), new GUIContent ("Height"));
        		script.customTexNormal2 = EditorGUI.ObjectField(new Rect(rt.x+margin+155, rt.y+basePos+24, 95, 45), script.customTexNormal2, typeof(Texture2D), true) as Texture2D;
        		//script.customTexHeight2 = EditorGUI.ObjectField(new Rect(rt.x+margin+209, rt.y+basePos+24, 45, 45), script.customTexHeight2, typeof(Texture2D), true) as Texture2D;

				GUI.contentColor = colorDisabled;
        		GUI.backgroundColor = colorDisabled;
  				GUI.Label (new Rect(rt.x+margin+284, rt.y+basePos+86, 100, 18), new GUIContent ("RGBA Normal"));		
  				GUI.contentColor = colorEnabled;
        		GUI.backgroundColor = colorEnabled;
  				GUI.Label (new Rect(rt.x+margin+287, rt.y+basePos+72, 100, 18), new GUIContent ("Deep Waves"));
  				//GUI.Label (new Rect(rt.x+margin+335, rt.y+basePos+72, 100, 18), new GUIContent ("Height"));
        		script.customTexNormal3 = EditorGUI.ObjectField(new Rect(rt.x+margin+277, rt.y+basePos+24, 95, 45), script.customTexNormal3, typeof(Texture2D), true) as Texture2D;
        		//script.customTexHeight3 = EditorGUI.ObjectField(new Rect(rt.x+margin+333, rt.y+basePos+24, 45, 45), script.customTexHeight3, typeof(Texture2D), true) as Texture2D;


        		basePos += 110;
  				tSpace += 87;
			} else {
				basePos += 25;
			}




			// CUSTOM MESH
			GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+basePos,372,1),divHorizTex);

        	GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
			EditorGUI.LabelField(new Rect(rt.x+margin+31, rt.y+basePos+5, 140, 18),"CUSTOM MESH");
  			script.enableCustomMesh = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+basePos+5, 20, 18),"", script.enableCustomMesh);

  		
  			if (script.enableCustomMesh){

  				if (script.typeIndex != 0){
        			script.customMesh = EditorGUI.ObjectField(new Rect(rt.x+margin+171, rt.y+basePos+8, 200, 18), script.customMesh, typeof(Mesh), true) as Mesh;
        		

					EditorGUI.LabelField(new Rect(rt.x+margin+41, rt.y+basePos+28, 260, 18),"Custom UV Scaling");
					EditorGUI.LabelField(new Rect(rt.x+margin+170, rt.y+basePos+28, 20, 18),"X:");
					script.cmScaleX = EditorGUI.FloatField(new Rect(rt.x+margin+190, rt.y+basePos+28, 60, 18),"", script.cmScaleX);
					EditorGUI.LabelField(new Rect(rt.x+margin+270, rt.y+basePos+28, 20, 18),"Y:");
					script.cmScaleY = EditorGUI.FloatField(new Rect(rt.x+margin+290, rt.y+basePos+28, 60, 18),"", script.cmScaleY);

        		}

				//infinite ocean warning messages
				GUI.contentColor = colorWarning;
	        	GUI.backgroundColor = colorWarning;
				if (script.typeIndex == 0){
					EditorGUI.LabelField(new Rect(rt.x+margin+132, rt.y+basePos+5, 260, 18),"NOTE: Not available in Infinite Ocean Mode!");
				}
				GUI.contentColor = colorEnabled;
	        	GUI.backgroundColor = colorEnabled;


        		basePos += 55;
  				tSpace += 30;
			} else {
				basePos += 25;
			}



			GUILayout.Space(160.0f+tSpace);


			
		}
        GUILayout.Space(10.0f);



     

        
    if (script.editorIndex == 1){

        //WAVE SETTINGS
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        script.showWaves = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showWaves, "");
        GUI.Label (new Rect(rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("WAVE SETTINGS"));

        GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 20),"")) script.showWaves = !script.showWaves;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);

        if (script.showWaves){

        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+30, 140, 18),"Wave Scale (Beaufort)");
        	if (!script.customWaves){
        		script.beaufortScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+30, setWidth, 18),"",script.beaufortScale,0.0f,12.0f);
        	} else {
        		GUI.contentColor = colorWarning;
	        	GUI.backgroundColor = colorWarning;
	        	EditorGUI.LabelField(new Rect(rt.x+margin+165, rt.y+30, setWidth, 18),"Disabled: Using custom settings!");
        	}
        	GUI.contentColor = colorEnabled;
	        GUI.backgroundColor = colorEnabled;
			//if (script.useTenkoku == 1.0 && script.tenkokuUseWind){
			//	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+30, 90, 18),"Flow Direction");
			//	GUI.contentColor = colorDisabled;
			//	EditorGUI.LabelField(new Rect(rt.x+margin+165, rt.y+30, 290, 18),"Currently using Tenkoku settings...");
			//	GUI.contentColor = colorEnabled;
			//	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 90, 18),"Flow Speed");
			//	GUI.contentColor = colorDisabled;
			//	EditorGUI.LabelField(new Rect(rt.x+margin+165, rt.y+50, 290, 18),"Currently using Tenkoku settings...");
			//	GUI.contentColor = colorEnabled;
			//} else {
				EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 90, 18),"Wave Direction");
        		script.flowDirection = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+50, setWidth, 18),"",script.flowDirection,0.0f,360.0f);

        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+70, 90, 18),"Wave Speed");
				script.flowSpeed = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+70, setWidth, 18),"",script.flowSpeed,0.0f,0.1f);

				if (script.typeIndex == 0 && !script.customWaves){
        			GUI.contentColor = colorDisabled;
	        		GUI.backgroundColor = colorDisabled;
				}
        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+90, 90, 18),"Wave Scale");
				script.waveScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+90, setWidth, 18),"",script.waveScale,0.0f,5.0f);
        			GUI.contentColor = colorEnabled;
	        		GUI.backgroundColor = colorEnabled;

        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+110, 110, 18),"Height Projection");
				script.heightProjection = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+110, setWidth, 18),"",script.heightProjection,0.0f,1.0f);
			//}


			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+135,372,1),divHorizTex);

       		script.customWaves = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+140, 20, 20), "", script.customWaves);
        	GUI.Label (new Rect(rt.x+margin+30, rt.y+140, 300, 20), new GUIContent ("Use Custom Settings"));
        	if (!script.customWaves){
				GUI.contentColor = colorDisabled;
	        	GUI.backgroundColor = colorDisabled;
        	}

        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+160, 90, 18),"Wave Height");
				script.waveHeight = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+160, setWidth, 18),"",script.waveHeight,0.0f,4.0f);

        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+180, 120, 18),"Turbulence Amount");
				script.turbulenceFactor = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+180, setWidth, 18),"",script.turbulenceFactor,0.0f,1.0f);

        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+200, 120, 18),"Large Wave Height");
				script.lgWaveHeight = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+200, setWidth, 18),"",script.lgWaveHeight,0.0f,4.0f);

        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+220, 120, 18),"Large Wave Scale");
				script.lgWaveScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+220, setWidth, 18),"",script.lgWaveScale,0.0f,4.0f);

				GUILayout.Space(220.0f);

        	//} else {
			//	GUILayout.Space(110.0);
        	//}
        	GUI.contentColor = colorEnabled;
	        GUI.backgroundColor = colorEnabled;
        }
		GUILayout.Space(10.0f);




        //SHORELINE SETTINGS
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        script.showShore = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showShore, "");
        GUI.Label (new Rect(rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("SHORELINE SETTINGS"));
        
        GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 20),"")) script.showShore = !script.showShore;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);

        if (script.showShore){

    		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+30, 130, 18),"Shoreline Height");
			script.shorelineHeight = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+30, setWidth, 18),"",script.shorelineHeight,0.0f,1.0f);
    		
    		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 130, 18),"Shoreline Frequency");
			script.shorelineFreq = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+50, setWidth, 18),"",script.shorelineFreq,0.0f,1.0f);

    		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+70, 130, 18),"Shoreline Speed");
			script.shorelineSpeed = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+70, setWidth, 18),"",script.shorelineSpeed,0.0f,10.0f);

    		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+90, 130, 18),"Shoreline Normalize");
			script.shorelineNorm = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+90, setWidth, 18),"",script.shorelineNorm,0.0f,1.0f);


			GUILayout.Space(100.0f);

    	}
    	GUILayout.Space(10.0f);




        
        // SURFACE SETTINGS
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        script.showSurface = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showSurface, "");
        GUI.Label (new Rect(rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("WATER SURFACE"));

		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 20),"")) script.showSurface = !script.showSurface;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);
        
        if (script.showSurface){
        
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+30, 140, 18),"Overall Brightness");
            script.overallBright = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+30, setWidth, 18),"",script.overallBright,0.0f,10.0f);
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 140, 18),"Overall Transparency");
        	script.overallTransparency = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+50, setWidth, 18),"",script.overallTransparency,0.0f,1.0f);


			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+75,372,1),divHorizTex);

			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+85, 130, 18),"Edge Blending");
        	script.edgeAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+85, setWidth, 18),"",script.edgeAmt,0.0f,1.0f);

			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+105, 140, 18),"Depth Absorption");
			script.depthAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+105, setWidth, 18),"",script.depthAmt,0.0f,1.0f);
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+125, 140, 18),"Shallow Absorption");
			script.shallowAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+125, setWidth, 18),"",script.shallowAmt,0.0f,1.0f);

            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+145, 140, 18),"Depth Color");
            script.depthColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+145, setWidth, 18),"",script.depthColor);
            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+165, 140, 18),"Shallow Color");
            script.shallowColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+165, setWidth, 18),"",script.shallowColor);

        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+185, 130, 18),"Surface Blend Color");
            script.blendColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+185, setWidth, 18),"",script.blendColor);
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+205, 130, 18),"Surface Overlay Color");
            script.overlayColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+205, setWidth, 18),"",script.overlayColor);


			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+230,372,1),divHorizTex);


			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+240, 130, 18),"Refraction Amount");
        	script.refractStrength = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+240, setWidth, 18),"",script.refractStrength,0.0f,1.0f);
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+260, 130, 18),"Chromatic Shift");
        	script.aberrationScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+260, setWidth, 18),"",script.aberrationScale,0.0f,1.0f);

        	if (script.moduleObject != null){
	        	if (!script.moduleObject.enableCausticsBlending){
		    		GUI.contentColor = colorDisabled;
		        	GUI.backgroundColor = colorDisabled;
	        	}
        	}
        	
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+280, 130, 18),"Caustics Blend");
        	script.causticsFade = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+280, setWidth, 18),"",script.causticsFade,0.0f,1.0f);
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+300, 130, 18),"Caustics Color");
            script.causticsColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+300, setWidth, 18),"",script.causticsColor);
	    	GUI.contentColor = colorEnabled;
	        GUI.backgroundColor = colorEnabled;

			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+325,372,1),divHorizTex);


			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+335, 130, 18),"Reflection Blur");
        	script.reflectBlur = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+335, setWidth, 18),"",script.reflectBlur,0.0f,1.0f);	
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+355, 130, 18),"Reflection Distortion");
        	script.reflectProjection = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+355, setWidth, 18),"",script.reflectProjection,0.0f,1.0f);		
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+375, 130, 18),"Reflection Term");
        	script.reflectTerm = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+375, setWidth, 18),"",script.reflectTerm,0.0f,1.0f);		
        	
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+395, 130, 18),"Reflection Sharpen");
        	script.reflectSharpen = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+395, setWidth, 18),"",script.reflectSharpen,0.0f,1.0f);		
        	
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+415, 130, 18),"Reflection Color");
            script.reflectionColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+415, setWidth, 18),"",script.reflectionColor);

			
			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+440,372,1),divHorizTex);

			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+450, 140, 18),"Specular Power");
			script.specularPower = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+450, setWidth, 18),"",script.specularPower,0.0001f,1000.0f);

			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+470, 140, 18),"Hot Specular");
			script.roughness = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+470, setWidth, 18),"",script.roughness,0.0f,1.0f);
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+490, 140, 18),"Wide Specular");
			script.roughness2 = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+490, setWidth, 18),"",script.roughness2,0.0f,1.0f);
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+510, 130, 18),"Specular Color");
            script.specularColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+510, setWidth, 18),"",script.specularColor);
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+530, 130, 18),"Back Light Scatter");
            script.sssColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+530, setWidth, 18),"",script.sssColor);



  			
			GUILayout.Space(535.0f);
		}
        GUILayout.Space(10.0f);

      
      
    
      
 
      
      
        // FOAM SETTINGS
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        script.showFoam = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showFoam, "");
        GUI.Label (new Rect(rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("FOAM SETTINGS"));

        GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 20),"")) script.showFoam = !script.showFoam;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);

        if (script.showFoam){
			
			script.enableFoam = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+30, 20, 20),"", script.enableFoam);
			EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+30, 90, 18),"Enable Foam");
			if (!script.enableFoam){
	    		GUI.contentColor = colorDisabled;
	        	GUI.backgroundColor = colorDisabled;
	        }
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 90, 18),"Foam Scale");
        	script.foamScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+50, setWidth, 18),"",script.foamScale,0.0f,1.0f);
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+70, 90, 18),"Foam Speed");
        	script.foamSpeed = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+70, setWidth, 18),"",script.foamSpeed,0.0f,1.0f);
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+90, 90, 18),"Foam Color");
        	script.foamColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+90, setWidth, 18),"",script.foamColor);
        	
			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+115,372,1),divHorizTex);

        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+125, 90, 18),"Edge Foam");
        	script.edgeFoamAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+125, setWidth, 18),"",script.edgeFoamAmt,0.0f,0.9f);
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+145, 120, 18),"Shoreline Wave Foam");
        	script.shallowFoamAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+145, setWidth, 18),"",script.shallowFoamAmt,0.0f,2.0f);

			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+170,372,1),divHorizTex);

			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+180, 90, 18),"Wave Foam");
        	script.heightFoamAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+180, setWidth, 18),"",script.heightFoamAmt,0.0f,1.0f);
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+200, 90, 18),"Wave Height");
        	script.hFoamHeight = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+200, setWidth, 18),"",script.hFoamHeight,0.0f,1.0f);
        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+220, 90, 18),"Wave Spread");
        	script.hFoamSpread = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+220, setWidth, 18),"",script.hFoamSpread,0.0f,1.0f);

	    	GUI.contentColor = colorEnabled;
	        GUI.backgroundColor = colorEnabled;

			GUILayout.Space(220.0f);
		}
        GUILayout.Space(10.0f);
        
        


    
        
        
        

        // UNDERWATER SETTINGS
        //if (script.enableUnderwaterFX){
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        script.showUnderwater = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showUnderwater, "");
        GUI.Label (new Rect(rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("UNDERWATER"));

        GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 20),"")) script.showUnderwater = !script.showUnderwater;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);

        if (script.showUnderwater){

            //GUI.contentColor = colorDisabled;
	        //GUI.backgroundColor = colorDisabled;

				GUI.contentColor = colorEnabled;
		        GUI.backgroundColor = colorEnabled;

	        	EditorGUI.LabelField(new Rect(rt.x+margin+30, rt.y+30, 120, 18),"Enable Underwater");
				script.enableUnderwater = EditorGUI.Toggle(new Rect(rt.x+margin+10, rt.y+30, 30, 18),"", script.enableUnderwater);

				if (!script.enableUnderwater){
					GUI.contentColor = colorDisabled;
			        GUI.backgroundColor = colorDisabled;
				}

				EditorGUI.LabelField(new Rect(rt.x+margin+190, rt.y+30, 90, 18),"Enable Debris");
				script.enableUnderDebris = EditorGUI.Toggle(new Rect(rt.x+margin+170, rt.y+30, 30, 18),"", script.enableUnderDebris);
	    	

			//EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 130, 18),"Underwater Depth");
            //script.underwaterDepth = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+50, setWidth, 18),"",script.underwaterDepth,0.0,100.0);

			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+55,372,1),divHorizTex);


			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+65, 130, 18),"Light Factor");
            script.underLightFactor = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+65, setWidth, 18),"",script.underLightFactor,0.0f,1.0f);

            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+85, 130, 18),"Refraction Amount");
            script.underRefractionAmount = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+85, setWidth, 18),"",script.underRefractionAmount,0.0f,0.1f);
            
            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+105, 130, 18),"Refraction Scale");
            script.underRefractionScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+105, setWidth, 18),"",script.underRefractionScale,0.0f,3.0f);
            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+125, 130, 18),"Refraction Speed");
            script.underRefractionSpeed = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+125, setWidth, 18),"",script.underRefractionSpeed,0.0f,5.0f);

           // GUI.contentColor = colorDisabled;
	        //GUI.backgroundColor = colorDisabled;      
          	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+145, 90, 18),"Blur Amount");
            script.underBlurAmount = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+145, setWidth, 18),"",script.underBlurAmount,0.0f,1.0f);

            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+165, 170, 18),"Depth Darkening Range");
            script.underDarkRange = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+165, setWidth, 18),"",script.underDarkRange,0.0f,500.0f);
	    	//GUI.contentColor = colorEnabled;
	        //GUI.backgroundColor = colorEnabled;


			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+190,372,1),divHorizTex);


            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+200, 90, 18),"Fog Distance");
			script.underwaterFogDist = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+200, setWidth, 18),"",script.underwaterFogDist,0.0f,500.0f);
            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+220, 90, 18),"Fog Spread");
			script.underwaterFogSpread = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+220, setWidth, 18),"",script.underwaterFogSpread,-20.0f,20.0f);

            EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+240, 90, 18),"Fog Color");
			script.underwaterColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+240, setWidth, 18),"",script.underwaterColor);
            
           
            GUI.contentColor = colorEnabled;
        	GUI.backgroundColor = colorEnabled;
            

			GUILayout.Space(245.0f);
		//}
        
        }
        GUILayout.Space(10.0f);
    }      
    



    	
        // SIMPLE SETTINGS
		if (script.editorIndex == 0){
		
	        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
	        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
	        script.showSimpleEditor = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showSimpleEditor, "");
	        GUI.Label (new Rect(rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("SIMPLE WATER SETTINGS"));

	        GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
			if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 20),"")) script.showSimpleEditor = !script.showSimpleEditor;
			GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);

	        //set settings
			//script.flowSpeed = Mathf.Lerp(0.0,0.2,Mathf.Clamp01(script.waveFlowSpeed*20));
			//script.surfaceSmooth = Mathf.Lerp(0.0,1.0,script.simpleWaveHeight);
			//script.detailHeight = Mathf.Lerp(0.0,1.25,Mathf.Clamp(script.simpleWaveHeight*2,0.0,1.0));
			//script.detailScale = 0.1;
			//script.waveHeight = Mathf.Lerp(0.0,3.0,script.simpleWaveHeight);



	        if (script.showSimpleEditor){

	    		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+30, 160, 18),"Wave Scale (Beaufort)");
        		script.beaufortScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+30, setWidth, 18),"",script.beaufortScale,0.0f,20.0f);
				EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+50, 90, 18),"Wave Direction");
        		script.flowDirection = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+50, setWidth, 18),"",script.flowDirection,0.0f,360.0f);
        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+70, 90, 18),"Flow Speed");
				script.flowSpeed = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+70, setWidth, 18),"",script.flowSpeed,0.0f,0.1f);
        		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+90, 90, 18),"Wave Scale");
				script.waveScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+90, setWidth, 18),"",script.waveScale,0.0f,1.0f);


        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+115,372,1),divHorizTex);

				EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+120, 140, 18),"Refraction Amount");
				script.refractStrength = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+120, setWidth, 18),"",script.refractStrength,0.0f,1.0f);
				EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+140, 140, 18),"Reflection Distortion");
				script.reflectProjection = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+140, setWidth, 18),"",script.reflectProjection,0.0f,1.0f);
				EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+160, 140, 18),"Reflection Color");
				script.reflectionColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+160, setWidth, 18),"",script.reflectionColor);

        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+185,372,1),divHorizTex);

				EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+190, 140, 18),"Depth Absorption");
				script.depthAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+190, setWidth, 18),"",script.depthAmt,0.0f,1.0f);
                EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+210, 140, 18),"Depth Color");
            	script.depthColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+210, setWidth, 18),"",script.depthColor);
				script.shallowColor = new Color(0f,0f,0f,0f);


        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+235,372,1),divHorizTex);

				EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+240, 90, 18),"Foam Scale");
	        	script.foamScale = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+240, setWidth, 18),"",script.foamScale,0.0f,1.0f);
	        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+260, 90, 18),"Foam Amount");
        		script.edgeFoamAmt = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+260, setWidth, 18),"",script.edgeFoamAmt,0.0f,1.0f);
	        	EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+280, 90, 18),"Foam Color");
	        	script.foamColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+280, setWidth, 18),"",script.foamColor);

        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+10,rt.y+305,372,1),divHorizTex);


		        EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+310, 190, 18),"Underwater Refraction");
		        script.underRefractionAmount = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+310, setWidth, 18),"",script.underRefractionAmount,0.0f,0.1f);
		        EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+330, 190, 18),"Underwater Density");
				script.underwaterFogSpread = EditorGUI.Slider(new Rect(rt.x+margin+165, rt.y+330, setWidth, 18),"",script.underwaterFogSpread,-20.0f,20.0f);
		        EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+350, 190, 18),"Underwater Depth Color");
				script.underwaterColor = EditorGUI.ColorField(new Rect(rt.x+margin+165, rt.y+350, setWidth, 18),"",script.underwaterColor);      
			    script.underLightFactor = 1.0f;
			    script.underRefractionScale = 0.5f;
			    script.underRefractionSpeed = 1.0f;
			    script.underwaterFogDist = 15.0f;

        		GUI.contentColor = colorEnabled;
        		GUI.backgroundColor = colorEnabled;

	
			GUILayout.Space(355.0f);
			}

        	GUILayout.Space(10.0f);
		
		
   		}

		

        // PRESET MANAGER
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        script.showPresets = EditorGUI.Foldout(new Rect(rt.x+margin+3, rt.y+5, 20, 20), script.showPresets, "");
        GUI.Label (new Rect(rt.x+margin+10, rt.y+5, 300, 20), new GUIContent ("PRESET MANAGER"));

        GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,0.0f);
		if (GUI.Button(new Rect(rt.x+margin+10, rt.y+5, 370, 16),"")) script.showPresets = !script.showPresets;
		GUI.color = new Color(GUI.color.r,GUI.color.g,GUI.color.b,1.0f);

        if (script.showPresets){

			//int presetWidth = Screen.width-78;
			//if (presetWidth < 120) presetWidth = 120;
			int presetWidth = setWidth + 140;

			//select preset file
			EditorGUI.LabelField(new Rect(rt.x+margin+18, rt.y+24, 110, 18),"Use Preset Folder:");
			script.presetFileIndex = EditorGUI.Popup(new Rect(rt.x+margin+130, rt.y+24, 253, 13),"",script.presetFileIndex, script.presetDirs.ToArray());

			EditorGUI.LabelField(new Rect(rt.x+margin+18, rt.y+44, 100, 18),"Transition:");
			script.presetTransIndexFrm = EditorGUI.Popup(new Rect(rt.x+margin+85, rt.y+44, 80, 13),"",script.presetTransIndexFrm, script.presetFiles);
			EditorGUI.LabelField(new Rect(rt.x+margin+167, rt.y+44, 100, 18),"-->");
			script.presetTransIndexTo = EditorGUI.Popup(new Rect(rt.x+margin+194, rt.y+44, 80, 13),"",script.presetTransIndexTo, script.presetFiles);
			script.presetTransitionTime = EditorGUI.FloatField(new Rect(rt.x+margin+285, rt.y+43, 30, 18),script.presetTransitionTime);
    		string transAction = "Start";

        	//if (script.presetStartTransition) transAction = (script.presetTransitionCurrent*script.presetTransitionTime).ToString("F2");//"Stop";
        	
        	if(GUI.Button(new Rect(rt.x+margin+324, rt.y+44, 60, 15), transAction)){
        		//script.presetStartTransition = !script.presetStartTransition;
        		string foldName = script.presetDirs[script.presetFileIndex];
        		string frmName = script.presetFiles[script.presetTransIndexFrm];
        		string toName = script.presetFiles[script.presetTransIndexTo];
        		script.SuimonoTransitionPreset(foldName,frmName, toName, script.presetTransitionTime);
        	}

			//}
			
			
			//start presets
			GUI.color = new Color(1f,1f,1f,0.1f);
			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+20,rt.y+65,presetWidth,5),bgPresetSt); //364
			
			//fill presets
			//presetOptions
			int prx = 0;
			for (int pr = 0; pr <= script.presetFiles.Length; pr++){
				prx = pr;
				if (pr > 0){
					//background
					GUI.color = new Color(1f,1f,1f,0.1f);
					if ((pr/2.0f) > Mathf.Floor(pr/2.0f)) GUI.color = new Color(1f,1f,1f,0.13f);
					if (script.presetIndex == pr-1) GUI.color = highlightColor;
					EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+20,rt.y+70+(pr*13),presetWidth,12),bgPreset); //364
					

	            	
	            	//rename
	            	GUI.color = new Color(1f,1f,1f,0.4f);
	            	if (script.presetIndex == pr-1) GUI.color = highlightColor;
					if (GUI.Button(new Rect(rt.x+margin+21, rt.y+67+(pr*13)+2, 11, 11),"")){
						Debug.Log("rename");
						setRename = (pr+1);
					}
					if (setRename == (pr+1)){
						renName = EditorGUI.TextField(new Rect(rt.x+margin+32, rt.y+69+(pr*13), 200, 14), renName);
						GUI.color = highlightColor2;
						if (GUI.Button(new Rect(rt.x+margin+230, rt.y+69+(pr*13), 30, 14),"OK")){
							setRename = 0;
							script.PresetRename(pr-1,renName);
							renName="";
							
						}
						GUI.color = new Color(1f,1f,1f,0.4f);
						if (GUI.Button(new Rect(rt.x+margin+262, rt.y+69+(pr*13), 20, 14),"X")){
							setRename = 0;
						}
	            	}
	            	
	            	//add/delete
	            	GUI.color = new Color(1f,1f,1f,0.35f);
	            	if (script.presetIndex == pr-1) GUI.color = highlightColor;
					if (GUI.Button(new Rect(rt.x+margin+(presetWidth-35), rt.y+68+(pr*13)+1, 25, 12),"+")) script.PresetSave(script.presetFileIndex,pr-1);
	            	if (GUI.Button(new Rect(rt.x+margin+(presetWidth-9), rt.y+68+(pr*13)+1, 25, 12),"-")) script.PresetDelete(script.presetFileIndex,pr-1);

	           		GUI.color = new Color(1f,1f,1f,1f);


					//preset name/button
					if (setRename != (pr+1)){
						GUI.color = new Color(1f,1f,1f,0.75f);
	            		EditorGUI.LabelField(new Rect(rt.x+margin+32, rt.y+67+(pr*13), 300, 16), script.presetFiles[pr-1]);
	            		GUI.color = new Color(1f,1f,1f,0.12f);
	            		if (GUI.Button(new Rect(rt.x+margin+32, rt.y+67+(pr*13)+2, (presetWidth-72), 13),"")){
	            			localPresetIndex = pr;
	            			script.presetIndex = pr-1;
	            			script.PresetLoad(pr-1);
	            		}
	            	}

	           	} else {
	           	
	           	
	           		//background
					GUI.color = new Color(1f,1f,1f,0.1f);
					if ((pr/2.0f) > Mathf.Floor(pr/2.0f)) GUI.color = new Color(1f,1f,1f,0.13f);
					if (script.presetIndex == pr-1) GUI.color = highlightColor;
					EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+20,rt.y+70+(pr*13),presetWidth,12),bgPreset);
					
					//preset name/button
					if (setRename != (pr+1)){
						GUI.color = new Color(1f,1f,1f,0.75f);
	            		EditorGUI.LabelField(new Rect(rt.x+margin+32, rt.y+67+(pr*13), 300, 16), "- NONE -");
	            		GUI.color = new Color(0f,0f,0f,0.06f);
	            		if (GUI.Button(new Rect(rt.x+margin+32, rt.y+67+(pr*13)+2, (presetWidth-15), 13),"")){
	            			localPresetIndex = 0;
	            			script.presetIndex = -1;
	            		}
	            	}

	           	
	           	}
           	}

           	//end presets
           	GUI.color = new Color(1f,1f,1f,0.1f);
			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin+20,rt.y+81+((prx-1)*13),presetWidth,23),bgPresetNd);
			
			GUI.color = new Color(1f,1f,1f,1f);
			GUI.color = new Color(1f,1f,1f,0.55f);
			if (GUI.Button(new Rect(rt.x+margin+(presetWidth-49), rt.y+86+((prx)*13), 65, 18),"+ NEW")) script.PresetAdd();
			
			GUI.color = colorEnabled;
			

			
			GUILayout.Space(80.0f+(prx*12f)+10f);

		}
        GUILayout.Space(10.0f);
        
	    EditorGUILayout.Space();

		
        	
        if (GUI.changed) EditorUtility.SetDirty(target);
    }

    
    
}