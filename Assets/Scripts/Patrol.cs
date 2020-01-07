// Patrol.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Patrol : MonoBehaviour
{
	public float speed;
	public GameObject[] points;

	private int destPointIndex = 0;
	private Vector3 destination;

	// Begin by going toward the first destination point
	void Start()
	{
		GotoNextPoint();
	}

	// Set the next destination point
	void GotoNextPoint()
	{
		// Returns if no points have been set up
		if (points.Length == 0)
			return;

		// Set the agent to go to the currently selected destination.
		destination = points[destPointIndex].transform.position;

		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPointIndex = (destPointIndex + 1) % points.Length;
	}

	// Move toward the current destination
	void Update()
	{
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (transform.position == destination)
			GotoNextPoint();
		else
		{
			// Move towards the current destination
			transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
		}
		// Always face the next destination
		transform.LookAt(destination);
		transform.forward = -transform.forward; // For some reason, the mesh on tractors are reversed...
	}
}