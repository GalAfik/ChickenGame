using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NonPlayerCharacter : MonoBehaviour
{
	public Dialog dialogManager;
	public string[] dialogLines;
	public bool canChallange = false;
	public Race[] challanges;

	private int currentChallange;
	private Transform startingPoint; // The starting location for the current challange

	private void Start()
	{
		currentChallange = 0;
		// Disable all challanges
		if (challanges != null)
		{
			foreach (Race challange in challanges)
			{
				if (challange != null) challange.gameObject.SetActive(false);
			}
			// Get the current starting position
			startingPoint = challanges[currentChallange].transform.Find("NPCStartingPoint");
		}
	}

	private void Update()
	{
		if (Vector3.Distance(transform.position, startingPoint.position) <= 0.05f)
		{
			transform.forward = startingPoint.forward;
		}
	}

	public void Speak()
	{
		dialogManager.Speak(dialogLines);
		if (canChallange)
		{
			challanges[currentChallange].gameObject.SetActive(true);
			// Move to starting location and face forward
			startingPoint = challanges[currentChallange].transform.Find("NPCStartingPoint");
			StartCoroutine(Master.MoveToLocation(gameObject, transform.position, startingPoint.position, 1f));
		}
	}
}
