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
		}
	}

	public void Speak()
	{
		dialogManager.Speak(dialogLines);
		if (canChallange)
		{
			challanges[currentChallange].gameObject.SetActive(true);
			// Move to starting location and face forward
			//transform.position = challanges[currentChallange].gameObject.GetComponent<>.startingPoint.transform.position;
			//transform.forward = startingPoint.transform.forward;
		}
	}
}
