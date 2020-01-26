using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public float maxForwardSpeed = 3f;
	public float maxSidewaysSpeed = 3f;
	public float minDriftSpeed = 2f;

	// Dash variables
	public float maxDashTime = 0.2f;
	public float dashCooldownTime = 1f;
	public float dashSpeedMultiplier = 4f;
	private Vector3 dashDirection;

	// SuperMode variables
	public float maxSuperModeTime = 4f; // How much time the player remains in superMode when picking up a Corn object
	public float superMultipliler = 1.5f; // All speed-related values will be multiplied by this when super mode is enabled

	// Interact variables
	public float interactRadius = 2f; // How close does an NPC have to be to interact with the player
	public float interactAngle = 90f;

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

	private Animator animator; // Handles animations
	private float runAnimSpeed = .5f; // How fast the player has to be moving to start the running animation
	private new Renderer renderer;
	private ParticleSystem dustParticleSystem;
	private ParticleSystem chicksParticleSystem;
	private ParticleSystem superModeParticleSystem;

	private Vector3 lastCheckpoint; // The latest checkpoint activated by the player
	private Vector3 lastGroundedPosition; // Saves the player's position every second as long as they're on the ground
	public float positionUpdateTime = .5f; // How much time to elapse between each saving of the player's position

	// Power-ups
	private bool canJump = true;
	private bool canShoot = true;
	private bool canGlide = true;
	private bool canDash = true;

	// Timers
	Timer dashTimer;
	Timer dashCooldownTimer;
	Timer superModeTimer;
	Timer positionUpdateTimer;


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
		renderer = GetComponent<SkinnedMeshRenderer>();
		dustParticleSystem = GetComponent<ParticleSystem>();
		chicksParticleSystem = transform.Find("ChickCollectionParticleSystem").GetComponent<ParticleSystem>();
		superModeParticleSystem = transform.Find("SuperModeParticleSystem").GetComponent<ParticleSystem>();
		SetParticleSystem(dustParticleSystem, false);
		SetParticleSystem(chicksParticleSystem, false);
		SetParticleSystem(superModeParticleSystem, false);

		// Zero out dash timer
		dashTimer = new Timer(maxDashTime, false);
		dashCooldownTimer = new Timer(dashCooldownTime, false);
		// Zero out superMode timer
		superModeTimer = new Timer(maxSuperModeTime, false);
		// Zero out grounded position timer
		positionUpdateTimer = new Timer(positionUpdateTime, true);
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
			if (Master.playerHasControl) transform.LookAt(mousePositionInWorld);
			// Handle forward acceleration
			if (!Input.GetMouseButton(1) && Master.playerHasControl)
				moveVector += lookVector.normalized * acceleration * Time.deltaTime; // Accelerate
			else
				moveVector -= moveVector.normalized * deceleration * Time.deltaTime; // Slow down using deceleration
		}

		// Temp variables for accounting for super mode multiplier
		float tempMaxForwardSpeed = (!superModeTimer.IsElapsed() ? maxForwardSpeed * superMultipliler : maxForwardSpeed);
		float tempMaxSidewaysSpeed = (!superModeTimer.IsElapsed() ? maxSidewaysSpeed * superMultipliler : maxSidewaysSpeed);
		float tempMinDriftSpeed = (!superModeTimer.IsElapsed() ? minDriftSpeed * superMultipliler : minDriftSpeed);

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
		SetParticleSystem(dustParticleSystem, ((sidewaysVelocity >= tempMinDriftSpeed && controller.isGrounded) ? true : false)); // Enable/disable drift effects

		// Handle gravity and hovering
		if (controller.isGrounded)
		{
			// Save the player's position every set amount of time
			if (positionUpdateTimer.IsElapsed())
			{
				lastGroundedPosition = transform.position;
				positionUpdateTimer.Reset(); // Reset the timer to count down again
			}
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
		if (canDash && Input.GetButton("Dash") && controller.isGrounded && dashCooldownTimer.IsElapsed()) // Start Dash
		{
			dashTimer.Reset();
			dashCooldownTimer.Reset();
			dashDirection = transform.forward; // Set the temporary dash direction
			// Emmit a large puff of particles to start off with
			GetComponent<ParticleSystem>().Emit(25);
		}
		// Dash
		if (!dashTimer.IsElapsed())
		{
			if (canJump) canJump = false; // Turn off jumping temporarily
			transform.forward = dashDirection; // Lock the direction the player is looking
			moveVector = dashDirection.normalized * maxForwardSpeed * dashSpeedMultiplier; // Move forward at maxForwardSpeed * dashSpeedMultiplier
			SetParticleSystem(dustParticleSystem, true);
		}
		else
		{
			canJump = true; // Reenable jumping after dash
		}

		// Finally, apply the moveVector to the player controller
		moveVector.y = verticalVelocity;
		controller.Move(moveVector * Time.deltaTime);

		// Check for click input to handle egg shooting mechanic
		if (canShoot && Input.GetMouseButtonDown(0) && Master.playerHasControl) Shoot(transform.forward);

		// Animation variables
		if (forwardVelocity >= runAnimSpeed || sidewaysVelocity >= runAnimSpeed) animator.SetBool("Run", true);
		else if (forwardVelocity > 0.1) animator.SetBool("Walk", true);
		else
		{
			// Reset animation variables
			animator.SetBool("Run", false);
			animator.SetBool("Walk", false);
		}

		// Handle superMode timer and effects
		// Tint the player color RED when in super mode
		if (!superModeTimer.IsElapsed())
		{
			renderer.material.SetColor("_BaseColor", Color.red);
			if (!superModeParticleSystem.emission.enabled) SetParticleSystem(superModeParticleSystem, true);
		}
		else
		{
			// If superMode timer is elapsed, turn off super mode!
			renderer.material.SetColor("_BaseColor", Color.white);
			if (superModeParticleSystem.emission.enabled) SetParticleSystem(superModeParticleSystem, false);
		}

		// Handle the player falling off the screen
		if (transform.position.y <= -10) transform.position = lastGroundedPosition;

		// Handle talking with NPCs
		if (Input.GetButton("Interact") && controller.isGrounded && Master.playerHasControl)
		{
			// Find all NPCs and look for the closest one
			GameObject[] nonPlayerCharacters = GameObject.FindGameObjectsWithTag("NPC");
			foreach (GameObject target in nonPlayerCharacters)
			{
				float distance = Vector3.Distance(target.transform.position, transform.position);
				if (distance < 1)
				{
					// Lose control of the player temporarily
					Master.playerHasControl = false;
					// Look at the NPC
					transform.LookAt(target.transform.position);
					// Initiate conversation with the NPC character
					target.GetComponent<NonPlayerCharacter>().Speak();
					// Stop the player in place
					moveVector = Vector3.zero;
				}
			}
		}

		/* DEBUG SECTION */
		//Debug.Log(moveVector);
		//Debug.DrawRay(transform.position, transform.up.normalized, Color.green);
		//Debug.DrawRay(transform.position, transform.forward.normalized, Color.blue);
		//Debug.DrawRay(transform.position, transform.right.normalized, Color.red);

		// When the Quick-Load button is pressed, teleport to the latest checkpoint
		if (Input.GetButton("Debug Reset") && lastCheckpoint != Vector3.zero)
		{
			Respawn();
		}

		// Update all timers
		dashTimer.Update();
		dashCooldownTimer.Update();
		superModeTimer.Update();
		positionUpdateTimer.Update();
	}

	// Respawn at the latest reached checkpoint
	private void Respawn()
	{
		transform.position = lastCheckpoint;
	}

	// Respawn at the specified location
	private void Respawn(Vector3 respawnLocation)
	{
		transform.position = respawnLocation;
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

	// Handle collisions and triggers
	private void OnTriggerEnter(Collider other)
	{
		// Handle picking up a corn object
		if (other.gameObject.tag == "Corn")
		{
			Destroy(other.gameObject);
			superModeTimer.Reset(); // Turn on super mode
		}
		else if (other.gameObject.tag == "Respawn")
		{
			lastCheckpoint = other.transform.position;
		}
		else if (other.gameObject.tag == "Chick")
		{
			SetParticleSystem(chicksParticleSystem, true);
			chicksParticleSystem.Emit(50);
		}
		// If the player touches water, they should respawn at the last grounded position
		else if (other.gameObject.tag == "Water")
		{
			Respawn(lastGroundedPosition);
		}
	}

	private void SetParticleSystem(ParticleSystem particleSystem, bool enabled)
	{
		ParticleSystem.EmissionModule em = particleSystem.emission;
		em.enabled = enabled;
	}
}
