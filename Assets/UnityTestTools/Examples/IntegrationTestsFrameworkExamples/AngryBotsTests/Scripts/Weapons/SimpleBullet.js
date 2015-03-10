#pragma strict

var speed : float = 10;
var lifeTime : float = 0.5;
var dist : float = 10000;

private var spawnTime : float = 0.0;
private var tr : Transform;

function OnEnable () {
	tr = transform;
	spawnTime = Time.time;
}

function Update () {
	tr.position += tr.forward * speed * Time.deltaTime;
	dist -= speed * Time.deltaTime;
	if (Time.time > spawnTime + lifeTime || dist < 0) {
		Spawner.Destroy (gameObject);
	}
}
