// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D


/* 

(realtime & planar) reflection shader.

handles simple planar (y is up) bump displacement of planar reflections.

*/

Shader "AngryBots/PlanarRealtimeReflection" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_ReflectionTex ("Internal reflection", 2D) = "black" {}
		_CubeReflTex ("Cube", CUBE) = "black" {}
		_Normals ("Normal", 2D) = "bump" {}
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _ReflectionTex;
		sampler2D _Normals;
		samplerCUBE _CubeReflTex;
		
		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;	
			half4 scr : TEXCOORD1;	
			half2 uvLM : TEXCOORD2;
		};

		struct v2f_full {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;	
			half4 scr : TEXCOORD1;	
			half3 tsBase0 : TEXCOORD2;
			half3 tsBase1 : TEXCOORD3;
			half3 tsBase2 : TEXCOORD4;	
			half3 viewDir : TEXCOORD5;
			half2 uvLM : TEXCOORD6;
		};
	
	ENDCG

	SubShader {
		LOD 400

		Tags { "RenderType"="Opaque" }
		Fog { Mode Off }

		Pass {
			
		CGPROGRAM
		
		#include "AngryInclude.cginc"
		
		uniform half4 _MainTex_ST;
		// half4 unity_LightmapST;	
		// sampler2D unity_Lightmap;		
		
		v2f_full vert(appdata_full v)
		{
			v2f_full o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			
			o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
			
			o.scr = ComputeScreenPos(o.pos);
			
			o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			
			WriteTangentSpaceData(v, o.tsBase0,o.tsBase1,o.tsBase2);	
			o.viewDir = normalize(WorldSpaceViewDir(v.vertex));					
			
			return o; 
		}
		
		half4 frag( v2f_full i ) : COLOR
		{	
			half3 normals = UnpackNormal(tex2D(_Normals, i.uv.xy));
			half3 bumpedNormal = half3(dot(i.tsBase0,normals), dot(i.tsBase1,normals), dot(i.tsBase2,normals));			
			
			half3 reflectVector = reflect(-i.viewDir.xyz, bumpedNormal.xyz);
			
			half4 color = tex2D(_MainTex, i.uv);
			i.scr = i.scr/i.scr.w;

			fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));
			color.rgb *= lm;
				
			i.scr.xy += normals.xy;
			return color + tex2D(_ReflectionTex, i.scr.xy) + texCUBE(_CubeReflTex, reflectVector) * 0.1;
		}		
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
				
		ENDCG
		 
		}		
	}
	
	SubShader {
		LOD 200

		Tags { "RenderType"="Opaque" }
		Fog { Mode Off }

		Pass {
						
		CGPROGRAM
		
		uniform half4 _MainTex_ST;
		// half4 unity_LightmapST;	
		// sampler2D unity_Lightmap;		
		
		v2f vert(appdata_full v)
		{
			v2f o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

			o.scr = ComputeScreenPos(o.pos);
			
			return o; 
		}
		
		fixed4 frag( v2f i ) : COLOR
		{	
			fixed4 color = tex2D(_MainTex, i.uv);
			
			fixed3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM));
			color.rgb *= lm;

			half2 screen = (i.scr.xy / i.scr.w);
						
			return color + tex2D(_ReflectionTex, screen) * color.a;
		}		
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
				
		ENDCG
		 
		}		
	} 
	
	FallBack "AngryBots/Fallback"
}
