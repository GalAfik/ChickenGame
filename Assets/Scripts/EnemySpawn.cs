using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
	public GameObject enemyObject;
	public int spawnCount;
	public float spawnDistance;
	public float primeDistance; // The player must be this far away for the spawn point to re-activate

	private GameObject playerObject;
	private bool active = true; // Is the spawn point primed to spawn new enemies?

    // Start is called before the first frame update
    void Start()
    {
		playerObject = GameObject.FindWithTag("Player");
	}

    // Update is called once per frame
    void Update()
    {
		// If the player is within the spawn distance, spawn enemies
		if (Vector3.Distance(transform.position, playerObject.transform.position) <= spawnDistance && active)
		{
			for (int i = 0; i < spawnCount; i++)
			{
				Instantiate(enemyObject);
				// Turn off the spawn point until the player leaves the area
				active = false;
			}
		}

		// If the player is outside of the prime distance, the spawn point becomes re-activated 
		if (Vector3.Distance(transform.position, playerObject.transform.position) >= primeDistance && !active)
		{
			active = true;
		}
	}
}
