#pragma strict

var objectToDestroy : GameObject;

function OnSignal () {
	Spawner.Destroy (objectToDestroy);
}
