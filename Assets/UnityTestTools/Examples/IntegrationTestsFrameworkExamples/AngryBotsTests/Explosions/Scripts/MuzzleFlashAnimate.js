
#pragma strict

function Update () {
	transform.localScale = Vector3.one * Random.Range(0.5,1.5);
	transform.localEulerAngles.z = Random.Range(0,90.0);
}