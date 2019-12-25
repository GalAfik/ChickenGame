using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassController : MonoBehaviour
{
	public GameObject grassObject;
	public float grassDensity;

	private Vector3 size;

    // Start is called before the first frame update
    void Start()
    {
		// Get size of plane in pixels
		Mesh planeMesh = GetComponent<MeshFilter>().mesh;
		Bounds bounds = planeMesh.bounds;
		// size in pixels
		size = new Vector3(
				transform.localScale.x * bounds.size.x,
				transform.localScale.y * bounds.size.y,
				transform.localScale.z * bounds.size.z
			);

		GenerateGrass();   
    }

	// Instantiate a random amount of grass and reposition it along the plane
	void GenerateGrass()
	{
		for (int i = 0; i < 2000 * grassDensity; i++)
		{
			Vector3 spawnPosition = new Vector3(
					Random.Range(-size[0]/2, size[0] / 2),
					0,
					Random.Range(-size[2]/2, size[2]/2)
				);
			Instantiate(grassObject, spawnPosition, Quaternion.identity);
		}
	}
}
