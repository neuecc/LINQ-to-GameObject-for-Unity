// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D


/* 

one of the most common shader in AngryBots, requires lightmap

handles simple CUBE map reflections (higher end) or 
fake planar (y is up) reflections (low end)

*/

Shader "AngryBots/ReflectiveBackgroundPlanarGeometry" {
	
Properties {
	_MainTex ("Base", 2D) = "white" {}
	_Cube ("Cube", Cube) = "" {}
	_2DReflect ("2D Reflection", 2D) = "" {}
	_Normal("Normal", 2D) = "bump" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 0	
}

CGINCLUDE		

// interpolator structs

struct v2f 
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	half2 uv2 : TEXCOORD1;
	half2 uvLM : TEXCOORD2;
};

struct v2f_full
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	half3 worldViewDir : TEXCOORD1;
	half3 tsBase0 : TEXCOORD2;
	half3 tsBase1 : TEXCOORD3;
	half3 tsBase2 : TEXCOORD4;	
	half2 uvLM : TEXCOORD5;
};
	
#include "AngryInclude.cginc"		

sampler2D _MainTex;
samplerCUBE _Cube;
sampler2D _2DReflect;
sampler2D _Normal;	
			
ENDCG 


SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 300 
	
	Pass {
		CGPROGRAM
		
		half4 _MainTex_ST;
		// half4 unity_LightmapST;
		// sampler2D unity_Lightmap;		
				
		v2f_full vert (appdata_full v) 
		{
			v2f_full o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			
			o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			
			o.worldViewDir = normalize(WorldSpaceViewDir(v.vertex));
			
			WriteTangentSpaceData(v, o.tsBase0, o.tsBase1, o.tsBase2);	
				
			return o; 
		}
		
		
		fixed4 frag (v2f_full i) : COLOR0 
		{
			half3 nrml = UnpackNormal(tex2D(_Normal, i.uv.xy));
			half3 bumpedNormal = half3(dot(i.tsBase0,nrml), dot(i.tsBase1,nrml), dot(i.tsBase2,nrml));
			
			half3 reflectVector = reflect(normalize(-i.worldViewDir.xyz), normalize(bumpedNormal.xyz));
						
			half4 refl = texCUBE(_Cube, (reflectVector)); 
			
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
			
			tex += refl * tex.a;	
			
			fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));
			tex.rgb *= lm;
							
			return tex;
			
		}		
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
	}
} 

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200 
	
	Pass {
		CGPROGRAM	
		
		half4 _MainTex_ST;
		// half4 unity_LightmapST;
		// sampler2D unity_Lightmap;			
		
		v2f vert (appdata_full v) 
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			o.uv2 = EthansFakeReflection (v.vertex);
				
			return o; 
		}	
		
		fixed4 frag (v2f i) : COLOR0 
		{
			fixed4 tex = tex2D (_MainTex, i.uv);
			
			fixed4 refl = tex2D (_2DReflect, i.uv2);
			tex += refl * tex.a;
			
			#ifdef LIGHTMAP_ON
			fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D (unity_Lightmap, i.uvLM));
			tex.rgb *= lm;			
			#endif
			
			return tex;		
		}				
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	
		ENDCG
	}
} 

FallBack "AngryBots/Fallback"
}
