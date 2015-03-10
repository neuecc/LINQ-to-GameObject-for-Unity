#pragma strict
#pragma downcast

var checkpoint : Transform;

function OnSignal () {
	transform.position = checkpoint.position;
	transform.rotation = checkpoint.rotation;
	
	ResetHealthOnAll ();
}

static function ResetHealthOnAll () {
	var healthObjects : Health[] = FindObjectsOfType (Health);
	for (var health : Health in healthObjects) {
		health.dead = false;
		health.health = health.maxHealth;
	}
}
