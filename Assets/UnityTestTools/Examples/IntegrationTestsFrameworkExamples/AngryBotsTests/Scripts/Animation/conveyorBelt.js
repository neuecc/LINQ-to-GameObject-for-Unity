
#pragma strict

var scrollSpeed : float = 0.1;
var mat : Material;

function Start () {
	enabled = false;
}

function OnBecameVisible () {
	enabled = true;	
}

function OnBecameInvisible () {
	enabled = false;	
}

function Update () {
	var offset : float = (Time.time * scrollSpeed) % 1.0;
	
	mat.SetTextureOffset ("_MainTex", Vector2(0, -offset));
	mat.SetTextureOffset ("_BumpMap", Vector2(0, -offset));
}
