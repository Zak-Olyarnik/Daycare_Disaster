using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves the gameobject holding the singing animation to follow the player.
	// (Cannot parent because player rotation / collisions caused too many problems.)
public class SingingMover : MonoBehaviour
{
	public Transform playerPos;

	void Update()
	{ transform.position = new Vector2(playerPos.position.x + .71f, playerPos.position.y + .71f); }
}
