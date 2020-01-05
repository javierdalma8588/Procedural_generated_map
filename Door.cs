using UnityEngine;
using System.Collections;

public class Door : DestructibleRoomPiece
{
	public GameObject damagedDoor;

	void OnCollisionEnter (Collision c)
	{
		if (c.collider.tag == "Bullet" || c.collider.tag == "Chair") {            
			AkSoundEngine.PostEvent ("Environment_WoodSmash", this.gameObject);
			Hit (c.collider);
			ShowDamagedDoor ();
		}

		//if (c.collider.tag == "RemoveDoor") {
			//print ("touching the door");
//			Destroy (this.gameObject);
		}
	}

	void OnTriggerEnter (Collider coll)
	{
		if (coll.tag == "Bullet") {             
			AkSoundEngine.PostEvent ("Environment_WoodSmash", this.gameObject);
			Hit (coll);
			ShowDamagedDoor ();
		}
	}

	void ShowDamagedDoor ()
	{
		mesh.enabled = false;
		if (resistance > 0)
			damagedDoor.SetActive (true);
		else
			damagedDoor.SetActive (false);              
	}
}