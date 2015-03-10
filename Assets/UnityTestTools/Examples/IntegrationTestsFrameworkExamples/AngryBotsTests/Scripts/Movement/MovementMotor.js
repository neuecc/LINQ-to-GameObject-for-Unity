#pragma strict

/*
This class can be used like an interface.
Inherit from it to define your own movement motor that can control
the movement of characters, enemies, or other entities.
*/

// The direction the character wants to move in, in world space.
// The vector should have a length between 0 and 1.
@HideInInspector
public var movementDirection : Vector3;

// Simpler motors might want to drive movement based on a target purely
@HideInInspector
public var movementTarget : Vector3;

// The direction the character wants to face towards, in world space.
@HideInInspector
public var facingDirection : Vector3;
