#pragma strict

@script RequireComponent (SphereCollider)

private var target : GameObject;
private var sphereCollider : SphereCollider;
private var activeRadius : float;

function Awake () {
	target = transform.parent.gameObject;
	sphereCollider = GetComponent.<SphereCollider> ();
	activeRadius = sphereCollider.radius;

	Disable ();
}

function OnTriggerEnter (other : Collider) {
	if (other.tag == "Player" && target.transform.parent == transform) {
		Enable ();
	}
}

function OnTriggerExit (other : Collider) {
	if (other.tag == "Player") {
		Disable ();
	}
}

function Disable () {
	transform.parent = target.transform.parent;
	target.transform.parent = transform;
	target.SetActive (false);
	sphereCollider.radius = activeRadius;
}

function Enable () {
	target.transform.parent = transform.parent;
	target.SetActive (true);
	transform.parent = target.transform;
	sphereCollider.radius = activeRadius * 1.1;
}
