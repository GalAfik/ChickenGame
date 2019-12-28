using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float maxForwardSpeed = 3f;
	public float maxSidewaysSpeed = 3f;
	public float minDriftSpeed = 2f;
	public float superMultipliler = 1.5f; // All speed-related values will be multiplied by this when super mode is enabled

	// Dash variables
	public float maxDashTime = 0.1f;
	public float dashSpeedMultiplier = 4f;
	public float dashCooldownTime = 1f;
	private float currentDashTime;
	private Vector3 dashDirection;
	private float currentDashCooldownTime;

	public float acceleration = 10f; // How fast the player character should start moving
	public float deceleration = 4f; // How fast the player character should stop when not moving
	public float gravity = 20f; // How fast the player should fall to the ground when not grounded
	public float glidingGravity = 3f; // How fast the player should fall when gliding
	public float jumpSpeed = 5f; // How much force to apply to the player when they jump
	public float shootDistance = 5f; // How far the shot object should be shot
	public GameObject projectileObject; // The game object being used as a projectile for the Shoot function

	private bool hasControl = true; // Does the player have control over the character?
	protected Vector3 moveVector; // The direction the player should move in
	private float verticalVelocity;
	private float minMoveDistance = .05f; // How far the mouse must be from the player before it will start moving
	private Vector3 mousePositionInWorld; // Mouse screen position transposed onto the game world
	private CharacterController controller;
	private Vector3 lastGroundedPosition;

	private Animator animator; // Handles animations
	private float runAnimSpeed = .5f; // How fast the player has to be moving to start the running animation
	private ParticleSystem.EmissionModule emissionModule;

	// Power-ups
	private bool canJump = true;
	private bool canShoot = true;
	private bool canGlide = true;
	private bool canDash = true;
	private bool canSwim = true;
	private bool canBawk = true;
	private bool superMode = false;

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
		emissionModule = GetComponent<ParticleSystem>().emission;
		emissionModule.enabled = true;

		// Zero out dash timer
		currentDashTime = 0;
		currentDashCooldownTime = 0;
	}

	// Update is called once per frame
	void Update()
	{
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
			if (hasControl) transform.LookAt(mousePositionInWorld);
			// Handle forward acceleration
			if (!Input.GetMouseButton(1) && hasControl)
				moveVector += lookVector.normalized * acceleration * Time.deltaTime; // Accelerate
			else
				moveVector -= moveVector.normalized * deceleration * Time.deltaTime; // Slow down using deceleration
		}

		// Temp variables for accounting for super mode multiplier
		float tempMaxForwardSpeed = (superMode ? maxForwardSpeed * superMultipliler : maxForwardSpeed);
		float tempMaxSidewaysSpeed = (superMode ? maxSidewaysSpeed * superMultipliler : maxSidewaysSpeed);
		float tempMinDriftSpeed = (superMode ? minDriftSpeed * superMultipliler : minDriftSpeed);

		// Ensure that velocity does not excede max speed
		float forwardVelocity = Mathf.Abs(Vector3.Dot(moveVector, transform.forward)); // Has to account only for hoizontal velocity and ignore vertical vel.
		float sidewaysVelocity = Mathf.Abs(Vector3.Dot(moveVector, transform.right));
		if (forwardVelocity > tempMaxForwardSpeed)
		{
			forwardVelocity = tempMaxForwardSpeed;
			moveVector = moveVector.normalized * tempMaxForwardSpeed;
		}
		if (forwardVelocity < 0.1) forwardVelocity = 0; // Account for tiny decimal velocity messing with animations 
		if (sidewaysVelocity > tempMaxSidewaysSpeed)
		{
			sidewaysVelocity = tempMaxSidewaysSpeed;
			moveVector = moveVector.normalized * tempMaxSidewaysSpeed;
		}
		if (sidewaysVelocity < 0.1) sidewaysVelocity = 0; // Account for tiny decimal velocity messing with animations 

		// Handle drifting
		emissionModule.enabled = ((sidewaysVelocity >= tempMinDriftSpeed && controller.isGrounded) ? true : false); // Enable/disable drift effects

		// Handle gravity and hovering
		if (controller.isGrounded)
		{
			lastGroundedPosition = transform.position;
			verticalVelocity = -gravity * Time.deltaTime;
			if (canJump && Input.GetButtonDown("Jump") && forwardVelocity > 0.1) verticalVelocity = jumpSpeed; // Handle Jumping - only when moving forward
		}
		else
		{
			// Handle gliding - only when moving toward the ground and holding the jump button
			if (canGlide && Input.GetButton("Jump") && moveVector.y < 0) verticalVelocity -= glidingGravity * Time.deltaTime;
			else verticalVelocity -= gravity * Time.deltaTime;
		}

		// Apply dash multiplier to max speed if the dash button is pressed
		if (canDash && Input.GetButton("Dash") && controller.isGrounded && currentDashCooldownTime <= 0)
		{
			currentDashTime = maxDashTime;
			currentDashCooldownTime = dashCooldownTime;
			dashDirection = transform.forward; // Set the temporary dash direction
		}
		if (currentDashTime > 0) // Dash
		{
			if (canJump) canJump = false; // Turn off jumping temporarily
			currentDashTime -= Time.deltaTime; // Iterate dash timer
			transform.forward = dashDirection; // Lock the direction the player is looking
			moveVector = dashDirection.normalized * maxForwardSpeed * dashSpeedMultiplier; // Move forward at maxForwardSpeed * dashSpeedMultiplier
			emissionModule.enabled = true; // Particle effect while dashing
		}
		if (currentDashCooldownTime > 0) currentDashCooldownTime -= Time.deltaTime; // Cooldown Dash ability

		// Finally, apply the moveVector to the player controller
		moveVector.y = verticalVelocity;
		controller.Move(moveVector * Time.deltaTime);

		// Check for click input to handle egg shooting mechanic
		if (canShoot && Input.GetMouseButtonDown(0)) Shoot(lookVector);

		// Animation variables
		if (forwardVelocity >= runAnimSpeed || sidewaysVelocity >= runAnimSpeed) animator.SetBool("Run", true);
		else if (forwardVelocity > 0.1) animator.SetBool("Walk", true);
		else
		{
			// Reset animation variables
			animator.SetBool("Run", false);
			animator.SetBool("Walk", false);
		}

		// Tint the player color RED when in super mode
		Renderer renderer = gameObject.GetComponent<SkinnedMeshRenderer>();
		if (superMode) renderer.material.color = Color.red;
		else renderer.material.color = Color.white;

		// Handle the player falling off the screen
		if (transform.position.y <= -10) transform.position = lastGroundedPosition;

		/* DEBUG SECTION */
		//Debug.Log(moveVector);
		//Debug.DrawRay(transform.position, transform.up.normalized, Color.green);
		//Debug.DrawRay(transform.position, transform.forward.normalized, Color.blue);
		//Debug.DrawRay(transform.position, transform.right.normalized, Color.red);
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
