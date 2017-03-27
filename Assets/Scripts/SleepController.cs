using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls player interaction with babies.
public class SleepController : MonoBehaviour
{
	public BabyController bc;	// reference to the baby being put back to sleep

	// baby gets put back to sleep while player is in range
	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{ Invoke("GoToSleep", 0f); }
	}

	// start singing animation
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{ PlayerController.singingCycle = 1;  }
	}

	// stop singing animation
	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{ PlayerController.singingCycle = 0; }
	}

	void GoToSleep()
	{ bc.sleep += .3f; }

	
}
