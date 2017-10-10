using UnityEngine;
using System.Collections;

public class Coins : MonoBehaviour {

	public int coinValue = 1;
	public bool collected = false;

	// if the character touches the coin and it has not been collected and the player can move then collect it
	void OnTriggerEnter2D (Collider2D coin)
	{
		if ((coin.tag == "Player" ) && (!collected) && (coin.gameObject.GetComponent<CharacterController2D>().characterMove))
		{
			// mark as collected so doesn't get taken multiple times
			collected=true;

			// this calls the CharacterController2S script to play the audio sfx
			coin.gameObject.GetComponent<CharacterController2D>().CollectCoin(coinValue);

			// destroys the coin
			DestroyObject(this.gameObject);
		}
	}

}
