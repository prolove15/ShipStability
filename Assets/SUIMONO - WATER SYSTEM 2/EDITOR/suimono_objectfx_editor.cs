using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[CustomEditor(typeof(Suimono.Core.fx_EffectObject))]
public class suimono_objectfx_editor : Editor {

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
	
	Color colorEnabled = new Color(1.0f,1.0f,1.0f,1.0f);
	Color colorDisabled = new Color(1.0f,1.0f,1.0f,0.25f);

 	//Color colorEnabled = new Color(1.0f,1.0f,1.0f,1.0f);
	//Color colorDisabled = new Color(1.0f,1.0f,1.0f,0.35f);
	//Color highlightColor2 = new Color(0.7f,1f,0.2f,0.6f);
	//Color highlightColor = new Color(1f,0.5f,0f,0.9f);

 	float emMin = 1.0f;
 	float emMax = 3.0f;
 	float aMin = 0.9f;
 	float aMax = 1.0f;
  	float apMin = 0.9f;
 	float apMax = 1.1f;
 	float szMin = 0.5f;
 	float szMax = 1.5f;

		

	Suimono.Core.fx_EffectObject script = (Suimono.Core.fx_EffectObject) target;	
    Undo.RecordObject(target, "Changed Area Of Effect");


    	
    	//load textures
	 	logoTex = Resources.Load("textures/gui_tex_suimonologo_i") as Texture;
		divTex = Resources.Load("textures/gui_tex_suimonodiv_i") as Texture;
		divRevTex = Resources.Load("textures/gui_tex_suimonodivrev_i") as Texture;


		if (EditorGUIUtility.isProSkin == true){
			divTex = Resources.Load("textures/gui_tex_suimonodiv") as Texture;
			logoTex = Resources.Load("textures/gui_tex_suimonologofx") as Texture;
			divRevTex = Resources.Load("textures/gui_tex_suimonodivrev") as Texture;
		}


