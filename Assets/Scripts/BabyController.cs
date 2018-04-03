using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls babies waking up.
public class BabyController : MonoBehaviour
{
	public static float wakeRate = .04f;		// rate of baby wakeup 
	public static float radiusRate = 1.0001f;	// rate of range increase
	public float sleep = 100f;					// sleep level 0-100
	public bool sleeping = true;				// used to indicate to GameController if game is lost
	public AudioSource asource;					// crying sound effect
	public GameObject bar;						// UI indication of sleep level
	public GameObject[] others;					// adjacent babies to be woken
	public SpriteRenderer baby;					// sprite switching when asleep/awake
	public Sprite asleep;
	public Sprite awake;
	public Sprite greenRange;
	public Sprite yellowRange;
	public Sprite redRange;
	public SpriteRenderer sleepIndicator;
	public Sprite[] sleepSprites;
	public Sprite[] tossingSprites;

	private SpriteRenderer range;
	private float barWidth = 7.5f;	// length of sleep bar when full

	private int sleepCycle;			// position in sleep animation
	private int tossingCycle;		// position in sleep animation

	void Start()
	{
		range = GetComponent<SpriteRenderer>();
		InvokeRepeating("SleepAnimation", 0f, .5f);
		InvokeRepeating("TossingAnimation", 0f, 1f);
	}

	void Update()
	{
        if (Time.timeScale == 0) return;    // Update is still run when timeScale = 0?  Inconsistent with Unity documentation...
        if (sleeping)
		{
			if (sleep >= 100f)	// max sleep is 100
			{ sleep = 100f; }
			
			if (sleep <= 0f)	// sleeping but sleep <= 0 -> wake up
			{
				sleep = 0f;
				baby.sprite = awake;
				range.sprite = redRange;
				sleeping = false;
				asource.Play();
				GameController.cryingBabies += 1;
			}
			else if (sleep <= 99f)		// still asleep but being worn down
			{ range.sprite = yellowRange; }
			else						// recharging a still-sleeping baby to 100
			{
				range.sprite = greenRange;
				baby.sprite = asleep;
			}
		}
		else		// awake
		{
			if (sleep <= 0f)	// min sleep is 0
			{ sleep = 0f; }

			if (sleep < 100f)	// normal awake behavior, wake nearby babies
			{
				foreach (GameObject g in others)
				{
					if (GameController.cryingBabies == 5)	// harder with more crying babies to ensure lose
					{ g.GetComponent<BabyController>().WakeUp(wakeRate * GameController.cryingBabies); }
					else
					{ g.GetComponent<BabyController>().WakeUp(wakeRate); }
				}
			}
			else		// awake but sleep >= 100 -> put back to sleep
			{
				sleep = 100f;
				baby.sprite = asleep;
				range.sprite = greenRange;
				sleeping = true;
				asource.Stop();
				GameController.cryingBabies -= 1;
			}
		}
		bar.gameObject.transform.localScale = new Vector3(sleep * barWidth / 100f, 1f, 1f);	// update bar length
		gameObject.transform.localScale = gameObject.transform.localScale * radiusRate;		// increase detection range slowly
	}

	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Child")		// child entered outer circle
		{ WakeUp(wakeRate); }
	}

	public void WakeUp(float rate)
	{ sleep -= rate; }

	// displays "zzzz" animation above sleeping baby
	public void SleepAnimation()
	{
		if (!sleeping)
		{ sleepCycle = 0; }
		else if (sleepCycle == 4)
		{ sleepCycle = 1; }
		else
		{ sleepCycle += 1; }

		sleepIndicator.sprite = sleepSprites[sleepCycle]; 
	}

	// displays tossing animation as baby begins to wake up
	public void TossingAnimation()
	{
		if (sleeping && sleep <= 99f)
		{
			if (tossingCycle == 3)
			{ tossingCycle = 0; }
			else
			{ tossingCycle += 1; }

		baby.sprite = tossingSprites[tossingCycle];
		}
	}
}
