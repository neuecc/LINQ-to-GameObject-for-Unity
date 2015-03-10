
#pragma strict

var playerTransform : Transform;
private var pos : Vector3;
private var scale : Vector3;
var minGlow : float = 0.2f;
var maxGlow : float = 0.5f;
var glowColor : Color = Color.white;

private var mat : Material;

function Start () {
	if (!playerTransform)
		playerTransform = GameObject.FindWithTag ("Player").transform;	
	pos = transform.position;
	scale = transform.localScale;
	mat = GetComponent.<Renderer>().material;
	enabled = false;
}

function OnDrawGizmos () {
	Gizmos.color = glowColor;
	Gizmos.color.a = maxGlow * 0.25f;	
	Gizmos.matrix = transform.localToWorldMatrix;
	var scale : Vector3 = 5.0f * Vector3.Scale (Vector3.one, Vector3(1,0,1));
	Gizmos.DrawCube (Vector3.zero, scale);
	Gizmos.matrix = Matrix4x4.identity;
}

function OnDrawGizmosSelected () {
	Gizmos.color = glowColor;
	Gizmos.color.a = maxGlow;	
	Gizmos.matrix = transform.localToWorldMatrix;
	var scale : Vector3 = 5.0f * Vector3.Scale (Vector3.one, Vector3(1,0,1));
	Gizmos.DrawCube (Vector3.zero, scale);
	Gizmos.matrix = Matrix4x4.identity;
}

function OnBecameVisible () {
	enabled = true;	
}

function OnBecameInvisible () {
	enabled = false;
}

function Update () {
	var vec : Vector3 = (pos - playerTransform.position);
	vec.y = 0.0f;
	var distance = vec.magnitude;	
	transform.localScale = Vector3.Lerp (Vector3.one * minGlow, scale, Mathf.Clamp01 (distance * 0.35f));
	mat.SetColor ("_TintColor",  glowColor * Mathf.Clamp (distance * 0.1f, minGlow, maxGlow));	
}