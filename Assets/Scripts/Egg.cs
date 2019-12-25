﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour, Shootable
{
	public GameObject playerObject;
	public float maxDistanceFromPlayer = 30f;

	protected new Rigidbody rigidbody;

	void Awake()
    {
		rigidbody = GetComponent<Rigidbody>();
		playerObject = GameObject.FindWithTag("Player");
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
			Destroy(gameObject);
	}

	public void Shoot(Vector3 shootVector, float shootDistance)
	{
		// Set a force in the direction the egg is shot when it's created
		rigidbody.AddForce(shootVector.normalized * shootDistance, ForceMode.Impulse);
	}
}
