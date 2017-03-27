using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Controls menu interactions.
public class Menu : MonoBehaviour
{
	public Text bestTimeText;				// displays high score
	public Text lastTimeText;				// displays last earned score
	public static bool firstTime = true;	// initial play flag
	public static int bestTime = 0;			// high score

	// displays scores only after first and subsequent plays
	void Start()
	{
		if (!firstTime)
		{
			if (GameController.days > bestTime)
			{ bestTime = GameController.days; }
			bestTimeText.text = "Longest Job: " + bestTime + " days";
			lastTimeText.text = "Last Job: " + GameController.days + " days";
		}
	}

	// moves to controls screen
	public void StartClick()
	{ SceneManager.LoadScene("controls"); }

	// exits
	public void QuitClick()
	{ Application.Quit(); }
}