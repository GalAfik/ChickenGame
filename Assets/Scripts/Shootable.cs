using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Shootable
{
	void Shoot(Vector3 shootVector, float shootDistance);
}