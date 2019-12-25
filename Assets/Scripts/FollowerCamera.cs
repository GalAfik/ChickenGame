using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerCamera : MonoBehaviour
{
	public GameObject objectToFollow;

	private Vector3 cameraOffset;
	
	// Set up object at start time
	private void Start()
	{
		// Manually set the position of the camera
		transform.position = objectToFollow.transform.position;
		transform.Translate(0, 4, -3, Space.World);
		// Get the relative position of the camera to the player
		cameraOffset = transform.position - objectToFollow.transform.position;
	}

	// Update is called once per frame
	void LateUpdate()
    {
		Vector3 newPosition = objectToFollow.transform.position + cameraOffset;

		// Interpolate between the old position of the camera and the new one
		transform.position = Vector3.Slerp(transform.position, newPosition, 1);
		transform.LookAt(objectToFollow.transform);
	}
}
