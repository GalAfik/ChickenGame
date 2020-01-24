using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour, Shootable
{
	public float maxDistanceFromPlayer = 30f;
	public GameObject eggSplatterObject;

	private GameObject playerObject;

	void Awake()
    {
		playerObject = GameObject.FindWithTag("Player");
		Physics.IgnoreCollision(playerObject.GetComponent<Collider>(), GetComponent<Collider>()); // Do not collide with the player object!
	}

	void Update()
	{
		// Destroy this object if it gets too far away from the player object
		if (Vector3.Distance(transform.position, playerObject.transform.position) > maxDistanceFromPlayer)
			Destroy(gameObject);
	}

	void OnCollisionEnter(Collision collision)
	{
		// Destroy this object when it hits the ground
		if (collision.gameObject.name != playerObject.name)
		{
			// Spawn an instance of the egg splatter object that fades over time
			Vector3 eggDecalHeightOffset = new Vector3(0, 0.01f, 0);
			GameObject newDecalObject = Instantiate(
				eggSplatterObject,
				collision.GetContact(0).point + eggDecalHeightOffset,
				Quaternion.FromToRotation(Vector3.forward, collision.GetContact(0).normal)
				);
			newDecalObject.transform.RotateAround(Vector3.zero, collision.GetContact(0).normal, Random.Range(0, 1)); // Randomly rotate new object
			newDecalObject.transform.localScale *= Random.Range(1, 1.5f); // Randomly scale new object

			// Destroy the egg
			Destroy(gameObject);
		}
	}

	public void Shoot(Vector3 shootVector, float shootDistance)
	{
		// Set a force in the direction the egg is shot when it's created
		GetComponent<Rigidbody>().AddForce(shootVector.normalized * shootDistance, ForceMode.Impulse);
	}
}
