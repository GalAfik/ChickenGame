using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerCamera : MonoBehaviour
{
	public GameObject objectToFollow;
	public float transparencyOfBlockingObjects = 0.1f;

	private Vector3 cameraOffset;
	private RaycastHit[] objectsBetweenCameraAndPlayer;

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
	void Update()
    {
		// Re-enable objects that were previously hidden 
		if (objectsBetweenCameraAndPlayer != null)
		{
			//foreach (RaycastHit hit in objectsBetweenCameraAndPlayer) SetAlpha(hit.transform.gameObject, 1f);
		}

		Vector3 newPosition = objectToFollow.transform.position + cameraOffset;

		// Interpolate between the old position of the camera and the new one
		transform.position = Vector3.Slerp(transform.position, newPosition, 1);
		transform.LookAt(objectToFollow.transform);

		// Check if the player is hidden by an object between the camera and the player
		float distanceToObjectFollowing = cameraOffset.magnitude;
		objectsBetweenCameraAndPlayer = Physics.RaycastAll(transform.position, transform.forward, distanceToObjectFollowing);
		//foreach (RaycastHit hit in objectsBetweenCameraAndPlayer) SetAlpha(hit.transform.gameObject, transparencyOfBlockingObjects);
	}

	// TODO : Set the alpha opacity of an object's material
	private void SetAlpha(GameObject gameObject, float alpha)
	{
		Renderer renderer = gameObject.GetComponent<MeshRenderer>();
		if (renderer != null)
		{
			Color tempColor = renderer.material.color;
			tempColor.a = alpha;
			renderer.material.SetColor("_Color", tempColor);
		}
	}
}
