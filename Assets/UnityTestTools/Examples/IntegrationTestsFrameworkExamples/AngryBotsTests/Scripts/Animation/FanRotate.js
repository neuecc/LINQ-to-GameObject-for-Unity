
#pragma strict

var thisMesh : Mesh;
var uvs : Vector2[];

#if !UNITY_IPHONE && !UNITY_ANDROID && !UNITY_WP8 && !UNITY_BLACKBERRY

function Start () 
{
    thisMesh = GetComponent(MeshFilter).mesh;
    uvs = thisMesh.uv;
}

function Update()
{
	for (var i : int = 0; i < uvs.length; i++) 
	{
		uvs[i].y = (uvs[i].y + 0.25);
	}
	
	thisMesh.uv = uvs;
}

#endif