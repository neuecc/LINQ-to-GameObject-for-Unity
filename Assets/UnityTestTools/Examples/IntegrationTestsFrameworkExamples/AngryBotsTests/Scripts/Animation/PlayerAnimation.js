#pragma strict

class MoveAnimation {
	// The animation clip
	var clip : AnimationClip;
	
	// The velocity of the walk or run cycle in this clip
	var velocity : Vector3;
	
	// Store the current weight of this animation
	@HideInInspector
	public var weight : float;
	
	// Keep track of whether this animation is currently the best match
	@HideInInspector
	public var currentBest : boolean = false;
	
	// The speed and angle is directly derived from the velocity,
	// but since it's slightly expensive to calculate them
	// we do it once in the beginning instead of in every frame.
	@HideInInspector
	public var speed : float;
	@HideInInspector
	public var angle : float;
	
	public function Init () {
		velocity.y = 0;
		speed = velocity.magnitude;
		angle = PlayerAnimation.HorizontalAngle (velocity);
	}
}

var rigid : Rigidbody;
var rootBone : Transform;
var upperBodyBone : Transform;
var maxIdleSpeed : float = 0.5;
var minWalkSpeed : float = 2.0;
var idle : AnimationClip;
var turn : AnimationClip;
var shootAdditive : AnimationClip;
var moveAnimations : MoveAnimation[];
var footstepSignals : SignalSender;

private var tr : Transform;
private var lastPosition : Vector3 = Vector3.zero;
private var velocity : Vector3 = Vector3.zero;
private var localVelocity : Vector3 = Vector3.zero;
private var speed : float = 0;
private var angle : float = 0;
private var lowerBodyDeltaAngle : float = 0;
private var idleWeight : float = 0;
private var lowerBodyForwardTarget : Vector3 = Vector3.forward;
private var lowerBodyForward : Vector3 = Vector3.forward;
private var bestAnimation : MoveAnimation = null;
private var lastFootstepTime : float = 0;
private var lastAnimTime : float = 0;

public var animationComponent : Animation;

function Awake () {
	tr = rigid.transform;
	lastPosition = tr.position;
	
	for (var moveAnimation : MoveAnimation in moveAnimations) {
		moveAnimation.Init ();
		animationComponent[moveAnimation.clip.name].layer = 1;
		animationComponent[moveAnimation.clip.name].enabled = true;
	}
	animationComponent.SyncLayer (1);
	
	animationComponent[idle.name].layer = 2;
	animationComponent[turn.name].layer = 3;
	animationComponent[idle.name].enabled = true;
	
	animationComponent[shootAdditive.name].layer = 4;
	animationComponent[shootAdditive.name].weight = 1;
	animationComponent[shootAdditive.name].speed = 0.6;
	animationComponent[shootAdditive.name].blendMode = AnimationBlendMode.Additive;
	
	//animation[turn.name].enabled = true;
}

function OnStartFire () {
	if (Time.timeScale == 0)
		return;
	
	animationComponent[shootAdditive.name].enabled = true;
}

function OnStopFire () {
	animationComponent[shootAdditive.name].enabled = false;
}

function FixedUpdate () {
	velocity = (tr.position - lastPosition) / Time.deltaTime;
	localVelocity = tr.InverseTransformDirection (velocity);
	localVelocity.y = 0;
	speed = localVelocity.magnitude;
	angle = HorizontalAngle (localVelocity);
	
	lastPosition = tr.position;
}

