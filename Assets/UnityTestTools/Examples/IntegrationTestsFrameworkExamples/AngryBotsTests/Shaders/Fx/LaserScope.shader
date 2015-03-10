
Shader "AngryBots/FX/LaserScope" {
    Properties {
        _MainTex ("MainTex", 2D) = "white"
        _NoiseTex ("NoiseTex", 2D) = "white" 
    }
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _NoiseTex;
		
		half4 _MainTex_ST;
		half4 _NoiseTex_ST;
		
		fixed4 _TintColor;
						
		struct v2f {
			half4 pos : SV_POSITION;
			half4 uv : TEXCOORD0;
		};

		v2f vert(appdata_full v)
		{
			v2f o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.uv.zw = TRANSFORM_TEX(v.texcoord, _NoiseTex);
					
			return o; 
		}
		
		fixed4 frag( v2f i ) : COLOR
		{	
			return tex2D (_MainTex, i.uv.xy) * tex2D (_NoiseTex, i.uv.zw);
		}
	
	ENDCG
	
	SubShader {
		Tags { "RenderType" = "Transparent" "Reflection" = "LaserScope" "Queue" = "Transparent"}
		Cull Off
		ZWrite Off
		Blend SrcAlpha One
		
	Pass {
	
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}
				
	} 
	FallBack Off
}
