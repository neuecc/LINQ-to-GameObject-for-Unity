// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D



Shader "AngryBots/Fallback" {
	
Properties {
	_MainTex ("Base", 2D) = "white" {}
}


CGINCLUDE		

struct v2f 
{
	half4 pos : SV_POSITION;
	half2 uv : TEXCOORD0;
	half2 uv2 : TEXCOORD1;
};
	
#include "AngryInclude.cginc"

sampler2D _MainTex;
						
ENDCG 

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 140
	
	Pass {
		CGPROGRAM
		
		// half4 unity_LightmapST;
		// sampler2D unity_Lightmap;
		half4 _MainTex_ST;		
				
		v2f vert (appdata_full v) 
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.uv2 = v.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
			return o; 
		}		
		
		fixed4 frag (v2f i) : COLOR0 
		{
			fixed4 tex = tex2D (_MainTex, i.uv);	
			#ifdef LIGHTMAP_ON
			fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv2));
			tex.rgb *= lm;	
			#else
			tex.rgb *= 0.65;		
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

FallBack Off
}

