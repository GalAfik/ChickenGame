using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
	public TextMeshProUGUI textDisplay;
	public GameObject continueButton;
	public GameObject dialogBox;
	public string[] sentences;
	public float typingEffectSpeed;

	private int index;

	IEnumerator Type()
	{
		foreach (char letter in sentences[index].ToCharArray())
		{
			textDisplay.text += letter;
			yield return new WaitForSeconds(typingEffectSpeed);
		}
	}

    // Start is called before the first frame update
    void Start()
    {
		textDisplay.text = "";
    }

	public void Speak(string[] dialog)
	{
		sentences = dialog;
		StartCoroutine(Type());
	}

	private void Update()
	{
		if (textDisplay.text == sentences[index])
		{
			continueButton.SetActive(true);
			Cursor.visible = true;
		}
	}

	public void NextSentence()
	{
		// Disable the continue button after it is clicked, until the sentence is done
		continueButton.SetActive(false);
		Cursor.visible = false;

		if (index < sentences.Length - 1)
		{
			index++;
			textDisplay.text = "";
			StartCoroutine(Type());
		}
		else
		{
			textDisplay.text = "";
			continueButton.SetActive(false);
			Master.playerHasControl = true;
		}
	}
}
