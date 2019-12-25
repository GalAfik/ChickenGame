using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
	public float speed = 1.5f;
	public float followRange = 1f;
	public float viewRange = 1f;
	public GameObject objectFollowing;

	protected bool isFollowing = false;
	protected bool isViewing = false;

	// Update is called once per frame
	void Update()
	{
		// Point towards the player
		Vector3 followVector = objectFollowing.transform.position - transform.position;
		//Debug.DrawRay(transform.position, transform.forward.normalized * 0.5f, Color.red);
		transform.forward = followVector;

		// Animate if player is within a certain range
		if (followVector.magnitude <= viewRange) isViewing = true;
		else isViewing = false;

		// Only move if the player is within a certain range
		if (followVector.magnitude <= followRange) isFollowing = true;

		// Walk forward at a constant speed
		if (isFollowing) transform.position += followVector.normalized * speed * Time.deltaTime;
	}
}
