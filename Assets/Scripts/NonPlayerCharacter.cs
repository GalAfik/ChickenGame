using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NonPlayerCharacter : MonoBehaviour
{
	public Dialog dialogManager;
	public string[] dialogLines;

	public void Speak()
	{
		dialogManager.Speak(dialogLines);
	}
}
