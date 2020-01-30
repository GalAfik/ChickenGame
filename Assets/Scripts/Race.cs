using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Race : MonoBehaviour
{
	public GameObject playerObject;
	public Material ringEnabledMaterial;
	public Material ringDisabledMaterial;
	public int totalLaps = 1;

	// Starting locations
	public GameObject npcStartingPoint;
	public GameObject playerStartingPoint;

	private List<GameObject> rings = new List<GameObject>(); // Keeps track of the rings in order
	private int iterator = 0; // Keeps track of the current ring objective
	private int currentLap = 1;

    // Start is called before the first frame update
    void Start()
    {
		// Get all child objects and store them in the rings list
		foreach (Transform child in transform)
		{
			if (child.gameObject.GetComponent<Ring>() != null) rings.Add(child.gameObject);
		}
	}

	// Handle collisions with the player object
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			/* If the total amount of laps is 1: each ring disappears after the player collides with it and the finishRace
			 * method is called when the last ring is crossed.
			 * If the total amount of laps is more than 1: rings stay in place until the final lap, where they disappear as usual. */
			
			// Make the current ring disappear or become inactive
			rings[iterator].GetComponent<BoxCollider>().enabled = false;
			if (currentLap == totalLaps)
				rings[iterator].transform.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
			else rings[iterator].transform.GetComponentInChildren<SkinnedMeshRenderer>().material = ringDisabledMaterial;

			// Make the next ring the active ring
			iterator++;

			// Make sure there are more rings, otherwise either go to the next lap or end the race
			if (iterator == rings.Count)
			{
				if (currentLap == totalLaps)
				{
					finishRace();
					return;
				}
				else
				{
					iterator = 0;
					currentLap++; // iterate to the next lap
					Debug.Log("finished lap!");
				}
			}

			rings[iterator].GetComponent<BoxCollider>().enabled = true;
			rings[iterator].transform.GetComponentInChildren<SkinnedMeshRenderer>().material = ringEnabledMaterial;
		}
	}

	// Move the competitors to the race starting point and start the countdown, then move the npc's through the race path
	public void StartRace(GameObject[] racers)
	{
		foreach (GameObject racer in racers)
		{
			// Move to starting location and face forward
			StartCoroutine(Master.MoveToLocation(racer, racer.transform.position, npcStartingPoint.transform.position, npcStartingPoint.transform.forward, 1f));
		}
		// Move the player to the starting location and face forward
		StartCoroutine(Master.MoveToLocation(playerObject, playerObject.transform.position, playerStartingPoint.transform.position, playerStartingPoint.transform.forward, 1f));
	}

	// Figure out if the player won the race and award them with a bonus chick
	private void finishRace()
	{
		Debug.Log("finished race!");
	}
}
