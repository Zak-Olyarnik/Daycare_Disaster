using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Controls UI and ending the game.
public class GameController : MonoBehaviour
{
	public static bool standardControls = true;		// sets whether controls are standard or point-and-click
	public static int days = 1;						// days passed in game
	public static int time;							// time passed in game
	public static int cryingBabies;					// number of crying babies, controls game loss

	public Text dayText;			// UI
	public Text timeText;
	public Text toysText;
	public GameObject click;
	public GameObject mask;
	public GameObject day;
	public GameObject num1;
	public GameObject num2;
	public Sprite[] nums;
	public Transform[] babyLocs;	// list of babies for children to target
	public GameObject[] babies;		// list of babies to randomly wake up
	public AudioSource asource;

	// Singleton
	public static GameController instance;
	public static GameController getInstance()
	{ return instance; }

	void Awake()
	{ instance = this; }

	void Start()
	{
		Time.timeScale = 0;
		mask.SetActive(true);
		click.SetActive(true);
		day.SetActive(true);
		int ones = days % 10;
		num2.GetComponent<Image>().sprite = nums[ones];
		int tens = ((days % 100) - ones) / 10;
		num1.GetComponent<Image>().sprite = nums[tens];
		num1.SetActive(true);
		num2.SetActive(true);
		dayText.text = "Day: " + days;
		time = 60;
		timeText.text = "Time: " + time;
		PlayerController.numToys = 6;
		toysText.text = "Toys Left: " + PlayerController.numToys;
		cryingBabies = 0;

		// adjust values for creep
		ChildController.speed += .15f;
		ChildController.toyDropTime -= 2f;
		if (ChildController.toyDropTime < 3f)
		{ ChildController.toyDropTime = 3f; }
		BabyController.wakeRate += .01f;
	}

	// starts new round on screen click
	public void Begin()
	{
		Time.timeScale = 1;
		mask.SetActive(false);
		click.SetActive(false);
		day.SetActive(false);
		num1.SetActive(false);
		num2.SetActive(false);
		InvokeRepeating("Clock", 1, 1);
		InvokeRepeating("RandomWakeup", 20f, 20f);
		asource.Play();
	}
	
	void Update()
	{
		if (PlayerController.numToys == 0)
		{ toysText.text = "Out of Toys!"; }
		else
		{ toysText.text = "Toys Left: " + PlayerController.numToys; }
		if (cryingBabies == 6)		// game over
		{ Invoke("Endgame", 1.5f); }
	}

	// updates the simple seconds counter, which also serves as a score display
	void Clock()
	{
		time -= 1;
		timeText.text = "Time: " + time;
		if (time == 0)
		{ SwitchRound(); }
	}

	// randomly wakes up a baby
	void RandomWakeup()
	{ StartCoroutine(RandomWakeupRepeating()); }

	// randomly wakes up a baby
	IEnumerator RandomWakeupRepeating()
	{
		int selection = Random.Range(0, babies.Length);
		BabyController baby = babies[selection].GetComponent<BabyController>();
		do
		{
			baby.WakeUp(.1f);
			yield return new WaitForSeconds(.025f);
		}
		while (baby.sleeping && baby.sleep != 100f);
	}

	// switches to new round with increased difficulty
	void SwitchRound()
	{
		days += 1;
		SceneManager.LoadScene("main");
	}

	void Endgame()
	{ SceneManager.LoadScene("end"); }
}
