using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls player movement and interactions, including two different control schemes
public class PlayerController : MonoBehaviour
{
	public static int numToys = 6;			// maximum number of toys in play
	public static int singingCycle = 0;		// position in singing animation

	public GameObject[] toys;
	public Transform toyDrop;		// drops toys slightly in front of player
	public SpriteRenderer singingIndicator;	// sprite switching when singing
	public Sprite[] singingSprites;

	private Rigidbody2D rb;
	private Animator anim;
	private Vector2 movement;		// vector of player movement at any given time
	private Vector3 target;			// mouse click position, used for point-and-click controls
	private float rotAngle;			// angle of sprite rotation
	private float speed = 2f;		// speed of movement
	private float toyRate = 1f;		// delay between dropping toys
	private float nextToy;			// time next toy can be dropped


	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		singingCycle = 0;
		InvokeRepeating("SingingAnimation", 0f, .5f);
	}

	void FixedUpdate()
	{
		if (GameController.standardControls)		// standard controls
		{
			// movement based on player input
			movement = new Vector2();
			movement.x = Input.GetAxis("Horizontal");
			movement.y = Input.GetAxis("Vertical");
			anim.SetFloat("movement", movement.magnitude);
			rb.velocity = movement * speed;

			// set sprite to face direction of movement
			if (movement.magnitude > 0)
			{
				rotAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);
			}

			// drop toy
			if (Input.GetButton("Fire1") && Time.time > nextToy && numToys > 0)
			{
				nextToy = Time.time + toyRate;
				numToys -= 1;
				Instantiate(toys[numToys], toyDrop.position, transform.rotation);
			}
		}
		else		// point-and-click controls
		{
			// set target to new mouse click position
			if(Input.GetButton("Fire1"))
			{
				//movement = new Vector2(0f, 0f);
				target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				target.z = 0f;
				movement = (target - transform.position).normalized;
			}

			// set sprite to face direction of movement
			if (movement.magnitude > 0)
			{
				rotAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);
			}

			// finally move
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
			if (Vector3.Distance(transform.position, target) < .1f)	// stop when very close to target
			{ movement = new Vector2(0f, 0f); }
			anim.SetFloat("movement", movement.magnitude);

			// drop toy
			if (Input.GetButton("Fire2") && Time.time > nextToy && numToys > 0)
			{
				nextToy = Time.time + toyRate;
				numToys -= 1;
				Instantiate(toys[numToys], toyDrop.position, transform.rotation);
			}
		}
	}

	// recollect dropped toys
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player_Pickup")
		{
			if (coll.gameObject.transform.parent.gameObject.GetComponent<ToyController>().playerPickup)	// enough time has passed to allow recollection
			{
				numToys += 1;
				Destroy(coll.gameObject.transform.parent.gameObject);
			}
		}
	}

	// don't try to run over babies in point-and-click
	void OnCollisionEnter2D(Collision2D coll)
	{
		if (!GameController.standardControls && coll.gameObject.tag == "Baby")
		{ movement = new Vector2(0f, 0f); }
	}

	// displays "zzzz" animation above sleeping baby
	public void SingingAnimation()
	{
		if (singingCycle == 0)
		{ }
		else if (singingCycle == 3)
		{ singingCycle = 1; }
		else
		{ singingCycle += 1; }

		singingIndicator.sprite = singingSprites[singingCycle];
	}
}
