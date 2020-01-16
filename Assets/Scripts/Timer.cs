using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Timer : MonoBehaviour
{
	private float maxTime;
	private float currentTime;

	public Timer(float time, bool awake)
	{
		maxTime = time;
		if (awake) currentTime = maxTime;
		else currentTime = 0;
	}

	private void Update()
	{
		if (currentTime > 0) currentTime -= Time.deltaTime;
	}

	// Has the timer reached its elapsed time?
	public bool IsElapsed()
	{
		if (currentTime <= 0) return true;
		return false;
	}

	// Reset this timer and start it again
	public void Reset()
	{
		currentTime = maxTime;
	}
}
