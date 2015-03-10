
#pragma strict

public var playerHealth : Health;
public var healthMaterial : Material;

private var healthBlink : float = 1.0f;
private var oneOverMaxHealth : float = 0.5f;

function Start () {
	oneOverMaxHealth = 1.0f / playerHealth.maxHealth;	
}

function Update () {
	var relativeHealth : float = playerHealth.health * oneOverMaxHealth;
	healthMaterial.SetFloat ("_SelfIllumination", relativeHealth * 2.0f * healthBlink);
	
	if (relativeHealth < 0.45f) 
		healthBlink = Mathf.PingPong (Time.time * 6.0f, 2.0f);
	else 
		healthBlink = 1.0f;
}