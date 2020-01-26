using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
	// Keep track of global game variables
	public static int chicksCollected; // How many chicks has the player collected?
	public static bool playerHasControl; // Does the player have control over the character?

	// Start is called before the first frame update
	void Start()
    {
		chicksCollected = 0;
		playerHasControl = true;

	}
}
