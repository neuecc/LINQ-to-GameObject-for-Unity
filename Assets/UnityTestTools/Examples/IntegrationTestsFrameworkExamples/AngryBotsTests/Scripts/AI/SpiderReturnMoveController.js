#pragma strict

// Public member data
public var motor : MovementMotor;

// Private memeber data
private var ai : AI;

private var character : Transform;
private var spawnPos : Vector3;
public var animationBehaviour : MonoBehaviour;

function Awake () {
	character = motor.transform;
	ai = transform.parent.GetComponentInChildren.<AI> ();
	spawnPos = character.position;
}

function Update () {
	motor.movementDirection = spawnPos - character.position;
	motor.movementDirection.y = 0;
	if (motor.movementDirection.sqrMagnitude > 1)
		motor.movementDirection = motor.movementDirection.normalized;
	
	if (motor.movementDirection.sqrMagnitude < 0.01) {
		character.position = new Vector3 (spawnPos.x, character.position.y, spawnPos.z);
		motor.GetComponent.<Rigidbody>().velocity = Vector3.zero;
		motor.GetComponent.<Rigidbody>().angularVelocity = Vector3.zero;
		motor.movementDirection = Vector3.zero;
		enabled = false;
		animationBehaviour.enabled = false;
	}
}
