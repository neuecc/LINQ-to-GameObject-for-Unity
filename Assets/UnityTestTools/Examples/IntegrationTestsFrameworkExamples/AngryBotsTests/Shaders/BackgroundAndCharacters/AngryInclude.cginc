// Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'


#ifndef ANGRY_CG_INCLUDED
#define ANGRY_CG_INCLUDED

#include "UnityCG.cginc"

void WriteTangentSpaceData (appdata_full v, out half3 ts0, out half3 ts1, out half3 ts2) {
	TANGENT_SPACE_ROTATION;
	ts0 = mul(rotation, _Object2World[0].xyz * 1.0);
	ts1 = mul(rotation, _Object2World[1].xyz * 1.0);
	ts2 = mul(rotation, _Object2World[2].xyz * 1.0);				
}

half2 EthansFakeReflection (half4 vtx) {
	half3 worldSpace = mul(_Object2World, vtx).xyz;
	worldSpace = (-_WorldSpaceCameraPos * 0.6 + worldSpace) * 0.07;
	return worldSpace.xz;
}

#endif