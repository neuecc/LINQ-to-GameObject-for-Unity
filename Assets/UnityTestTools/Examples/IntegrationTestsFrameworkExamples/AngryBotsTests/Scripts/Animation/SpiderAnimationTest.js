#pragma strict

#if !UNITY_FLASH

var rigid : Rigidbody;
var forwardAnim : AnimationClip;
var backAnim : AnimationClip;
var leftAnim : AnimationClip;
var rightAnim : AnimationClip;

var walking : float;
var angle : float;

private var tr : Transform;

function OnEnable () {
	tr = rigid.transform;
	
	GetComponent.<Animation>()[forwardAnim.name].layer = 1;
	GetComponent.<Animation>()[forwardAnim.name].enabled = true;
	GetComponent.<Animation>()[backAnim.name].layer = 1;
	GetComponent.<Animation>()[backAnim.name].enabled = true;
	GetComponent.<Animation>()[leftAnim.name].layer = 1;
	GetComponent.<Animation>()[leftAnim.name].enabled = true;
	GetComponent.<Animation>()[rightAnim.name].layer = 1;
	GetComponent.<Animation>()[rightAnim.name].enabled = true;
	GetComponent.<Animation>().SyncLayer (1);
}

function Update () {
	rigid.velocity = Quaternion.Euler(0, angle, 0) * rigid.transform.forward * 2.4 * walking;
	
	var velocity : Vector3 = rigid.velocity;
	velocity.y = 0;
	
	var walkWeight : float = velocity.magnitude / 2.4;
	
	GetComponent.<Animation>()[forwardAnim.name].speed = walkWeight;
	GetComponent.<Animation>()[rightAnim.name].speed = walkWeight;
	GetComponent.<Animation>()[backAnim.name].speed = walkWeight;
	GetComponent.<Animation>()[leftAnim.name].speed = walkWeight;
	
	if (velocity == Vector3.zero) {
		return;
	}
	
	var angle : float = Mathf.DeltaAngle (
		HorizontalAngle (tr.forward),
		HorizontalAngle (rigid.velocity)
	);
	
	var w : float;
	if (angle < -90) {
		w = Mathf.InverseLerp (-180, -90, angle);
		GetComponent.<Animation>()[forwardAnim.name].weight = 0;
		GetComponent.<Animation>()[rightAnim.name].weight = 0;
		GetComponent.<Animation>()[backAnim.name].weight = 1 - w;
		GetComponent.<Animation>()[leftAnim.name].weight = 1;
	}
	else if (angle < 0) {
		w = Mathf.InverseLerp (-90, 0, angle);
		GetComponent.<Animation>()[forwardAnim.name].weight = w;
		GetComponent.<Animation>()[rightAnim.name].weight = 0;
		GetComponent.<Animation>()[backAnim.name].weight = 0;
		GetComponent.<Animation>()[leftAnim.name].weight = 1 - w;
	}
	else if (angle < 90) {
		w = Mathf.InverseLerp (0, 90, angle);
		GetComponent.<Animation>()[forwardAnim.name].weight = 1 - w;
		GetComponent.<Animation>()[rightAnim.name].weight = w;
		GetComponent.<Animation>()[backAnim.name].weight = 0;
		GetComponent.<Animation>()[leftAnim.name].weight = 0;
	}
	else {
		w = Mathf.InverseLerp (90, 180, angle);
		GetComponent.<Animation>()[forwardAnim.name].weight = 0;
		GetComponent.<Animation>()[rightAnim.name].weight = 1 - w;
		GetComponent.<Animation>()[backAnim.name].weight = w;
		GetComponent.<Animation>()[leftAnim.name].weight = 0;
	}
}

static function HorizontalAngle (direction : Vector3) {
	return Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
}

function OnGUI () {
	GUILayout.Label ("Angle (0 to 360): "+angle.ToString("0.00"));
	angle = GUILayout.HorizontalSlider (angle, 0, 360, GUILayout.Width (200));
	for (var i : int = 0; i<=360; i+=45) {
		if (Mathf.Abs (angle - i) < 10)
			angle = i;
	}
	
	GUILayout.Label ("Walking (0 to 1): "+walking.ToString("0.00"));
	walking = GUILayout.HorizontalSlider (walking, 0, 1, GUILayout.Width (100));
}
#endif