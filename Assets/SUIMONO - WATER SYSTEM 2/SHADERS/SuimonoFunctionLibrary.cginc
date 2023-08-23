// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

	struct appdata{
		float4 vertex : POSITION;
		float4 tangent : TANGENT;
		float3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
		float2 texcoord2 : TEXCOORD2;
	};

	sampler2D _suimono_TransTex;
	sampler2D _suimono_CausticTex;
	sampler2D _suimono_NormalsTex;
	sampler2D _suimono_WakeTex;
	sampler2D _suimono_shorelineTex;
	sampler2D _ReflectionTex;
	sampler2D _CameraDepthTexture;
	sampler2D _NormalTexS;
	sampler2D _NormalTexD;
	sampler2D _NormalTexR;
	sampler2D _FoamTex;
	sampler2D _WaveTex;
	sampler2D _MaskTex;

	samplerCUBE _SkyCubemap;
	samplerCUBE _CubeTex;

	half4 _SkyCubemap_HDR;
	half4 _SkyTint;
	half _SkyExposure;
	float _SkyRotation;
	float4 _TintColor2;
	fixed4 _shallowColor;
	fixed4 _depthColor;
	fixed4 _BlendColor;
	fixed4 _OverlayColor;
	fixed4 _ReflectionColor;
	fixed4 _SSSColor;
	fixed4 _SpecularColor;
	fixed4 _FoamColor;
	fixed4 _CausticsColor;
	float _Tess;
	float _minDist;
	float _maxDist;
	float _NormalStrength;
	float _RefractStrength;
	float _ReflectStrength;
	float _EdgeFade;
	float _EdgeFoamFade;
	float _HeightFoamAmt;
	float _HeightFoamHeight;
	float _ShallowFoamAmt;
	float _DepthFade;
	float _ShallowFade;
	float _RefractFade;
	float _CausticsFade;
	float _aberrationScale;
	float _heightScale;
	float _specularPower;
	float _roughness;
	float _roughness2;
	float _reflecTerm;
	float _reflecSharp;
	float4 _suimono_Dir;
	fixed3 worldNormalMask;
	half heightMask;
	half heightMaskRWave;
	float4 screenTransparent;
	float4 screenTransparentDistort;
	fixed4 screenCaustics;
	fixed4 screenCausticsDistort;
	float4 screenDepthMask;
	float4 screenDepthMaskDistort;
	fixed4 screenNormals;
	fixed4 screenWakeMask;
	float4 screenShoreline;
	float4 _scaleUVs;
	float _tessScale;
	float waterMask;
	float4 useReflectColor;
	float reflectDistance;
	float edgeDepth;
	float edgeDarkenDepth;
	float foamDepth;
	float foamDarkenDepth;
	float refractDepth;
	float _foamSpeed;
	float _foamScale;
	float _beaufortScale;
	float _turbulenceFactor;
	float _lgWaveHeight;
	float _lgWaveScale;
	float _beaufortFlag;
	float _causticsFlag;
	float _reflectFlag;
	float _reflectDynamicFlag;
	float _reflectFallback;
	half4 _reflectFallbackColor;
	float reflectMask;
	float _overallTransparency;
	float _overallBrightness;
	float4 tempWaves;
	float _shorelineScale;
	float _shorelineFrequency;
	float _shorelineSpeed;
	float _shorelineHeight;
	float _shorelineNorm;
	float _distortMask;
	float _depthMask;
	float _shallowMask;
	float _heightProjection;
	float underwaterDistance;
	float depthDifferential;
	float foamDifferential;
	float _suimono_uvx;
	float _suimono_uvy;
	half rHeight;
	float2 flowmap;
	half3 flowWaves;
	float2 flowSpeed;
	half3 tbNormal;
	float _isPlaying;
	float4 _UnderwaterColor;
	float _Suimono_isLinear;
	float3 ambientLight;

	float _suimonoTransDist;
	float transDist;

	float suimono_tess_on;
	float suimono_trans_on;
	float suimono_caustic_on;
	float suimono_refl_sky;
	float suimono_refl_cube;
	float suimono_refl_color;
	float suimono_dynrefl_on;
	float suimono_refl_off;
	float _enableFoam;

	//test
	int _sui_metalTestMode;

	struct Input {
		float2 uv_NormalTexS : TEXCOORD0;
		float2 uv2_FoamTex : TEXCOORD1;
		float2 uv3_WaveTex : TEXCOORD2;
		float4 screenPos;
		float3 worldRefl;
		INTERNAL_DATA
	};


	//TESSELLATION HELPER FUNCTIONS ---------------------------------------------------------------------------------------
		float UnityCalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess){
			float3 wpos = mul(unity_ObjectToWorld,vertex).xyz;
			float dist = distance(wpos, _WorldSpaceCameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}
		float4 UnityCalcTriEdgeTessFactors (float3 triVertexFactors){
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}
		float4 UnityDistanceBasedTess (float4 v0, float4 v1, float4 v2, float minDist, float maxDist, float tess){
			float3 f;
			f.x = UnityCalcDistanceTessFactor (v0,minDist,maxDist,tess);
			f.y = UnityCalcDistanceTessFactor (v1,minDist,maxDist,tess);
			f.z = UnityCalcDistanceTessFactor (v2,minDist,maxDist,tess);
			return UnityCalcTriEdgeTessFactors (f);
		}

	//APPLY TESSELLATION
	float4 SuimonoTess (appdata v0, appdata v1, appdata v2) {
		float4 tess;
		if (suimono_tess_on == 1.0){
			tess = UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _minDist, _maxDist, (_Tess*_tessScale));
		} else {
			tess = float4(1,1,1,1);
		}
		return tess;
	}


	//ROTATION FUNCTION ---------------------------------------------------------------------------------------------------
	float3 SuimonoRotateAroundYAxis(float3 v, float deg){
		float alpha = deg * UNITY_PI / 180.0;
		float sina, cosa;
		sincos(alpha, sina, cosa);
		float2x2 m = float2x2(cosa, -sina, sina, cosa);
		return float3(mul(m, v.xz), v.y).xzy;
	}


	//VERTEX --------------------------------------------------------------------------------------------------------------
	void SuimonoVert(inout appdata v){
		//vertex setup
		float4 pos = UnityObjectToClipPos(v.vertex);
		float4 projPos = ComputeScreenPos(pos);
		float screenHeight = tex2Dproj(_suimono_WakeTex, projPos).b;

		//calculate height
		float2 texUV = v.texcoord.xy*float2(_scaleUVs.x,_scaleUVs.y);
		float2 texUV2 = v.texcoord2.xy;
		float2 flowSpeed = float2(_suimono_Dir.x,_suimono_Dir.z)*_suimono_Dir.w;
		float2 waveOff1 = float2(_suimono_uvx,_suimono_uvy);

		//Calm Wave Height
		float CheightPos0 = saturate(lerp((float)-0.035, (float)0.5, tex2Dlod(_NormalTexS, float4(texUV + flowSpeed + waveOff1,0,0)).a ));
		float CheightPos1 = saturate(lerp((float)-0.035, (float)0.5, tex2Dlod(_NormalTexS, float4(texUV * 0.75 - (flowSpeed*0.25) + (waveOff1*0.75),0,0)).a ));

		//Turbulent Wave Height
		float TheightPos0 = saturate(lerp((float)-0.035, (float)0.5, tex2Dlod(_NormalTexD, float4(texUV + flowSpeed + waveOff1,0,0)).a ));
		float TheightPos1 = saturate(lerp((float)-0.035, (float)0.5, tex2Dlod(_NormalTexD, float4(texUV * 0.5 - (flowSpeed*0.25) + (waveOff1*0.5),0,0)).a ));

		//Rolling Wave Height
		float RheightPos0 = saturate(lerp((float)-0.035, (float)0.5, tex2Dlod(_NormalTexR, float4(texUV * _lgWaveScale + (flowSpeed*0.0625) + (waveOff1*_lgWaveScale),0,0)).a ));
		float RheightPos1 = saturate(lerp((float)-0.035, (float)0.5, tex2Dlod(_NormalTexR, float4(texUV * _lgWaveScale + (flowSpeed*0.125) + (waveOff1*_lgWaveScale),0,0)).a ));

		//displace height
		half cHeight = (CheightPos0 + CheightPos1) * 0.8;
		half tHeight = ((TheightPos0 * 0.2) + (TheightPos0 * TheightPos1 * 0.8)) * _turbulenceFactor * 0.5;
		half rHeight = ((RheightPos0 * 4.0) + (RheightPos1*3.0));

		//set final vertex
		half useY = 0.0;

		//waves
		useY += (cHeight * _heightScale);
		useY += (tHeight * _heightScale);
		useY += (rHeight * _lgWaveHeight);

		useY = lerp((half)0.0,useY,_heightProjection);


		#if UNITY_CAN_COMPILE_TESSELLATION
			//calculate shoreline wave height
			half2 p = projPos.xy/projPos.w;
			half3 shoreTex = tex2Dlod(_suimono_shorelineTex, float4(p.xy,0,0)).rgb;
			float2 flowmap = float2(saturate(shoreTex.r),_shorelineScale) * 2 *_shorelineFrequency;

			half3 flowWaves = tex2Dlod(_WaveTex, float4((float2(2.5,texUV2.y)*flowmap)-float2(_Time.x*_shorelineSpeed,1.0),0,0)).rgb * saturate(shoreTex.r*0.5);

			half shoreline = saturate(lerp((half)flowWaves.r, (half)flowWaves.g, shoreTex.g));
			shoreline = shoreline * (shoreTex.r);// + shoreTex.g);
			shoreline = flowWaves.r;

			shoreline = shoreline - (shoreTex.b * 2 *_shorelineHeight);
			half shoreFac = shoreline * _shorelineHeight * saturate(lerp((half)1.0, (half)0.0, (pos.z/300)));

			//clamp gamma/linear
			shoreFac = shoreFac * lerp((half)0.464646, (half)1.2, _Suimono_isLinear);

			//shoreline waves
			useY += (shoreFac*(1.8-shoreTex.g)*50.0);
			//useY += (shoreFac*(1.8)*50.0);

			//normalize
			useY = lerp((half)useY, (half)0.0, (shoreTex.r + shoreTex.b) * _shorelineNorm * 0.5);

			//calculate wake wave height
			half4 vWake = tex2Dlod(_suimono_WakeTex, float4(p.xy,0,0));
			half4 NWake = tex2Dlod(_suimono_NormalsTex, float4(p.xy,0,0));
			useY += saturate(vWake.b) * 0.15;

			//calculate normal offset
			useY += saturate(NWake.r-0.5 + NWake.g-0.5)*0.12;
		#endif

		//blend wave vertice offset in distance
		useY = lerp((half)0.0, (half)useY, tex2Dlod(_MaskTex, float4(v.texcoord.xy,0,0)).r);

		//normalize`
		//useY = lerp((half)useY, (half)0.0, (1-saturate(shoreTex.r + shoreTex.b)) * _shorelineNorm);

		//assign vertex height
		v.vertex.y = v.vertex.y + useY;

	}




	half3 SuimonoCalcNormal(float2 uvs, float2 uvWave, float4 uvScreen){
		half3 n;

		//calculate speed and flow vectors -----------------------------------------------------------------------------------------
		float2 flowSpeed = float2(_suimono_Dir.x,_suimono_Dir.z)*_suimono_Dir.w;
		float2 waveOff1 = float2(_suimono_uvx,_suimono_uvy);
		half nFac = 0.0;

		//convert normal for linear/gamma useage
		float pFac = lerp((float)1.0, (float)0.464646, _Suimono_isLinear);

		//Calm Wave Normals and Height
		half4 CnormalTex0 = tex2D(_NormalTexS, uvs + flowSpeed + waveOff1);
		half4 CnormalTex1 = tex2D(_NormalTexS, uvs * 0.75 - (flowSpeed*0.25) + (waveOff1*0.75));

		CnormalTex0.rgb = pow(CnormalTex0.rgb,pFac)*2.0-1.0;
		CnormalTex1.rgb = pow(CnormalTex1.rgb,pFac)*2.0-1.0;

		CnormalTex0.a = saturate(lerp((half)-0.035, (half)0.5, CnormalTex0.a));
		CnormalTex1.a = saturate(lerp((half)-0.035, (half)0.5, CnormalTex1.a));

		CnormalTex0.rgb = lerp(half3(0.0,0.0,1.0), (half3)CnormalTex0.rgb, _NormalStrength*_heightScale);
		CnormalTex1.rgb = lerp(half3(0.0,0.0,1.0), (half3)CnormalTex1.rgb, _NormalStrength*_heightScale);

		half cHeight = saturate(CnormalTex0.a + CnormalTex1.a);
		half3 cNormal = float3(CnormalTex0.xy + CnormalTex1.xy, CnormalTex0.z * CnormalTex1.z);

		nFac = saturate((0.0+lerp((half)1.0, (half)0.0, uvScreen.w/400.0)));
		cNormal = lerp(half3(0.0,0.0,1), (half3)cNormal, nFac);



		//Turbulent Wave Normals and Height
		half4 TnormalTex0 = tex2D(_NormalTexD, uvs + flowSpeed + (waveOff1));
		half4 TnormalTex1 = tex2D(_NormalTexD, uvs * 0.5 - (flowSpeed*0.25) + (waveOff1*0.5));

		TnormalTex0.rgb = pow(TnormalTex0.rgb,pFac)*2.0-1.0;
		TnormalTex1.rgb = pow(TnormalTex1.rgb,pFac)*2.0-1.0;

		TnormalTex0.a = saturate(lerp((half)-0.035, (half)0.5, TnormalTex0.a));
		TnormalTex1.a = saturate(lerp((half)-0.035, (half)0.5, TnormalTex1.a));

		TnormalTex0.rgb = lerp(half3(0.0,0.0,1.0), (half3)TnormalTex0.rgb, _NormalStrength);
		TnormalTex1.rgb = lerp(half3(0.0,0.0,1.0), (half3)TnormalTex1.rgb, _NormalStrength);

		half tHeight = saturate(TnormalTex0.a + TnormalTex1.a);
		half3 tNormal = float3(TnormalTex0.xy + TnormalTex1.xy, TnormalTex0.z * TnormalTex1.z);

		nFac = saturate((0.0+lerp((half)2.0, (half)0.0, uvScreen.w/4000.0)));
		tNormal = lerp(half3(0.0,0.0,1), (half3)tNormal,nFac);
		half3 tbNormal = tNormal;

		half3 TnormalTex3 = tex2D(_NormalTexD, uvs * 6.0 + (flowSpeed*0.5) + (waveOff1*6.0) + (tNormal.xy * 0.06 * lerp((half)0.0, (half)1.0, tNormal.z)));
		TnormalTex3 = pow(TnormalTex3,pFac)*2.0-1.0;
		TnormalTex3 = lerp(half3(0.0,0.0,1.0), (half3)TnormalTex3.rgb, _NormalStrength);
		tNormal = lerp((half3)tNormal, half3(tNormal.xy + TnormalTex3.xy, tNormal.z * TnormalTex3.z),saturate(lerp((half)0.0, (half)1.0, _turbulenceFactor)));

		nFac = saturate((0.0+lerp((half)2.0, (half)0.0, uvScreen.w/4000.0)));
		tNormal = lerp(half3(0,0,1), (half3)tNormal, nFac);


		//Rolling Wave Normals and Height
		half4 RnormalTex0 = tex2D(_NormalTexR, uvs * _lgWaveScale + (flowSpeed*0.0625) + (waveOff1*_lgWaveScale));
		half4 RnormalTex1 = tex2D(_NormalTexR, uvs * _lgWaveScale + (flowSpeed*0.125) + (waveOff1*_lgWaveScale));

		RnormalTex0.rgb = pow(RnormalTex0.rgb,pFac)*2.0-1.0;
		RnormalTex1.rgb = pow(RnormalTex1.rgb,pFac)*2.0-1.0;

		RnormalTex0.a = saturate(lerp((half)-0.035, (half)0.5, RnormalTex0.a));
		RnormalTex1.a = saturate(lerp((half)-0.035, (half)0.5, RnormalTex1.a));

		RnormalTex0.rgb = lerp(half3(0.0,0.0,1.0), (half3)RnormalTex0.rgb, _NormalStrength);
		RnormalTex1.rgb = lerp(half3(0.0,0.0,1.0), (half3)RnormalTex1.rgb, _NormalStrength);

		half rHeight = saturate(RnormalTex0.a + RnormalTex1.a);
		half3 rNormal = float3(RnormalTex0.xy + RnormalTex1.xy, RnormalTex0.z * RnormalTex1.z);

		nFac = saturate((0.0+lerp((half)1.0, (half)0.0,uvScreen.w/6000.0)));
		rNormal = lerp(half3(0.0,0.0,1), (half3)rNormal,nFac);


		//final combine
		float4 uv0 = uvScreen;
		uv0 = float4(max(0.001f, uv0.x),max(0.001f, uv0.y),max(0.001f, uv0.z),max(0.001f, uv0.w));
		screenShoreline = tex2Dproj(_suimono_shorelineTex, UNITY_PROJ_COORD(uv0));

		n = lerp((half3)cNormal, half3(cNormal.xy + tNormal.xy, cNormal.z * tNormal.z), _turbulenceFactor);
		n = lerp((half3)n, half3(rNormal.xy + n.xy, rNormal.z * n.z), saturate(lerp((half)-0.7, (half)1.2, _lgWaveHeight)));
		heightMask = (cHeight * _heightScale * 2);
		heightMask += (tHeight * _heightScale * 2);
		heightMask += (rHeight * _lgWaveHeight * 2);
		heightMaskRWave = lerp((half)heightMask, (half)rHeight, saturate(lerp((half)0.0, (half)1.0, _lgWaveHeight)));

		//Shore Map
		float2 flowmap = float2(saturate(screenShoreline.r),_shorelineScale) * 2 *_shorelineFrequency;
		half3 flowWaves = tex2D(_WaveTex,(float2(2.5,uvWave.y)*flowmap)-float2(_Time.x*_shorelineSpeed,1.0)).rgb * saturate(screenShoreline.r);
		tempWaves = saturate(lerp((float)flowWaves.r, (float)flowWaves.g, screenShoreline.g)) * screenShoreline.r;

		//final height
		heightMask += (tempWaves.r * _shorelineHeight * lerp((half)0.464646, (half)1.0, _Suimono_isLinear));

		//final normal
		n = lerp((half3)n, half3(1,1,1), saturate(flowWaves.g*2)*screenShoreline.g * _shorelineHeight);

		return n;
	}




	//LIGHTING --------------------------------------------------------------------------------------------------------------
	half4 LightingSuimonoLight (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten){

		// PHYSICAL BASED RENDERING
		fixed4 col = fixed4(0,0,0,1);
		float foamFac = foamDepth * _enableFoam * _FoamColor.a;
		_roughness = lerp((float)_roughness, (float)0.5 * _roughness, foamFac);
		_reflecTerm = lerp((float)_reflecTerm, (float)1.0, foamFac);

		//------------------------------
		//##  WORLD LIGHT FUNCTIONS  ##
		//------------------------------
		half4 outLight = _LightColor0;

		//-------------------------------
		//##  LIGHT TERM CALCULATION  ##
		//-------------------------------
		s.Normal = normalize(s.Normal);

		half NdotV = max(0,dot((half3)s.Normal,(half3)viewDir));
		half h = max(0,dot((half3)s.Normal, normalize((half3)lightDir+(half3)viewDir)));

		//------------------------------------
		//##  FRESNEL CALCULATION (Schlick)  ##
		//------------------------------------
		half fresnel;
		fresnel = _reflecTerm+(1.0-_reflecTerm)*pow((dot((half3)s.Normal, normalize((half3)lightDir+(half3)viewDir))),5);
		fresnel = fresnel * (_reflecTerm+(1.0-_reflecTerm)*pow((1.0-NdotV),5));
		fresnel = saturate(max(fresnel,_reflecTerm+(1.0-_reflecTerm)*pow((1.0-NdotV),5)));
		//sharpen reflection
		fresnel = saturate(lerp((half)_reflecSharp, (half)1.0, fresnel));

		//--------------------------------------------
		//##  NORMAL DISTRIBUTION FUNCTIONS (ggx)  ##
		//--------------------------------------------
		half ndf = 1.0;
		float ms = pow((_roughness),1.5);
		ndf = (ms*ms)/pow(((h*h)*((ms*ms)-1.0)+1.0),2.0);

		//----------------------------------------------
		//##  NORMAL DISTRIBUTION FUNCTIONS 2 (ggx)  ##
		//----------------------------------------------
		half ndf2 = 1.0;
		float ms2 = pow((_roughness2),1.5);
		ndf2 = (ms2*ms2)/pow(((h*h)*((ms2*ms2)-1.0)+1.0),2.0);

		//-------------------------------------
		//##  GEOMETRY FUNCTIONS (Implicit)  ##
		//-------------------------------------
		half gf = max(0,dot((half3)s.Normal, (half3)lightDir))*NdotV;

		//-----------------------------
		//##  FINAL COMBINATION  ##
		//-----------------------------

		//Blend Color
		screenCausticsDistort.rgb = lerp((fixed3)screenCausticsDistort.rgb, (fixed3)screenCausticsDistort.rgb*(_BlendColor.rgb*2),_BlendColor.a);
		col.rgb = screenCausticsDistort.rgb;

		//depth color
		col.rgb = lerp((fixed3)col.rgb, (fixed3)_shallowColor.rgb*outLight.rgb, saturate(_shallowMask) * _shallowColor.a);
		col.rgb = lerp((fixed3)col.rgb * lerp(fixed3(1.0,1.0,1.0), lerp(fixed3(0.65,0.65,0.65), fixed3(0.85,0.85,0.85), fresnel), saturate(_shallowMask)), col.rgb, atten);
		col.rgb = lerp((fixed3)col.rgb, (fixed3)_depthColor.rgb * (fixed3)outLight.rgb, saturate(_depthMask) * _depthColor.a);
		col.rgb = col.rgb * lerp(fixed3(1.4,1.4,1.4), fixed3(1,1,1), _Suimono_isLinear);
		col.rgb = lerp((fixed3)col.rgb * lerp(fixed3(1,1,1), lerp(fixed3(0.25,0.25,0.25), fixed3(0.45,0.45,0.45), fresnel), saturate(_depthMask)), col.rgb, atten);

		//blend distance (turned off)
		//col.rgb = lerp((fixed3)_depthColor.rgb*(fixed3)outLight.rgb, (fixed3)col.rgb, transDist);

		//sub surface color
		half sss= saturate(lerp((half)-5, (half)0.8, dot((half3)viewDir,(half3)-lightDir))) * saturate(0.1+s.Normal.z);
		col.rgb = ((fixed3)col.rgb + (fixed3)_SSSColor.rgb * lerp(fixed3(1,1,1), fixed3(2.2,2.2,2.2),_Suimono_isLinear) * outLight.rgb * sss * h);

		//blend overlay Color
		col.rgb = lerp((fixed3)col.rgb, (fixed3)_OverlayColor.rgb, _OverlayColor.a);

		//overall transparency blend
		if (suimono_trans_on == 1.0){
			screenTransparent.rgb = lerp((fixed3)col.rgb, (fixed3)screenTransparent.rgb, transDist);
			col.rgb = lerp((fixed3)screenTransparent.rgb, (fixed3)col.rgb, _overallTransparency);
		} else {
			col.rgb = col.rgb * outLight.rgb;
		}

		//overall brightness blend
		col.rgb = col.rgb * _overallBrightness;

		//Combine Reflections
		half refFac = 1.0;
		half refTerm = lerp(max((half)useReflectColor.r, max((half)useReflectColor.g, (half)useReflectColor.b)), (half)1.0, _reflecTerm);
		refTerm = refTerm * (fresnel*refFac);
		refTerm = refTerm *lerp(lerp((half)0.1, (half)0.6, fresnel), (half)1.0, atten);
		refTerm = refTerm * reflectMask;
		col.rgb = lerp((fixed3)col.rgb, (fixed3)useReflectColor.rgb * (fixed3)_ReflectionColor.rgb * 0.5, refTerm*_ReflectionColor.a);

		//adjust wave height coloring based on beaufort model
		col.rgb = lerp((fixed3)col.rgb, (fixed3)_shallowColor.rgb*(fixed3)outLight.rgb, saturate(lerp(-0.2,0.3,heightMask*(heightMaskRWave/10.0)))*0.1*saturate(lerp(-2.0,1.0,_beaufortScale)));

		//combine transparent edge
		if (suimono_trans_on == 1.0){
			col.rgb = lerp((fixed3)col.rgb, (fixed3)screenTransparent.rgb, edgeDepth);
		}

		//add direct light casting
		//col.rgb = saturate(col.rgb);

		//combine specular
		if (suimono_trans_on == 1.0){
			//rough
			col.rgb = col.rgb + (lerp((((fixed3(ndf2,ndf2,ndf2) * fixed3(h,h,h) * lerp(fixed3(1,1,1), fixed3(0.1,0.1,0.1), _roughness))) * _SpecularColor.rgb * outLight.a),0.0,edgeDepth)*_SpecularColor.a);
			
			//bright old
			//col.rgb = col.rgb + lerp((((fixed3(ndf,ndf,ndf) * fixed3(h,h,h) * lerp(fixed3(1,1,1), fixed3(0.1,0.1,0.1), _roughness)) * atten) * outLight.rgb)*1.75,0.0,edgeDepth);
		
			//bright new
			float hotspec = lerp((((fixed3(ndf,ndf,ndf) * fixed3(h,h,h) * lerp(fixed3(1,1,1), fixed3(0.1,0.1,0.1), _roughness)) * atten) )*1.75,0.0,edgeDepth);
			col.rgb = col.rgb + saturate(hotspec) * outLight.rgb * _specularPower;

		} else {
			//rough
			col.rgb = col.rgb + ((((fixed3(ndf2,ndf2,ndf2) * fixed3(h,h,h) * lerp(fixed3(1,1,1), fixed3(0.1,0.1,0.1), _roughness))) * _SpecularColor.rgb * outLight.a) * _SpecularColor.a);
			//bright
			col.rgb = col.rgb + (((fixed3(ndf,ndf,ndf) * fixed3(h,h,h) * lerp(fixed3(1,1,1), fixed3(0.1,0.1,0.1), _roughness)) * atten) * outLight.rgb)*1.75;
		}


		//atten
		half3 oCol = col.rgb;
		col.rgb = lerp((fixed3)col.rgb, (fixed3)col.rgb * fixed3(atten,atten,atten), atten);

			//combine foam
			half4 foam;
			foamDepth = (foamDepth);
			foam.a = (foamDepth * _FoamColor.a);

			foam.rgb = _FoamColor.rgb;

			//combine extreme edge darken
			foam.rgb = lerp((half3)foam.rgb, half3(0,0,0), edgeDarkenDepth*0.4*_FoamColor.a);

			//atten
			foam.rgb = (foam.rgb * lerp(half3(0.4,0.4,0.4), half3(1,1,1), atten));

			//combine bright specular
			half spc = ((ndf * h * lerp((half)1.0, (half)0.1, _roughness)) * atten);

			//fresnel reduction
			foam.a = foam.a * lerp((half)1.0, (half)0.8, fresnel-(spc*1.75));

			//foam Lighting
			foam.rgb = foam.rgb * outLight.rgb;

			//Add foam to main Return
			col.rgb = lerp((fixed3)col.rgb, (fixed3)foam.rgb, foamDepth * _FoamColor.a * (1.0-edgeDarkenDepth));

		//atten (note: destroys proper shadow rendering)
		col.rgb = col.rgb * atten;

//temp
if (_sui_metalTestMode == 2){
	col.rgb = screenTransparent.rgb;
}
if (_sui_metalTestMode == 3){
	col.rgb = screenTransparentDistort.rgb;
}

		col.a = (s.Alpha);

		return col;
	}







	//LIGHTING UNDER --------------------------------------------------------------------------------------------------------------
	half4 LightingSuimonoLightUnder (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten){

		// PHYSICAL BASED RENDERING
		fixed4 col = fixed4(0,0,0,1);
		_roughness = lerp((float)_roughness, (float)0.7, foamDepth);
		_reflecTerm = lerp((float)_reflecTerm, (float)0.0, foamDepth);

		//set beaufort scale settings
		_ReflectionColor.a = lerp(_ReflectionColor.a, max(min(_ReflectionColor.a,saturate(1.0-(_beaufortScale*1.6))),0.05), _beaufortFlag);

		//------------------------------
		//##  WORLD LIGHT FUNCTIONS  ##
		//------------------------------
		half4 outLight = _LightColor0;

		//-------------------------------
		//##  LIGHT TERM CALCULATION  ##
		//-------------------------------
		s.Normal = normalize(-s.Normal * half3(1.0,lerp((half)2.2, (half)1.0, _Suimono_isLinear),1.0));
		half NdotV = dot((half3)s.Normal, (half3)viewDir);
		half h = max(0,dot((half3)s.Normal, normalize((half3)lightDir+(half3)viewDir)));

		//underwater
		_reflecTerm = 0.1;

		//------------------------------------
		//##  FRESNEL CALULATION (Schlick)  ##
		//------------------------------------
		half fresnel;
		fresnel = _reflecTerm+(1.0-_reflecTerm)*pow((dot((half3)s.Normal, normalize((half3)lightDir+(half3)viewDir))),5);
		fresnel = fresnel * (_reflecTerm+(1.0-_reflecTerm)*pow((1.0-NdotV),5));
		fresnel = saturate(max(fresnel,_reflecTerm+(1.0-_reflecTerm)*pow((1.0-NdotV),5)));
		//sharpen reflection
		fresnel = saturate(lerp((half)_reflecSharp, (half)1.0, fresnel));

		//--------------------------------------------
		//##  NORMAL DISTRIBUTION FUNCTIONS (ggx)  ##
		//--------------------------------------------
		half ndf = 1.0;
		float ms = pow((max(0.001f,_roughness)*1.75),1.5);
		ndf = (ms*ms)/pow(((h*h)*((ms*ms)-1.0)+1.0),2.0);

		//-----------------------------
		//##  FINAL COMBINATION  ##
		//-----------------------------
		half transMask = saturate(lerp((half)2.0, (half)0.0, underwaterDistance));

		//reflection of underwater scene
		half underREFL = (saturate(lerp((half)1.5, (half)0.0, fresnel)));

		useReflectColor.rgb = lerp((float3)_UnderwaterColor.rgb, (float3)useReflectColor.rgb * (float3)_ReflectionColor.rgb, _ReflectionColor.a);

		col.rgb = lerp((fixed3)col.rgb, lerp((fixed3)_UnderwaterColor.rgb, ((fixed3)_UnderwaterColor.rgb * fixed3(2,2,2)) + (fixed3)useReflectColor.rgb, (useReflectColor.a/10.0)), _isPlaying);
		col.rgb  = lerp((fixed3)col.rgb, (fixed3)useReflectColor.rgb * fixed3(0.75,0.75,0.75), saturate(lerp(fixed3(-2,-2,-2), fixed3(6,6,6), dot((fixed3)-viewDir*fixed3(1,2.2,1),fixed3(0,1,0)))) * _isPlaying ) * (fixed3)outLight.rgb * outLight.a;

		//atten
		col.rgb = col.rgb * atten;

		half underREFL2 = saturate(lerp((half)2.0, (half)0.0, fresnel*6.5));
		half underREFL3 = saturate(lerp((half)-2.0, (half)35.0, saturate(underREFL2)));

		if (suimono_trans_on == 1.0){
			//col.rgb = lerp(col.rgb, screenTransparentDistort.rgb* lerp(1.0,1.0,_Suimono_isLinear), underREFL3 * _isPlaying);
			fixed tFac = lerp((fixed)1.0, (fixed)0.1, _roughness);
			col.rgb = lerp((fixed3)col.rgb, (fixed3)screenTransparentDistort.rgb * 0.5, underREFL3 * _isPlaying);
			col.rgb = col.rgb + lerp(fixed3(0,0,0), lerp(((saturate(fixed3(ndf,ndf,ndf) * fixed3(h,h,h) * fixed3(tFac,tFac,tFac)) * fixed3(atten,atten,atten)) * (fixed3)outLight.rgb)*fixed3(2,2,2), fixed3(0,0,0), edgeDepth),underREFL3 * _isPlaying);
		}

		//darken back side
		col.rgb = lerp(((fixed3)_UnderwaterColor.rgb * (fixed3)outLight.rgb * (fixed3)outLight.a), (fixed3)col.rgb, saturate(underREFL + 0.2));
		col.a = transMask;

		return col;
	}




	//SURFACE --------------------------------------------------------------------------------------------------------------
	void SuimonoSurf (Input IN, inout SurfaceOutput o) {

		//get normal data
		float4 uv0 = IN.screenPos;
		uv0 = float4(max(0.001f, uv0.x),max(0.001f, uv0.y),max(0.001f, uv0.z),max(0.001f, uv0.w));
		o.Normal = SuimonoCalcNormal(IN.uv_NormalTexS, IN.uv3_WaveTex.y, uv0);

		//get screen Textures ------------------------------------------------------------------------------------------------------
		float4 uv6 = IN.screenPos;
		uv6 = float4(max(0.001f, uv6.x),max(0.001f, uv6.y),max(0.001f, uv6.z),max(0.001f, uv6.w));
		uv6.y = uv6.y - 1 * ((heightMask * _heightProjection) + heightMaskRWave) * _heightScale;
		screenNormals = tex2Dproj(_suimono_NormalsTex, UNITY_PROJ_COORD(uv6));
		//screenWakeMask = tex2Dproj(_suimono_WakeTex, UNITY_PROJ_COORD(uv6));

		//surface depth gen
		//shoreMap = tex2D(_suimono_shorelineTex, UNITY_PROJ_COORD(uv6));

		//Blend screen normals with surface normals --------------------------------------------------------------------------------
		screenNormals.rgb = max(screenNormals.rgb,half3(0.25,0.25,1.0));
		half3 normalsMask = normalize(((half3)screenNormals.rgb*lerp((half)1.0, (half)2.2, _Suimono_isLinear))*2.0-1.0);
		#if UNITY_CAN_COMPILE_TESSELLATION
			//better normal combination available in dx11
			o.Normal = lerp(o.Normal,half3(0,0,1),screenNormals.a);
		#endif
		o.Normal = float3(o.Normal.xy + normalsMask.xy, o.Normal.z*normalsMask.z);

		//get distorted screen Textures --------------------------------------------------------------------------------------------
		float4 uv1 = IN.screenPos;
		uv1 = float4(max(0.001f, uv1.x),max(0.001f, uv1.y),max(0.001f, uv1.z),max(0.001f, uv1.w));
		uv1.y = uv1.y - (o.Normal.y *_RefractStrength);

		if (suimono_trans_on == 1.0){
			screenTransparent = tex2Dproj(_suimono_TransTex, UNITY_PROJ_COORD(uv0));
			screenTransparentDistort = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv1));
		} else {
			screenTransparent = half4(_depthColor.r,_depthColor.g,_depthColor.b,0.0);
			screenTransparentDistort = half4(_depthColor.r,_depthColor.g,_depthColor.b,0.0);
		}

		//calculate water depth masks ---------------------------------------------------------------------------------------------
		float sceneZ1 = 0;
		float sceneZ2 = 0;

		#if UNITY_CAN_COMPILE_TESSELLATION
			//DX11
			sceneZ1 = Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv0)).r);
			sceneZ2 = Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv1)).r);

			depthDifferential = (screenTransparentDistort.a-sceneZ2);
			foamDifferential = ((screenTransparent.a)-(sceneZ1));

			float clipDist = (1.0/_ProjectionParams.w);
			float rawDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv1)).r);
			float surfaceRawDepth = screenTransparentDistort.a * clipDist;
			depthDifferential = saturate((surfaceRawDepth - rawDepth) / 800.0);
			sceneZ2 = rawDepth * clipDist;

			float rawDepth2 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv0)).r);
			float surfaceRawDepth2 = screenTransparent.a * clipDist;
			foamDifferential = saturate((surfaceRawDepth2 - rawDepth2) / 800.0);
			sceneZ1 = rawDepth2 * clipDist;




			_distortMask = saturate(saturate(lerp((float)-0.1, (float)1.0, 100.0 * depthDifferential))*20.0);

			//fix distort
			//_distortMask = lerp((float)0, _distortMask, step(sceneZ1,sceneZ2+0.35));
			depthDifferential = lerp(foamDifferential, depthDifferential, _distortMask);
			edgeDepth = saturate(lerp((float)1.0, (float)-10.0, _EdgeFade * depthDifferential));
			edgeDarkenDepth = saturate(lerp((float)1.0, (float)-10.0, 800 * depthDifferential));
			float foamSpread = saturate(lerp((float)1.0, (float)-20.0, _EdgeFoamFade * depthDifferential)*2);
			foamSpread = foamSpread - saturate(lerp((float)1.0, (float)-10.0, 1000.0 * depthDifferential));
			refractDepth = saturate(lerp((float)1.0, (float)-2.0, _RefractFade * depthDifferential));
			_depthMask = saturate(lerp((float)0.0, (float)2.0,_DepthFade * depthDifferential));
			_shallowMask = saturate(lerp((float)0.0, (float)2.0,_ShallowFade * depthDifferential));
			screenDepthMask = half4(edgeDepth,foamSpread,0,0);
			screenDepthMaskDistort = half4(edgeDepth,foamDepth,0,0);

		#else

			//DX9
			sceneZ1 = Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv0)).r);
			sceneZ2 = Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv1)).r);
			depthDifferential = ((screenTransparentDistort.a+0.0001)-((sceneZ2-0.0003)));
			foamDifferential = ((screenTransparent.a)-(sceneZ1));

			float clipDist = (1.0/_ProjectionParams.w);

			float rawDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv1)).r);
			float surfaceRawDepth = screenTransparentDistort.a * clipDist;
			depthDifferential = saturate((surfaceRawDepth - rawDepth) / 800.0);
			sceneZ2 = rawDepth * clipDist;

			float rawDepth2 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv0)).r);
			float surfaceRawDepth2 = screenTransparent.a * clipDist;
			foamDifferential = saturate((surfaceRawDepth2 - rawDepth2) / 800.0);
			sceneZ1 = rawDepth2 * clipDist;


			_distortMask = saturate(saturate(lerp((float)-0.1, (float)1.0,100.0 * depthDifferential))*20.0);
			depthDifferential = lerp((float)foamDifferential, (float)depthDifferential, _distortMask);

			edgeDepth = saturate(lerp((float)1.0, (float)-22.0, _EdgeFade * depthDifferential));
			edgeDarkenDepth = saturate(lerp((float)1.0, (float)-22.0, 800 * depthDifferential));
			half foamSpread = saturate(lerp((half)1.0, (half)-40.0, _EdgeFoamFade * depthDifferential)*2);
			foamSpread = foamSpread - saturate(lerp((half)1.0, (half)-2.0, 1000.0 * depthDifferential));
			refractDepth = saturate(lerp((float)1.0, (float)-5.0, _RefractFade * depthDifferential));
			_depthMask = saturate(lerp((float)0.0, (float)4.1, _DepthFade * depthDifferential));
			_shallowMask = saturate(lerp((float)0.0, (float)4.1, _ShallowFade * depthDifferential));
			screenDepthMask = half4(edgeDepth,foamSpread,0,0);
			screenDepthMaskDistort = half4(edgeDepth,foamDepth,0,0);

		#endif




		//calculate caustic screen textures ----------------------------------------------------------------------------------------
		if (suimono_caustic_on == 1.0){

			#if UNITY_CAN_COMPILE_TESSELLATION
				screenCaustics = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv0));
				screenCausticsDistort = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv1));
			#else
				screenCausticsDistort = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv1));
				screenCaustics = screenCausticsDistort;
			#endif

			screenCaustics = lerp((fixed4)screenTransparent, (fixed4)screenCaustics, _causticsFlag);

	//test
	if (_sui_metalTestMode <= 0){
			screenCausticsDistort = lerp((fixed4)screenCausticsDistort, (fixed4)screenTransparentDistort, _causticsFlag);
	}
			half causticFade = saturate(lerp((half)1.0, (half)-2.0, _CausticsFade * depthDifferential));
			screenCaustics = lerp((fixed4)screenCaustics, (fixed4)screenTransparent, causticFade);
	//test
	if (_sui_metalTestMode <= 0){
			screenCausticsDistort = lerp((fixed4)screenCausticsDistort, (fixed4)screenTransparentDistort, causticFade);
	}
			screenCaustics.rgb = screenTransparent.rgb+((screenCaustics.rgb-screenTransparent.rgb)*_CausticsColor.rgb*_CausticsColor.a);
			screenCausticsDistort.rgb = screenTransparentDistort.rgb+((screenCausticsDistort.rgb-screenTransparentDistort.rgb)*_CausticsColor.rgb*_CausticsColor.a);

			//remove caustic distortion artifacts
			screenCausticsDistort = lerp((fixed4)screenCaustics, (fixed4)screenCausticsDistort, _distortMask);
		} else {
			//remove caustic transparent artifacts
			screenTransparentDistort = lerp((fixed4)screenTransparent, (fixed4)screenTransparentDistort, _distortMask);
			screenCaustics = screenTransparent;
			screenCausticsDistort = screenTransparentDistort;
		}

		//calculate water distortion mask ------------------------------------------------------------------------------------------
		waterMask = (sceneZ1/100.0);
		waterMask = saturate(max(waterMask,screenDepthMaskDistort.a) - waterMask);
		waterMask = saturate(waterMask * ((float)1.0+lerp((float)5.0, (float)-0.5, uv0.w/30.0)));

		//calculate chromatic aberation offset ------------------------------------------------------------------------------------
		#if UNITY_CAN_COMPILE_TESSELLATION
		if(_aberrationScale > 0){
			float4 uv2 = uv0;
			float4 uv3 = uv0;
			float abFac = _aberrationScale * saturate(lerp((float)0.04, (float)0.1+(float)screenDepthMask.a, o.Normal.y)) * (1.0-waterMask);
			uv2.x = uv2.x - abFac;
			uv3.x = uv3.x + abFac;
			screenCaustics.r = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv2)).r;
			screenCaustics.g = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv3)).g;
			float4 uv4 = uv1;
			float4 uv5 = uv1;
			uv4.x = uv4.x - abFac;
			uv5.x = uv5.x + abFac;
			screenCausticsDistort.r = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv4)).r;
			screenCausticsDistort.g = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv5)).g;
		}
		#endif

		//Calculate reflections ----------------------------------------------------------------------------------------------------
		useReflectColor = half4(1,1,1,1);
		float4 uv7 = IN.screenPos;
		float3 cubeRef;
		half4 fallbackReflectColor = half4(1,1,1,1);

		//reflection distance
		reflectMask = 0.0;

		//skbox reflection fallback
		if (suimono_refl_sky == 1.0){
			reflectMask = 1.0;
			cubeRef = WorldReflectionVector(IN, o.Normal * _ReflectStrength);
			cubeRef.y = cubeRef.y+0.015;
			cubeRef = SuimonoRotateAroundYAxis(cubeRef, -_SkyRotation);
			fallbackReflectColor.rgb = DecodeHDR(texCUBE(_SkyCubemap, cubeRef), _SkyCubemap_HDR);
			useReflectColor = fallbackReflectColor;
			useReflectColor = useReflectColor * _SkyTint * _SkyExposure * 2.2;
		}

		//cubemap reflection fallback
		if (suimono_refl_cube == 1.0){
			reflectMask = 1.0;
			cubeRef = WorldReflectionVector(IN, o.Normal * _ReflectStrength);
			fallbackReflectColor.rgb = texCUBE(_CubeTex, cubeRef).rgb;
			useReflectColor = fallbackReflectColor;
		}

		//color reflection fallback
		if (suimono_refl_color == 1.0){
			reflectMask = 1.0;
			fallbackReflectColor = _reflectFallbackColor;
			useReflectColor = fallbackReflectColor;
		}

		//dynamic Reflection
		if (suimono_dynrefl_on == 1.0){
			reflectMask = 1.0;
			uv7 = float4(max(0.001f, uv7.x),max(0.001f, uv7.y),max(0.001f, uv7.z),max(0.001f, uv7.w));
			uv7.y = uv7.y - (2.0 * o.Normal.y * _ReflectStrength);
			uv7.y = uv7.y + (screenNormals.r*0.5 + screenNormals.g*0.5)*0.2;
			uv7.y = uv7.y - (heightMask*_heightProjection)*(_lgWaveHeight*0.9);
			useReflectColor = lerp((float4)useReflectColor, (float4)tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(uv7)), _reflectDynamicFlag);
		}

		//fallback none
		if (suimono_refl_off == 1.0){
			reflectMask = lerp((float)0.0, (float)reflectMask, _reflectDynamicFlag);
		}

		reflectMask = lerp((float)0.0, (float)reflectMask, _reflectFlag);

		//rdistance mask
		reflectDistance = 1.0;



		//CALCULATE FOAM ----------------------------------------------------------------------------------------------------------
			float2 flowSpeed = float2(_suimono_Dir.x,_suimono_Dir.z)*_suimono_Dir.w;
			float2 waveOff1 = float2(_suimono_uvx,_suimono_uvy);
			fixed4 foamTex = tex2D(_FoamTex, (IN.uv_NormalTexS * _foamScale) + (flowSpeed*_foamSpeed) + (waveOff1 * _foamScale) - (tbNormal.xy * 5 * tbNormal.z));
			fixed4 foamMask = tex2D(_FoamTex, (IN.uv_NormalTexS * _foamScale * 0.0625) + (flowSpeed*_foamSpeed*0.0625) + (waveOff1 * _foamScale * 0.0625) - (tbNormal.xy * 5 * tbNormal.z));

			#if UNITY_CAN_COMPILE_TESSELLATION
				half ffac1 = -20.0;
				half ffac2 = -10.0;
			#else
				half ffac1 = -40.0;
				half ffac2 = -20.0;
			#endif

			//edge foam coverage
			half foamCover = saturate(lerp((half)foamTex.b, (half)ffac1, _EdgeFoamFade * 1.1 * foamDifferential));
			foamCover = foamCover + saturate(lerp((half)foamTex.r, (half)ffac1, _EdgeFoamFade * foamDifferential)*0.7);
			foamCover = foamCover + saturate(lerp((half)foamTex.g, (half)ffac1, _EdgeFoamFade * 0.9 * foamDifferential)*0.5);
			foamDepth = foamDepth + foamCover;

			//clamp foam edges
			foamDepth = saturate(foamDepth * ((edgeDepth*0.75 - foamDepth*0.5))*2.5);

			//wake foam coverage
			half wFoam = saturate(lerp((half)0.0, (half)foamTex.b, _EdgeFoamFade * 1.5 * screenWakeMask.r)*0.2);
			wFoam = wFoam + saturate(lerp((half)0.0, (half)foamTex.r*(half)0.2, _EdgeFoamFade * screenWakeMask.g));
			wFoam = wFoam + saturate(lerp((half)0.0, (half)foamTex.g*(half)0.2, _EdgeFoamFade * 0.2 * screenWakeMask.r));
			foamDepth = foamDepth + wFoam;

			//height wave foam (turbulence)
			half heightFoam = heightMask/(5*_HeightFoamHeight);
			heightFoam = lerp((half)0.0, (half)_HeightFoamAmt, heightFoam);
			foamDepth = foamDepth + (saturate(lerp((float)-1.1, (float)foamTex.a*(float)0.6, heightFoam)))*foamMask.r;
			foamDepth = foamDepth + (saturate(lerp((float)-1.9, (float)foamTex.a*(float)0.6, heightFoam)))*foamMask.r;
			foamDepth = foamDepth + (saturate(lerp((float)-0.7, (float)foamTex.g*(float)0.55, heightFoam)))*foamMask.r;
			foamDepth = foamDepth + (saturate(lerp((float)-1.6, (float)foamTex.r*(float)0.75, heightFoam)))*foamMask.a;
			foamDepth = saturate(foamDepth);

			//shoreline foam
			half shoreFoam = saturate(lerp((half)0.0, lerp((half)foamTex.g, (half)foamTex.b, screenShoreline.g)*(half)2, tempWaves.g*5));
			shoreFoam += saturate(lerp((half)0.0, lerp((half)foamTex.g, (half)foamTex.r, screenShoreline.g)*(half)2, flowWaves.g*5));
			shoreFoam += saturate(lerp((half)0.0, (half)foamTex.r*(half)2, tempWaves.g*5));
			shoreFoam += saturate(lerp((half)0.0, lerp((half)foamTex.r, (half)foamTex.b, screenShoreline.g)*(half)8, (screenShoreline.g * 0.2 * flowWaves.b)));
			foamDepth += shoreFoam * _ShallowFoamAmt * screenShoreline.g;
			foamDepth = saturate(foamDepth);

