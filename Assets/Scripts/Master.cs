﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
	// UI Elements
	public GameObject pauseMenu;
	public float pauseDelaySeconds = 0.2f; // How long to wait before the pause button can be clicked after clicking once

	// Keep track of global game variables
	public static int chicksCollected; // How many chicks has the player collected?
	public static bool playerHasControl; // Does the player have control over the character?
	public static bool paused; // Is the game paused?
	private bool canPause; // Can the player currently pause the game?

	// Start is called before the first frame update
	void Start()
    {
		chicksCollected = 0;
		playerHasControl = true;
		paused = false;
		canPause = true;
	}

	private void Update()
	{
		if (Input.GetButton("Pause") && canPause)
		{
			// Pause the game
			paused = !paused;
			Time.timeScale = paused ? 0 : 1;
			playerHasControl = paused ? false : true;
			pauseMenu.SetActive(paused ? true : false);

			// If the player has pause functionality, delay the ability to pause for some time
			if (canPause) StartCoroutine("PauseDelay");
		}
	}

	IEnumerator PauseDelay()
	{
		canPause = false;
		yield return new WaitForSecondsRealtime(pauseDelaySeconds); // Have to use WaitForSecondsRealtime because time is stopped when pausing!
		canPause = true;
	}
}
