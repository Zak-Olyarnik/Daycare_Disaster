using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menu that allows switching the control scheme.
public class Controls : MonoBehaviour
{
	public Image scheme;
	public Sprite key, mouse;		// sprites for button switching

	public void Start()
	{ GameController.standardControls = true; }

	// initializes and loads main level
	public void StartClick()
	{
		Menu.firstTime = false;
		GameController.days = 1;
		BabyController.wakeRate = .04f;
		ChildController.toyDropTime = 17f;
		ChildController.speed = .15f;
		SceneManager.LoadScene("main");
	}

	// return to menu
	public void BackClick()
	{ SceneManager.LoadScene("menu"); }

	// switch scheme and button indicator
	public void SwitchClick()
	{
		GameController.standardControls = !GameController.standardControls;
		if (GameController.standardControls)
		{ scheme.sprite = key; }
		else
		{ scheme.sprite = mouse; }
	}
}
