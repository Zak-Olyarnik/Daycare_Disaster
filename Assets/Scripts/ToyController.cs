using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls toys being picked up by children.
public class ToyController : MonoBehaviour
{
	public AudioSource asource;			// plays child pickup sound
	public bool playerPickup = true;	// toy can be picked up by player
	public bool childPickup = true;		// toy can be picked up by child

	private ChildController cc;		// reference to the child picking up the toy

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Child")
		{
			cc = coll.GetComponent<ChildController>();
			if (childPickup == true && !cc.hasToy)	// prevents duplicate pickup
			{
				cc.PickupToy(gameObject);
				childPickup = false;
				playerPickup = false;
				transform.SetParent(cc.toyHolder);
				transform.localPosition = new Vector3(0f, 0f, 0f);
				asource.Play();
			}
		}
	}
}
