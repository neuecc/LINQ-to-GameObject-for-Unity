#pragma strict

var audioSource : AudioSource;
var sound : AudioClip;

function Awake () {
	if (!audioSource && GetComponent.<AudioSource>())
		audioSource = GetComponent.<AudioSource>();
}

function OnSignal () {
	if (sound)
		audioSource.clip = sound;
	audioSource.Play ();
}