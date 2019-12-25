﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float maxForwardSpeed = 4f;
	public float maxSidewaysSpeed = 4f;
	public float minDriftSpeed = 3f;
	public float acceleration = 10f; // How fast the player character should start moving
	public float deceleration = 4f; // How fast the player character should stop when not moving
	public float gravity = 20f; // How fast the player should fall to the ground when not grounded
	public float glidingGravity = 3f; // How fast the player should fall when gliding
	public float jumpSpeed = 5f; // How much force to apply to the player when they jump
	public float shootDistance = 5f; // How far the shot object should be shot
	public GameObject projectileObject; // The game object being used as a projectile for the Shoot function

	protected Vector3 moveVector; // The direction the player should move in
	private float verticalVelocity;
	private float minMoveDistance = .05f; // How far the mouse must be from the player before it will start moving
	private Vector3 mousePositionInWorld; // Mouse screen position transposed onto the game world
	private CharacterController controller;
	private Vector3 lastGroundedPosition;

	private Animator animator; // Handles animations
	private float runAnimSpeed = .5f; // How fast the player has to be moving to start the running animation

	// Power-ups
	private bool canJump = true;
	private bool canShoot = true;
	private bool canGlide = true;
	private bool canDash = true;
	private bool canSwim = true;
	private bool canBawk = true;

	// Runs when the object is first created, before the first step
	private void Start()
	{
		controller = GetComponent<CharacterController>();

		// Zero out vectors and positions
		mousePositionInWorld = Vector3.zero;
		moveVector = Vector3.zero;

		// Get animator object to set variables for animations
		animator = GetComponent<Animator>();
		// Set running animation speed to half of max speed
		runAnimSpeed *= maxForwardSpeed;

		// Set Cursor to not be visible
		Cursor.visible = false;

		// Start with drift effect off
		GetComponent<ParticleSystem>().Pause();
	}

	// Update is called once per frame
	void Update()
	{
		// Reset animation variables
		animator.SetBool("Run", false);
		animator.SetBool("Walk", false);

		// Figure out the ground plane
		// Create an invisible plane, centered on the player object, parallel to the "ground" to intersect the mouse ray with
		RaycastHit objectUnderPlayer;
		Plane plane;
		float distanceOfGroundUnderPlayer = 0.1f;
		if (Physics.Raycast(transform.position, Vector3.down, out objectUnderPlayer, distanceOfGroundUnderPlayer))
			plane = new Plane(objectUnderPlayer.normal, transform.position);
		else plane = new Plane(Vector3.up, transform.position);// Hovering above the ground

		// Raycast out to the plane to determine where the mouse is in the world
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float ent = 100.0f;
		if (plane.Raycast(ray, out ent)) mousePositionInWorld = ray.GetPoint(ent);
		Vector3 lookVector = mousePositionInWorld - transform.position; // Vector pointing from the player to the mouse's position

		// Make sure the mouse is at least a small distance from the player to start moving and looking -- reduces stuttering
		if (lookVector.magnitude >= minMoveDistance)
		{
			// Rotate to look at the mouse
			transform.LookAt(mousePositionInWorld); //transform.forward = lookVector;
			// Handle forward acceleration
			if (!Input.GetMouseButton(1))
				moveVector += lookVector.normalized * acceleration * Time.deltaTime; // Accelerate
			else
				moveVector -= moveVector.normalized * deceleration * Time.deltaTime; // Slow down using deceleration
		}

		// Ensure that velocity does not excede max speed
		float forwardVelocity = Mathf.Abs(Vector3.Dot(moveVector, transform.forward)); // Has to account only for hoizontal velocity and ignore vertical vel.
		float sidewaysVelocity = Mathf.Abs(Vector3.Dot(moveVector, transform.right));
		if (forwardVelocity > maxForwardSpeed) moveVector = moveVector.normalized * maxForwardSpeed;
		else if (forwardVelocity < 0) moveVector = Vector3.zero; // This should never happen, but may due to bad calculation and needs to be fixed
		if (sidewaysVelocity > maxSidewaysSpeed) moveVector = moveVector.normalized * maxSidewaysSpeed;
		else if (sidewaysVelocity < 0) moveVector = Vector3.zero; // This should never happen, but may due to bad calculation and needs to be fixed

		// Handle drifting
		if (sidewaysVelocity >= minDriftSpeed && controller.isGrounded) GetComponent<ParticleSystem>().Play();
		else GetComponent<ParticleSystem>().Pause();

		// Handle gravity and hovering
		if (controller.isGrounded)
		{
			lastGroundedPosition = transform.position;
			verticalVelocity = -gravity * Time.deltaTime;
			if (canJump && Input.GetButtonDown("Jump")) verticalVelocity = jumpSpeed; // Handle Jumping
		}
		else
		{
			// Handle gliding - only when moving toward the ground and holding the jump button
			if (canGlide && Input.GetButton("Jump") && moveVector.y < 0) verticalVelocity -= glidingGravity * Time.deltaTime;
			else verticalVelocity -= gravity * Time.deltaTime;
		}

		// Finally, apply the moveVector to the player controller
		moveVector.y = verticalVelocity;
		controller.Move(moveVector * Time.deltaTime);

		// Check for click input to handle egg shooting mechanic
		if (canShoot && Input.GetMouseButtonDown(0)) Shoot(lookVector);

		// Animation variables
		if (forwardVelocity >= runAnimSpeed || !controller.isGrounded) animator.SetBool("Run", true);
		else if (forwardVelocity > 0) animator.SetBool("Walk", true);
		else
		{
			animator.SetBool("Run", false);
			animator.SetBool("Walk", false);
		}

		// Handle the player falling off the screen
		if (transform.position.y <= -10) transform.position = lastGroundedPosition;

		/* DEBUG SECTION */
		//Debug.Log(moveVector);
		Debug.DrawRay(transform.position, transform.up.normalized, Color.green);
		Debug.DrawRay(transform.position, transform.forward.normalized, Color.blue);
		Debug.DrawRay(transform.position, transform.right.normalized, Color.red);
	}

	// Instantiate a shootable game object and invoke its Shoot() function
	void Shoot(Vector3 lookVector)
	{
		// Figure out the angle the object should travel in
		Vector3 shootVector = -lookVector;
		shootVector = shootVector + transform.up;
		// Figure out where the object should be spawned
		Vector3 spawnPosition = transform.position - lookVector.normalized / 4;
		spawnPosition.y += 0.2f;

		// Shoot an object in the shootVector direction
		Shootable projectileInstance = Instantiate(projectileObject, spawnPosition, Quaternion.identity).GetComponent<Shootable>();
		projectileInstance.Shoot(shootVector, shootDistance);
	}
}
