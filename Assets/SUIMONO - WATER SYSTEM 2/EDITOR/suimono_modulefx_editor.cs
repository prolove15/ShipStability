using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;



[ExecuteInEditMode]
[CustomEditor(typeof(Suimono.Core.SuimonoModuleFX))]
public class suimono_modulefx_editor : Editor {

	public override void OnInspectorGUI() {



	//string renName = "";
	//int setRename = 0;
	
	//int localPresetIndex = -1;
	
	//bool showErrors = false;
	//bool showPresets = false;
 	//bool showSplash = false;
  	//bool showWaves = false;
  	//bool showGeneral = false;
  	//bool showSurface = false;
   	//bool showUnderwater = false;
  	//bool showEffects = false;
  	//bool showColor = false;
   	//bool showReflect = false;
 	//bool showFoam = false;
 	
 	Texture logoTex;
	Texture divTex;
	Texture divRevTex;
	//Texture divVertTex;
	//Texture divHorizTex;
	//Texture bgPreset;
	//Texture bgPresetSt;
	//Texture bgPresetNd;
			
 	//Color colorEnabled = new Color(1.0f,1.0f,1.0f,1.0f);
	//Color colorDisabled = new Color(1.0f,1.0f,1.0f,0.35f);
	//Color highlightColor2 = new Color(0.7f,1f,0.2f,0.6f);
	//Color highlightColor = new Color(1f,0.5f,0f,0.9f);
 	
 	//float aMin = 0.9f;
 	//float aMax = 1.0f;
  	//float apMin = 0.9f;
 	//float apMax = 1.1f;


	Suimono.Core.SuimonoModuleFX script = (Suimono.Core.SuimonoModuleFX) target;	
    Undo.RecordObject(target, "Changed Area Of Effect");


        //load textures
	 	logoTex = Resources.Load("textures/gui_tex_suimonologo_i") as Texture;
		divTex = Resources.Load("textures/gui_tex_suimonodiv_i") as Texture;
		divRevTex = Resources.Load("textures/gui_tex_suimonodivrev_i") as Texture;

		 if (EditorGUIUtility.isProSkin == true){
			divTex = Resources.Load("textures/gui_tex_suimonodiv") as Texture;
			logoTex = Resources.Load("textures/gui_tex_suimonologofxsys") as Texture;
			divRevTex = Resources.Load("textures/gui_tex_suimonodivrev") as Texture;
		 }


		
		//SUIMONO LOGO
		GUIContent buttonText = new GUIContent(""); 
		GUIStyle buttonStyle = GUIStyle.none; 
		Rect rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
		int margin = 15;

        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,36),logoTex);
        GUILayout.Space(25.0f);
        
        


         
        //SET SYSTEMS
		rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
		EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
		EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y+89+(script.effectsSystems.Length*28.0f),387,24),divRevTex);
		
		//string[] lbl = script.effectsLabels;
		
		if (script.effectsSystems.Length <= 0){
			EditorGUI.LabelField(new Rect(rt.x+margin+50, rt.y+35, 387, 18),"THERE ARE CURRENTLY NO FX SYSTEMS...");

		} else {
			for (int rL = 0; rL < script.effectsSystems.Length; rL++){
			if (rL <= script.effectsSystems.Length){
			
				EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y+35+(rL * 28.0f),387,24),divTex);

				script.effectsSystems[rL] = EditorGUI.ObjectField(new Rect(rt.x+margin+40f, rt.y+14f+(rL * 28.0f), 210f, 18f), script.effectsSystems[rL], typeof(Transform), true) as Transform;
				script.clampIndex[rL] = EditorGUI.Popup(new Rect(rt.x+margin+260f, rt.y+15f+(rL * 28.0f), 120f, 18f),"",script.clampIndex[rL], script.clampOptions.ToArray());

				if (GUI.Button(new Rect(rt.x+margin+10,rt.y+15+(rL * 28.0f),18,16),"-")){
					script.DeleteSystem(rL);
				}

				GUILayout.Space(28.0f);

			}
			}
		}
		
		if (GUI.Button(new Rect(rt.x+margin+90f,rt.y+20f+(script.effectsSystems.Length*28.0f),200f,18f),"+ ADD NEW SYSTEM")) script.AddSystem();
	  
        GUILayout.Space(100.0f);
         

        if (GUI.changed) EditorUtility.SetDirty (target);
    }
    
    
    
    
    
    
}