if (_enableFoam == 0){
	foamDepth = 0;
}

		//set transparent distance mask
		transDist = saturate((0.0+lerp((float)1.0, (float)0.0, uv0.w/(_suimonoTransDist*0.65) )));

		//set surface properties ---------------------------------------------------------------------------------------------------
		o.Albedo = 0.0;
		o.Alpha = 1.0;

		//Clip shoreline
		//half3 shoreTex = tex2Dproj(_suimono_shorelineTex, UNITY_PROJ_COORD(IN.screenPos)).rgb;
		//clip (0.0 - shoreTex.b);

//TEST
if (_sui_metalTestMode == 1){
	screenTransparent = tex2Dproj(_suimono_TransTex, UNITY_PROJ_COORD(uv0));
	screenTransparentDistort = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv1));
}
if (_sui_metalTestMode == 2){
	screenTransparent = tex2Dproj(_suimono_TransTex, UNITY_PROJ_COORD(uv0));
	screenTransparentDistort = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv1));
}
if (_sui_metalTestMode == 3){
	screenTransparent = tex2Dproj(_suimono_TransTex, UNITY_PROJ_COORD(uv0));
	screenTransparentDistort = tex2Dproj(_suimono_CausticTex, UNITY_PROJ_COORD(uv1));
}



}