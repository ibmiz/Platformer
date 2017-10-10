using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {


	// if the character touches the key and can move, then it has reached the end of the level
	void OnTriggerEnter2D (Collider2D key)
	{
		if ((key.tag == "Player" ) && (key.gameObject.GetComponent<CharacterController2D>().characterMove))
		{
			// the key object will call the victory function assigned to the Robot
			key.gameObject.GetComponent<CharacterController2D>().Victory();

			// destroy the victory object
			DestroyObject(this.gameObject);
		}
	}

}
