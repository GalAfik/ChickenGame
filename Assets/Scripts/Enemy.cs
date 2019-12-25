using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Follower
{
	public float health;

	private void OnTriggerEnter(Collider other)
	{
		// Decrease health when hit by an egg object or other projectile
		if (other.gameObject.tag == "Egg")
		{
			Destroy(other.gameObject);
			health--;
		}

		// Destroy object once it reaches zero health
		if (health <= 0) Destroy(gameObject);
	}
}
