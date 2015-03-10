#pragma strict 

@script RequireComponent (PerFrameRaycast)

public var scrollSpeed : float = 0.5;
public var pulseSpeed : float = 1.5;

public var noiseSize : float = 1.0;

public var maxWidth : float = 0.5;
public var minWidth : float = 0.2;

public var pointer : GameObject = null;

private var lRenderer : LineRenderer;
private var aniTime : float = 0.0;
private var aniDir : float = 1.0;

private var raycast : PerFrameRaycast;

function Start() {
	lRenderer = gameObject.GetComponent (LineRenderer) as LineRenderer;	
	aniTime = 0.0;
	
	// Change some animation values here and there
	ChoseNewAnimationTargetCoroutine();
	
	raycast = GetComponent.<PerFrameRaycast> ();
}

function ChoseNewAnimationTargetCoroutine () {
	while (true) {
		aniDir = aniDir * 0.9 + Random.Range (0.5, 1.5) * 0.1;
		yield;
		minWidth = minWidth * 0.8 + Random.Range (0.1, 1.0) * 0.2;
		yield WaitForSeconds (1.0 + Random.value * 2.0 - 1.0);	
	}	
}

function Update () {
	GetComponent.<Renderer>().material.mainTextureOffset.x += Time.deltaTime * aniDir * scrollSpeed;
	GetComponent.<Renderer>().material.SetTextureOffset ("_NoiseTex", Vector2 (-Time.time * aniDir * scrollSpeed, 0.0));

	var aniFactor : float = Mathf.PingPong (Time.time * pulseSpeed, 1.0);
	aniFactor = Mathf.Max (minWidth, aniFactor) * maxWidth;
	lRenderer.SetWidth (aniFactor, aniFactor);
	
	// Cast a ray to find out the end point of the laser
	var hitInfo : RaycastHit = raycast.GetHitInfo ();
	if (hitInfo.transform) {
		lRenderer.SetPosition (1, (hitInfo.distance * Vector3.forward));
		GetComponent.<Renderer>().material.mainTextureScale.x = 0.1 * (hitInfo.distance);
		GetComponent.<Renderer>().material.SetTextureScale ("_NoiseTex", Vector2 (0.1 * hitInfo.distance * noiseSize, noiseSize));		
		
		// Use point and normal to align a nice & rough hit plane
		if (pointer) {
			pointer.GetComponent.<Renderer>().enabled = true;
			pointer.transform.position = hitInfo.point + (transform.position - hitInfo.point) * 0.01;
			pointer.transform.rotation = Quaternion.LookRotation (hitInfo.normal, transform.up);
			pointer.transform.eulerAngles.x = 90.0;
		}
	}
	else {
		if (pointer)
			pointer.GetComponent.<Renderer>().enabled = false;		
		var maxDist : float = 200.0;
		lRenderer.SetPosition (1, (maxDist * Vector3.forward));
		GetComponent.<Renderer>().material.mainTextureScale.x = 0.1 * (maxDist);		
		GetComponent.<Renderer>().material.SetTextureScale ("_NoiseTex", Vector2 (0.1 * (maxDist) * noiseSize, noiseSize));		
	}
}
