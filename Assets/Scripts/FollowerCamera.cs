using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerCamera : MonoBehaviour
{
	public GameObject objectToFollow;
	public float transparencyOfBlockingObjects = 0.3f;

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
		// Instantiate the RaycastHit array
		objectsBetweenCameraAndPlayer = new RaycastHit[16];
	}

	// Update is called once per frame
	void Update()
    {
		// Re-enable objects that were previously hidden 
		foreach (RaycastHit hit in objectsBetweenCameraAndPlayer)
		{
			if (hit.transform != null && hit.transform.gameObject.tag != "Player")
			{
				SetAlphaRecursive(hit.transform.gameObject, 1);
			}
		}

		Vector3 newPosition = objectToFollow.transform.position + cameraOffset;

		// Interpolate between the old position of the camera and the new one
		transform.position = Vector3.Slerp(transform.position, newPosition, 1);
		transform.LookAt(objectToFollow.transform);

		// Check if the player is hidden by an object between the camera and the player
		float distanceToObjectFollowing = cameraOffset.magnitude;
		objectsBetweenCameraAndPlayer = Physics.RaycastAll(transform.position, transform.forward, distanceToObjectFollowing);
		foreach (RaycastHit hit in objectsBetweenCameraAndPlayer)
		{
			if (hit.transform.gameObject.tag != "Player")
			{
				SetAlphaRecursive(hit.transform.gameObject, transparencyOfBlockingObjects);
			}
		}
	}

	// Get all children of a gameobject and set each of their alphas - ignore the player object!
	private void SetAlphaRecursive(GameObject parentGameObject, float alpha)
	{
		// If the transform has children, call this again for each child
		if (parentGameObject.transform.childCount > 0)
		{
			foreach (Transform childTransform in parentGameObject.transform)
			{
				SetAlphaRecursive(childTransform.gameObject, alpha);
			}
		}
		SetAlpha(parentGameObject.gameObject, transparencyOfBlockingObjects);
	}

	// Set the alpha opacity of an object's material
	private void SetAlpha(GameObject gameObject, float alpha)
	{
		Renderer renderer = gameObject.GetComponent<MeshRenderer>();
		if (renderer != null)
		{
			if (alpha == 1) ToOpaqueMode(renderer.material);
			else ToTransparentMode(renderer.material, alpha);
		}
	}

	private void ToOpaqueMode(Material material)
	{
		Color tempColor = material.color;
		tempColor.a = 1f;
		material.color = tempColor;
		material.SetOverrideTag("RenderType", "");
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
		material.SetInt("_ZWrite", 1);
		material.DisableKeyword("_ALPHATEST_ON");
		material.DisableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = -1;
	}

	private void ToTransparentMode(Material material, float alpha)
	{
		Color tempColor = material.color;
		tempColor.a = alpha;
		material.color = tempColor;
		material.SetOverrideTag("RenderType", "Transparent");
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
	}
}
