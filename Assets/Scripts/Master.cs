using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
	// Keep track of global game variables
	public static int chicksCollected;

    // Start is called before the first frame update
    void Start()
    {
		chicksCollected = 0;
    }
}
