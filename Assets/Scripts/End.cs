using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//  Displays game over screen.
public class End : MonoBehaviour
{
	// moves to menu screen
	public void NextClick()
	{ SceneManager.LoadScene("menu"); }
}
