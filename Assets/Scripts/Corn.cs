using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corn : MonoBehaviour
{
	public GameObject playerObject;
	public float turnOnDistance = 10f; // The object should only emmit particles when the player is this close to it

	// Floating effect variables
	public float degreesPerSecond = 15.0f;
	public float amplitude = 0.2f;
	public float frequency = 1f;
	// Position Storage Variables
	private Vector3 posOffset = new Vector3();
	private Vector3 tempPos = new Vector3();

	private ParticleSystem.EmissionModule emissionModule;



	// Start is called before the first frame update
	void Start()
    {
		// Store the starting position & rotation of the object
		posOffset = transform.position;

		// Turn off the particle effect until the player gets close to this object
		emissionModule = GetComponent<ParticleSystem>().emission;
		emissionModule.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
		// Turn on the particle effect if the player is close to this object
        if (Vector3.Distance(playerObject.transform.position, transform.position) <= turnOnDistance)
		{
			emissionModule.enabled = true;
		}

		// Spin object around Y-Axis
		transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

		// Float up/down with a Sin()
		tempPos = posOffset;
		tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

		transform.position = tempPos;
	}
}
