using UnityEngine;
using System.Collections;

public class ShootAtCharacter : MonoBehaviour {

    public float characterRange;//the range at which the enemy will engage with the character
    public GameObject bullet; //Add a sprite gameobject that will collide with the user
    public CharacterController2D character; //Get characters movement script to get damage 
    public Transform startpoint; //where the bullet will start from
    public float waitpershot; //how long it should wait per shot
    private float shotCounter; //get amount of shots done

	// Use this for initialization
	void Start () {
        character = FindObjectOfType<CharacterController2D>(); //gets characters movement script
        shotCounter = waitpershot; //make the shotcounter = wait per shot so we can compare to time
	}
	
	// Update is called once per frame
	void Update () {
        shotCounter -= Time.deltaTime; //remove current time to make shotCounter 0
        if (transform.localScale.x < 0 && character.transform.position.x > transform.position.x && character.transform.position.x < transform.position.x + characterRange && shotCounter<0)
        {
            Instantiate(bullet, startpoint.position, startpoint.rotation); //Create a bullet at starting position
            shotCounter = waitpershot; //reset timing to waitpershot so it waits before shooting again
        }
        if (transform.localScale.x > 0 && character.transform.position.x < transform.position.x && character.transform.position.x > transform.position.x - characterRange && shotCounter<0)
        {
            Instantiate(bullet, startpoint.position, startpoint.rotation); 
            shotCounter = waitpershot;
        }
    }
}
