using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls child movement and interactions, implemented as a state machine.
public class ChildController : MonoBehaviour
{
	public enum State { Start, Wander, MoveToTarget, Stop };	// child behaves as a state machine

	public static float toyDropTime = 17f;		// time toy effect lasts (slowly decreases)
	public static float speed = .15f;				// speed of movement

	public State state;				// current state
	public bool hasToy = false;		// has toy (controls movement and stops duplicate pickup)
	public Transform toyHolder;		// sprite location of toy
	public AudioSource asource;		// plays toy drop sound
	
	private Rigidbody2D rb;
	private Animator anim;
	private Vector2 movement;			// movement direction
	private float rotAngle;				// direction of sprite
	private Vector3 target;				// target movement position
	private Transform[] babyLocs;		// list of babies as possible targets
	private GameObject toy;				// collected toy
	private int normalWeight = 1;
	private int stoppedWeight = 50;		// children become heavier when stopped so they can't be moved

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		babyLocs = GameController.getInstance().babyLocs;
		state = State.Start;
		Invoke("RandomizeMovement", 2.5f);
	}
	
	void FixedUpdate() 
	{
		switch (state)
		{
			case (State.Start):
				rb.velocity = transform.right * speed;
				break;
			case (State.Wander):		// wander randomly
				anim.SetBool("stopped", false);
				rb.velocity = movement * speed;
				break;
			case (State.MoveToTarget):	// move towards target (baby or center)
				anim.SetBool("stopped", false);
				rb.velocity = new Vector3(0f, 0f, 0f);
				if (Vector3.Distance(transform.position, target) < 1f)	// stop when very close to target
				{
					state = State.Stop;
					rb.mass = stoppedWeight;
				}
				else
				{
					movement = (target - transform.position).normalized;
					transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
				}
				break;
			case (State.Stop):		// stop moving
				anim.SetBool("stopped", true);
				movement = new Vector2(0f, 0f);
				rb.velocity = movement * speed;
				break;
			default:
				break;
		}

		// set sprite to face direction of movement
		if (movement.magnitude > 0)
		{
			rotAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Border")	// tried to leave screen
		{ movement = -movement; }
		else if (coll.gameObject.tag == "Baby_Inner" && !hasToy)
		{
			state = State.Stop;
			rb.mass = stoppedWeight;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Child" && state != State.Stop)	// ignore collision if this child is stopped
		{
			if (hasToy)
			{ movement = -movement; }	// back up but continue on path to center
			else
			{
				state = State.Wander;	// change movement randomly
				movement = -movement;
			}
		}
	}

	// invoked from ToyController on collider interaction with toy
		// child returns to center of screen and drops toy after a slowly-decreasing time period
	public void PickupToy(GameObject t)
	{
		toy = t;
		hasToy = true;
		Invoke("DropToy", toyDropTime);
		target = new Vector3(0f, 0f, 0f);
		state = State.MoveToTarget;
		rb.mass = normalWeight;
	}

	// drops toy and returns to wandering, called with a delay after pickup
	void DropToy()
	{
		asource.Play();
		toy.transform.SetParent(null);
		toy.GetComponent<ToyController>().playerPickup = true;
		hasToy = false;
		state = State.Wander;
		rb.mass = normalWeight;
		RandomizeMovement();
	}

	// randomize movement every few seconds.  1/5 chance to pick a baby as a "target" and move directly to it
	void RandomizeMovement()
	{
		if (state == State.Wander || state == State.Start)
		{
			int rand = Random.Range(0, 5);
			if (rand < 4)		// random wander movement
			{ movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized; }
			else				// move to baby
			{
				int baby = Random.Range(0, babyLocs.Length);
				target = babyLocs[baby].position;
				state = State.MoveToTarget; 
			}
		}
		Invoke("RandomizeMovement", Random.Range(3f, 5f));
	}
}
