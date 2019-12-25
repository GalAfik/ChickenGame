using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
	public float speed = 1.5f;
	public float followRange = 1f;
	public GameObject objectFollowing;

	private bool following = false;

	// Update is called once per frame
	void Update()
	{
		// Point towards the player
		Vector3 followVector = objectFollowing.transform.position - transform.position;
		//Debug.DrawRay(transform.position, transform.forward.normalized * 0.5f, Color.red);
		transform.forward = followVector;

		// Only move if the player is within a certain range
		if (followVector.magnitude <= followRange && !following) following = true;

		// Walk forward at a constant speed
		if (following) transform.position += followVector.normalized * speed * Time.deltaTime;
	}
}
