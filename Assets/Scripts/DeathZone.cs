using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {

	// If the character collides with the deathzone, then call the... 
	void OnCollisionEnter2D (Collision2D death)
    {
		if (death.gameObject.tag == "Player")
		{
			// ...FallDeathZone function which kills the character
			death.gameObject.GetComponent<CharacterController2D>().FallDeathZone();
		}

        else  { // if any other objects other than the character falls, just destroy them
			DestroyObject(death.gameObject);
		}
	}
}
