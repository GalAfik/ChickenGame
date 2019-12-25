using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chick : Follower
{
	public float speedRelativeToPlayer = 2f;

	private void Start()
	{
		// Overwrite speed with player spped + variable speed
		Player playerObject = FindObjectOfType<Player>();
		if (playerObject != null)
		{
			speed = playerObject.maxSpeed + speedRelativeToPlayer;
		}
	}

	// Destroy this object when it collides with the player object
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == objectFollowing.name)
		{
			Destroy(gameObject);
		}
	}
}
