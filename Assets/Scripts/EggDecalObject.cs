using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggDecalObject : MonoBehaviour
{
	public float fadeRate = .05f; // How fast the sprite should fade before disappearing
	private float alpha = 1; // Starting alpha

    // Update is called once per frame
    void Update()
    {
		// Fade out over time
		alpha -= fadeRate * Time.deltaTime;
		GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(1, 1, 1, alpha));

		// Destroy object when it's invisible
		if (alpha <= 0) Destroy(gameObject);
    }
}
