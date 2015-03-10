
#pragma strict 

public var blink : float = 0.0f;

function OnWillRenderObject () {
	GetComponent.<Renderer>().sharedMaterial.SetFloat ("_SelfIllumStrength", blink);	
}

function Blink () {
	blink = 1.0f - blink;
}