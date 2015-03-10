#pragma strict

#if !UNITY_FLASH

var turning : float = 0;
var walking : float = 0;
var turnOffset : float = 0.0;

var rigid : Rigidbody;
var idle : AnimationClip;
var walk : AnimationClip;
var turnLeft : AnimationClip;
var turnRight : AnimationClip;
var footstepSignals : SignalSender;

function OnEnable () {
	
	GetComponent.<Animation>()[idle.name].layer = 0;
	GetComponent.<Animation>()[idle.name].weight = 1;
	GetComponent.<Animation>()[idle.name].enabled = true;
	
	GetComponent.<Animation>()[walk.name].layer = 1;
	GetComponent.<Animation>()[turnLeft.name].layer = 1;
	GetComponent.<Animation>()[turnRight.name].layer = 1;
	
	GetComponent.<Animation>()[walk.name].weight = 1;
	GetComponent.<Animation>()[turnLeft.name].weight = 0;
	GetComponent.<Animation>()[turnRight.name].weight = 0;
	
	GetComponent.<Animation>()[walk.name].enabled = true;
	GetComponent.<Animation>()[turnLeft.name].enabled = true;
	GetComponent.<Animation>()[turnRight.name].enabled = true;
	
	//animation[walk.name].speed = 0.93;
	
	//animation.Play ();
}

function FixedUpdate () {
	GetComponent.<Animation>()[walk.name].speed = Mathf.Lerp (1, GetComponent.<Animation>()[walk.name].length / GetComponent.<Animation>()[turnLeft.name].length, Mathf.Abs (turning));
	
	GetComponent.<Animation>()[turnLeft.name].time = GetComponent.<Animation>()[walk.name].time + turnOffset;
	GetComponent.<Animation>()[turnRight.name].time = GetComponent.<Animation>()[walk.name].time + turnOffset;
	
	rigid.velocity = rigid.transform.forward * 2.5 * walking;
	rigid.angularVelocity = Vector3.up * turning * 100 * Mathf.Deg2Rad;
	
	var turningWeight : float = rigid.angularVelocity.y * Mathf.Rad2Deg / 100.0;
	var forwardWeight : float = rigid.velocity.magnitude / 2.5;
	
	GetComponent.<Animation>()[turnLeft.name].weight = Mathf.Clamp01 (-turningWeight);
	GetComponent.<Animation>()[turnRight.name].weight = Mathf.Clamp01 (turningWeight);
	GetComponent.<Animation>()[walk.name].weight = Mathf.Clamp01 (forwardWeight);
}

function OnGUI () {
	GUILayout.Label ("Walking (0 to 1): "+walking.ToString("0.00"));
	walking = GUILayout.HorizontalSlider (walking, 0, 1, GUILayout.Width (100));
	if (GUI.changed) {
		turning = Mathf.Clamp (Mathf.Abs (turning), 0, 1 - walking) * Mathf.Sign (turning);
		GUI.changed = false;
	}
	
	GUILayout.Label ("Turning (-1 to 1): "+turning.ToString("0.00"));
	turning = GUILayout.HorizontalSlider (turning, -1, 1, GUILayout.Width (100));
	if (Mathf.Abs (turning) < 0.1)
		turning = 0;
	if (GUI.changed) {
		walking = Mathf.Clamp (walking, 0, 1 - Mathf.Abs (turning));
		GUI.changed = false;
	}
	
	GUILayout.Label ("Offset to turning anims (-0.5 to 0.5): "+turnOffset.ToString("0.00"));
	turnOffset = GUILayout.HorizontalSlider (turnOffset, -0.5, 0.5, GUILayout.Width (100));
	if (Mathf.Abs (turnOffset) < 0.05)
		turnOffset = 0;
}
#endif