		//SUIMONO LOGO
		GUIContent buttonText = new GUIContent(""); 
		GUIStyle buttonStyle = GUIStyle.none; 
		Rect rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
		int margin = 15;
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,36),logoTex);

        GUILayout.Space(25.0f);
        
        
        
        //SET TYPE
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
        //EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y+30,387,24),divRevTex);

        	//if (GUI.Button(new Rect(rt.x+margin,rt.y+12,192,24),"Particle Effect")) script.typeIndex = 0;
        	//if (GUI.Button(new Rect(rt.x+margin+194,rt.y+12,192,24),"Audio Effect")) script.typeIndex = 1;
        	
        	if (GUI.Button(new Rect(rt.x + margin, rt.y + 12, 128, 24), "Particle Effect")) script.typeIndex = 0;
        	if (GUI.Button(new Rect(rt.x + margin + 130, rt.y + 12, 128, 24), "Audio Effect")) script.typeIndex = 1; 
        	if (GUI.Button(new Rect(rt.x + margin + 130 * 2, rt.y + 12, 128, 24), "Event Trigger")) script.typeIndex = 2;
        
        	
        GUILayout.Space(30.0f);
        
        

        //SET ACTION TYPE
        rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);

		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+12, 180, 18),"Action Type");
		script.actionIndex = EditorGUI.Popup(new Rect(rt.x+margin+110, rt.y+12, 260, 18),"",script.actionIndex, script.actionOptions.ToArray());

		if (script.actionIndex == 0 || script.actionIndex == 2){

			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+32, 90, 18),"Reset Time");
			script.actionReset = EditorGUI.FloatField(new Rect(rt.x+margin+110, rt.y+32, 40, 18),"",script.actionReset);

			if (script.actionIndex == 2){
				EditorGUI.LabelField(new Rect(rt.x+margin+225, rt.y+32, 100, 18),"Repeat Number");
				script.actionNum = EditorGUI.IntField(new Rect(rt.x+margin+325, rt.y+32, 40, 18),"",script.actionNum);
			}
			GUILayout.Space(20.0f);
		}

		GUILayout.Space(20.0f);


        
        //SET EFFECT PARTICLE UI
		if (script.typeIndex == 0){
         
            rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y+139,387,24),divRevTex);
			
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+15, 90, 18),"Particle Effect");

			script.systemIndex = EditorGUI.Popup(new Rect(rt.x+margin+110, rt.y+15, 260, 18),"",script.systemIndex, script.sysNames.ToArray());
		//script.systemIndex = EditorGUI.Popup(new Rect(rt.x+margin+110, rt.y+15, 260, 18),"",script.systemIndex, script.sysNames);

        	
        	emMin = script.emitNum.x;
			emMax = script.emitNum.y;
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+45, 130, 18),"Emit Number");
			EditorGUI.MinMaxSlider(new Rect(rt.x+margin+115, rt.y+45, 200, 18), ref emMin, ref emMax, 0.0f, 20.0f);
			EditorGUI.LabelField(new Rect(rt.x+margin+340, rt.y+45, 50, 18),Mathf.Floor(emMin)+"  "+Mathf.Floor(emMax));
			
			szMin = script.effectSize.x;
			szMax = script.effectSize.y;
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+65, 130, 18),"Particle Size");
			EditorGUI.MinMaxSlider(new Rect(rt.x+margin+115, rt.y+65, 200, 18), ref szMin, ref szMax,0.0f,4.0f);
			EditorGUI.LabelField(new Rect(rt.x+margin+340, rt.y+65, 50, 18),szMin.ToString("F1")+"  "+szMax.ToString("F1"));
				



			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+95, 130, 18),"Emission Speed");
			script.emitSpeed = EditorGUI.FloatField(new Rect(rt.x+margin+115, rt.y+95, 60, 18),"",script.emitSpeed);
				
			EditorGUI.LabelField(new Rect(rt.x+margin+210, rt.y+95, 130, 18),"Directional Speed");
			script.directionMultiplier = EditorGUI.FloatField(new Rect(rt.x+margin+320, rt.y+95, 40, 18),"",script.directionMultiplier);
			
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+115, 130, 18),"Emit At Surface");
			script.emitAtWaterLevel = EditorGUI.Toggle(new Rect(rt.x+margin+115, rt.y+114, 40, 18),"",script.emitAtWaterLevel);

			EditorGUI.LabelField(new Rect(rt.x+margin+210, rt.y+115, 130, 18),"Distance Range");
			script.effectDistance = EditorGUI.FloatField(new Rect(rt.x+margin+320, rt.y+114, 40, 18),"",script.effectDistance);
			
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+135, 130, 18),"Clamp Rotation");
			script.clampRot = EditorGUI.Toggle(new Rect(rt.x+margin+115, rt.y+134, 40, 18),"",script.clampRot);
			
			EditorGUI.LabelField(new Rect(rt.x+margin+155, rt.y+135, 130, 18),"Tint Color");
			script.tintCol = EditorGUI.ColorField(new Rect(rt.x+margin+225, rt.y+134, 140, 18),"",script.tintCol);

			script.emitNum.x = Mathf.Floor(emMin);
			script.emitNum.y = Mathf.Floor(emMax);
        	script.effectSize.x = szMin;
			script.effectSize.y = szMax;
			
        	GUILayout.Space(150.0f);
         
         }
        
        
        //SET EFFECT AUDIO UI
		if (script.typeIndex == 1){
         
            rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
        	EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
			EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y+139,387,24),divRevTex);
			
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+15, 130, 18),"Select Audio Sample");
			script.audioObj = EditorGUI.ObjectField(new Rect(rt.x+margin+150, rt.y+15, 220, 18), script.audioObj, typeof(AudioClip), true) as AudioClip;
			
			aMin = script.audioVol.x;
			aMax = script.audioVol.y;
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+45, 130, 18),"Audio Volume Range");
			EditorGUI.MinMaxSlider(new Rect(rt.x+margin+150, rt.y+45, 230, 18), ref aMin, ref aMax,0.0f,1.0f);

			apMin = script.audioPit.x;
			apMax = script.audioPit.y;
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+65, 130, 18),"Audio Pitch Range");
			EditorGUI.MinMaxSlider(new Rect(rt.x+margin+150, rt.y+65, 230, 18),ref apMin,ref apMax,0.0f,2.0f);
			
			EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+90, 150, 18),"Audio Repeat Speed");
			script.audioSpeed = EditorGUI.FloatField(new Rect(rt.x+margin+145, rt.y+90, 60, 18),"",script.audioSpeed);
			

			script.audioVol.x = aMin;
			script.audioVol.y = aMax;
			script.audioPit.x = apMin;
			script.audioPit.y = apMax;
			
        	GUILayout.Space(150.0f);
         }
        

        
        //SET EVENT UI
		if (script.typeIndex == 2){
            rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
            EditorGUI.DrawPreviewTexture(new Rect(rt.x + margin, rt.y, 387, 24), divTex);
            EditorGUI.DrawPreviewTexture(new Rect(rt.x + margin, rt.y + 139, 387, 24), divRevTex);

		    GUI.contentColor = colorDisabled;
		    GUI.backgroundColor = colorDisabled;
            EditorGUI.LabelField(new Rect(rt.x + margin + 10, rt.y + 10, 387, 18), "*Event will be triggered REGARDLESS of action type*");
   		    GUI.contentColor = colorEnabled;
		    GUI.backgroundColor = colorEnabled;


            EditorGUI.LabelField(new Rect(rt.x + margin + 30, rt.y + 30, 130, 18), "Enable Event Broadcasting");
            script.enableEvents = EditorGUI.Toggle(new Rect(rt.x + margin + 10, rt.y + 30, 40, 18), "", script.enableEvents);


			if (!script.enableEvents){
		    	GUI.contentColor = colorDisabled;
		    	GUI.backgroundColor = colorDisabled;
			}

            EditorGUI.LabelField(new Rect(rt.x + margin + 10, rt.y + 60, 130, 18), "Interval(sec)");
            script.eventInterval = EditorGUI.FloatField(new Rect(rt.x + margin + 115, rt.y + 60, 30, 18), "", script.eventInterval);
            EditorGUI.LabelField(new Rect(rt.x + margin + 10, rt.y + 80, 130, 18), "At Surface");
            script.eventAtSurface = EditorGUI.Toggle(new Rect(rt.x + margin + 115, rt.y + 80, 40, 18), "", script.eventAtSurface);

		    GUI.contentColor = colorEnabled;
		    GUI.backgroundColor = colorEnabled;

            GUILayout.Space(150.0f);
        }




        //SET RULES
		rt = GUILayoutUtility.GetRect(buttonText, buttonStyle);
		EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y,387,24),divTex);
		EditorGUI.DrawPreviewTexture(new Rect(rt.x+margin,rt.y+89f+(script.effectRule.Length*20.0f),387f,24f),divRevTex);
		
		EditorGUI.LabelField(new Rect(rt.x+margin+10, rt.y+15, 387, 18),"SET ACTIVATION RULES");
		
		if (script.effectRule.Length <= 0){
			EditorGUI.LabelField(new Rect(rt.x+margin+50, rt.y+35, 387, 18),"THERE ARE CURRENTLY NO RULES TO VIEW...");

		} else {
			for (int rL = 0; rL < script.effectRule.Length; rL++){
				if (rL < script.effectRule.Length){

				if (GUI.Button(new Rect(rt.x+margin+10f,rt.y+45f+(rL * 20.0f),18f,16f),"-")){
					script.DeleteRule(rL);
				}
				
				if (rL >= script.effectRule.Length) break;
				
				EditorGUI.LabelField(new Rect(rt.x+margin+35f, rt.y+45f+(rL * 20.0f), 70f, 18f),"RULE "+(rL+1));

				//-----------------
				if (script.ruleIndex[rL] > 3 && script.ruleIndex[rL] < 8){
					script.ruleIndex[rL] = EditorGUI.Popup(new Rect(rt.x+margin+90f, rt.y+45f+(rL * 20.0f), 246f, 18f),"",script.ruleIndex[rL], script.ruleOptions.ToArray());
					script.effectData[rL] = EditorGUI.FloatField(new Rect(rt.x+margin+340f, rt.y+44f+(rL * 20.0f), 30f, 18f),"",script.effectData[rL]);
				
				} else {
					script.ruleIndex[rL] = EditorGUI.Popup(new Rect(rt.x+margin+90f, rt.y+45f+(rL * 20.0f), 280f, 18f),"",script.ruleIndex[rL], script.ruleOptions.ToArray());
				}
				//-----------------

				GUILayout.Space(20.0f);
				}
			}
		}
		
		if (GUI.Button(new Rect(rt.x+margin+90f,rt.y+60f+(script.effectRule.Length*20.0f),200f,18f),"+ ADD NEW RULE")) script.AddRule();
	
		GUILayout.Space(100.0f);

        EditorUtility.SetDirty (script);
    }
    
}