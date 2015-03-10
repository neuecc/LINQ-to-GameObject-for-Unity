#pragma strict

enum FootType {
	Player,
	Mech,
	Spider
}

var audioSource : AudioSource;
var footType : FootType;

private var physicMaterial : PhysicMaterial;

function OnCollisionEnter (collisionInfo : Collision) {
	physicMaterial = collisionInfo.collider.sharedMaterial;
}

function OnFootstep () {
	if (!audioSource.enabled)
	{
		return;
	}
	
	var sound : AudioClip;
	switch (footType) {
	case FootType.Player:
		//sound = MaterialImpactManager.GetPlayerFootstepSound (physicMaterial);
		break;
	case FootType.Mech:
		//sound = MaterialImpactManager.GetMechFootstepSound (physicMaterial);
		break;
	case FootType.Spider:
		//sound = MaterialImpactManager.GetSpiderFootstepSound (physicMaterial);
		break;
	}	
	audioSource.pitch = Random.Range (0.98, 1.02);
	audioSource.PlayOneShot (sound, Random.Range (0.8, 1.2));
}
