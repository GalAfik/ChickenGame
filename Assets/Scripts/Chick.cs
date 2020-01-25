﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chick : Follower
{
	public float speedRelativeToPlayer = 2f;
	private Animator animator; // Handles animations

	private void Start()
	{
		// Get animator object to set variables for animations
		animator = GetComponent<Animator>();

		// Overwrite speed with player spped + variable speed
		Player playerObject = FindObjectOfType<Player>();
		if (playerObject != null)
		{
			speed = playerObject.maxForwardSpeed + speedRelativeToPlayer;
		}
	}

	private void LateUpdate()
	{
		if (isViewing && !isFollowing) animator.SetBool("Jump", true);
		else animator.SetBool("Jump", false);
	}

	// Destroy this object when it collides with the player object
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == objectFollowing.name)
		{
			// Increment the number of chicks collected within the Master object
			Master.chicksCollected++;
			// Destroy this object
			Destroy(gameObject);
		}
	}
}