function Update () {
	idleWeight = Mathf.Lerp (idleWeight, Mathf.InverseLerp (minWalkSpeed, maxIdleSpeed, speed), Time.deltaTime * 10);
	animationComponent[idle.name].weight = idleWeight;
	
	if (speed > 0) {
		var smallestDiff : float = Mathf.Infinity;
		for (var moveAnimation : MoveAnimation in moveAnimations) {
			var angleDiff : float = Mathf.Abs(Mathf.DeltaAngle (angle, moveAnimation.angle));
			var speedDiff : float = Mathf.Abs (speed - moveAnimation.speed);
			var diff : float = angleDiff + speedDiff;
			if (moveAnimation == bestAnimation)
				diff *= 0.9;
			
			if (diff < smallestDiff) {
				bestAnimation = moveAnimation;
				smallestDiff = diff;
			}
		}
		
		animationComponent.CrossFade (bestAnimation.clip.name);
	}
	else {
		bestAnimation = null;
	}
	
	if (lowerBodyForward != lowerBodyForwardTarget && idleWeight >= 0.9)
		animationComponent.CrossFade (turn.name, 0.05);
	
	if (bestAnimation && idleWeight < 0.9) {
		var newAnimTime = Mathf.Repeat (animationComponent[bestAnimation.clip.name].normalizedTime * 2 + 0.1, 1);
		if (newAnimTime < lastAnimTime) {
			if (Time.time > lastFootstepTime + 0.1) {
				footstepSignals.SendSignals (this);
				lastFootstepTime = Time.time;
			}
		}
		lastAnimTime = newAnimTime;
	}
}

function LateUpdate () {
	var idle : float = Mathf.InverseLerp (minWalkSpeed, maxIdleSpeed, speed);
	
	if (idle < 1) {
		// Calculate a weighted average of the animation velocities that are currently used
		var animatedLocalVelocity : Vector3 = Vector3.zero;
		for (var moveAnimation : MoveAnimation in moveAnimations) {
			// Ignore this animation if its weight is 0
			if (animationComponent[moveAnimation.clip.name].weight == 0)
				continue;
			
			// Ignore this animation if its velocity is more than 90 degrees away from current velocity
			if (Vector3.Dot (moveAnimation.velocity, localVelocity) <= 0)
				continue;
			
			// Add velocity of this animation to the weighted average
			animatedLocalVelocity += moveAnimation.velocity * animationComponent[moveAnimation.clip.name].weight;
		}
		
		// Calculate target angle to rotate lower body by in order
		// to make feet run in the direction of the velocity
		var lowerBodyDeltaAngleTarget : float = Mathf.DeltaAngle (
			HorizontalAngle (tr.rotation * animatedLocalVelocity),
			HorizontalAngle (velocity)
		);
		
		// Lerp the angle to smooth it a bit
		lowerBodyDeltaAngle = Mathf.LerpAngle (lowerBodyDeltaAngle, lowerBodyDeltaAngleTarget, Time.deltaTime * 10);
		
		// Update these so they're ready for when we go into idle
		lowerBodyForwardTarget = tr.forward;
		lowerBodyForward = Quaternion.Euler (0, lowerBodyDeltaAngle, 0) * lowerBodyForwardTarget;
	}
	else {
		// Turn the lower body towards it's target direction
		lowerBodyForward = Vector3.RotateTowards (lowerBodyForward, lowerBodyForwardTarget, Time.deltaTime * 520 * Mathf.Deg2Rad, 1);
		
		// Calculate delta angle to make the lower body stay in place
		lowerBodyDeltaAngle = Mathf.DeltaAngle (
			HorizontalAngle (tr.forward),
			HorizontalAngle (lowerBodyForward)
		);
		
		// If the body is twisted more than 80 degrees,
		// set a new target direction for the lower body, so it begins turning
		if (Mathf.Abs(lowerBodyDeltaAngle) > 80)
			lowerBodyForwardTarget = tr.forward;
	}
	
	// Create a Quaternion rotation from the rotation angle
	var lowerBodyDeltaRotation : Quaternion = Quaternion.Euler (0, lowerBodyDeltaAngle, 0);
	
	// Rotate the whole body by the angle
	rootBone.rotation = lowerBodyDeltaRotation * rootBone.rotation;
	
	// Counter-rotate the upper body so it won't be affected
	upperBodyBone.rotation = Quaternion.Inverse (lowerBodyDeltaRotation) * upperBodyBone.rotation;
	
}

static function HorizontalAngle (direction : Vector3) {
	return Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
}
