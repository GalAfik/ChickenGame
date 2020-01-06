using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
	// Floating effect variables
	public bool startRandom = true;
	public float amplitude = 0.2f;
	public float frequency = 1f;
	// Position Storage Variables
	private Vector3 posOffset = new Vector3();
	private Vector3 tempPos = new Vector3();
	private float randomOffset;

	// Start is called before the first frame update
	void Start()
    {
		// Store the starting position & rotation of the object
		posOffset = transform.position;

		// Start the object in a random place along its floating path
		if (startRandom)
		{
			randomOffset = Random.Range(0, 1000) * Time.deltaTime;
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Float up/down with a Sin()
		tempPos = posOffset;
		tempPos.y += Mathf.Sin((Time.fixedTime + randomOffset) * Mathf.PI * frequency) * amplitude; // Add a random offset to the start time to float randomly
		transform.position = tempPos;
	}

	// Handle collision with the player
	private void OnTriggerEnter(Collider other)
	{
		transform.parent.GetComponent<Race>().OnTriggerEnter(other);
	}
}
