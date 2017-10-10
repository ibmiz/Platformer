using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
    public float speed;
    public int damage;
    public CharacterController2D character;
    private Rigidbody2D bulletrigidbody;
	// Use this for initialization
	void Start () {
        character = FindObjectOfType<CharacterController2D>(); //get movement script of character

        if(character.transform.position.x< transform.position.x) // if the character is on the right side of the enemy
        {
            speed = -speed; //make the velocity negative so it goes right
        }
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y); //Constantly update the velocity to keep it moving
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") //if it collides with the player...
        {
            Destroy(gameObject); //destroy the bullet
            collision.GetComponent<CharacterController2D>().Damage(damage);//deal damage
            collision.GetComponent<CharacterController2D>().CharacterStunned();//make the character stunned
        }
    }
}
