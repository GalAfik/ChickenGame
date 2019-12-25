using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float maxSpeed = 3f;
	public float acceleration = 6f; // How fast the player character should start moving
	public float deceleration = 10f; // How fast the player character should stop when not moving
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
		runAnimSpeed *= maxSpeed;

		// Set Cursor to not be visible
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update()
	{
		// Reset animation variables
		animator.SetBool("Run", false);
		animator.SetBool("Walk", false);

		// Create an invisible plane, centered on the player object, parallel to the "ground" to intersect the mouse ray with
		RaycastHit objectUnderPlayer;
		Plane plane;
		float distanceOfGroundUnderPlayer = 0.1f;
		if (Physics.Raycast(transform.position, Vector3.down, out objectUnderPlayer, distanceOfGroundUnderPlayer))
		{
			plane = new Plane(objectUnderPlayer.normal, transform.position);
		}
		else // Hovering above the ground
		{
			plane = new Plane(Vector3.up, transform.position);
		}

		// Raycast out to the plane to determine where the mouse is in the world
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float ent = 100.0f;
		if (plane.Raycast(ray, out ent)) mousePositionInWorld = ray.GetPoint(ent);
		// Vector pointing from the player to the mouse's position
		Vector3 lookVector = mousePositionInWorld - transform.position;

		// Make sure the mouse is at least a small distance from the player to start moving and looking -- reduces stuttering
		if (lookVector.magnitude >= minMoveDistance)
		{
			// Rotate transform to look toward the mouse relative to this object
			transform.forward = lookVector;

			// Handle forward acceleration
			moveVector += lookVector.normalized * acceleration * Time.deltaTime;
		}

		// Freeze chicken in place if right mouse button is held down
		if (Input.GetMouseButton(1))
		{
			// Slow down using deceleration
			moveVector -= moveVector.normalized * deceleration * Time.deltaTime;
		}

		// Ensure that velocity does not excede max speed
		float velocity = Mathf.Abs(Vector3.Dot(moveVector, transform.forward)); // Has to account only for hoizontal velocity and ignore vertical vel.
		if (velocity > maxSpeed) moveVector = moveVector.normalized * maxSpeed;
		else if (velocity < 0) moveVector = Vector3.zero;

		// Handle gravity and hovering
		if (controller.isGrounded)
		{
			lastGroundedPosition = transform.position;
			verticalVelocity = -gravity * Time.deltaTime;
			// Handle Jumping
			if (canJump && Input.GetButtonDown("Jump"))
			{
				verticalVelocity = jumpSpeed;
			}
		}
		else
		{
			// Handle gliding - only when moving toward the ground and holding the jump button
			if (canGlide && Input.GetButton("Jump") && moveVector.y < 0)
			{
				verticalVelocity -= glidingGravity * Time.deltaTime;
			}
			else
			{
				verticalVelocity -= gravity * Time.deltaTime;
			}
		}

		// Finally, apply the moveVector to the player controller
		moveVector.y = verticalVelocity;
		controller.Move(moveVector * Time.deltaTime);

		// Check for click input to handle egg shooting mechanic
		if (canShoot && Input.GetMouseButtonDown(0))
		{
			Shoot(lookVector);
		}

		// Animation variables
		if (velocity >= runAnimSpeed || !controller.isGrounded)
		{
			animator.SetBool("Run", true);
		}
		else if (velocity > 0)
		{
			animator.SetBool("Walk", true);
		}
		else
		{
			animator.SetBool("Run", false);
			animator.SetBool("Walk", false);
		}

		// Handle the player falling off the screen
		if (transform.position.y <= -10)
		{
			transform.position = lastGroundedPosition;
		}

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
