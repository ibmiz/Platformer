using UnityEngine;
using System.Collections;

public class EnemyStun : MonoBehaviour {
    
    private int CharacterDamage; //A variable that will dynamically change depending on damage in CharacterController2D script

    public void Start()
    {
        GameObject Character = GameObject.FindGameObjectWithTag("Player"); //Get GameObject called Player which I can get variables from
        CharacterController2D DynamicDamage = Character.GetComponent<CharacterController2D>(); //Get the script component of the character
        CharacterDamage = DynamicDamage.characterDamage; //Assign the variable of the characterDamage in the other script to local variable
    }// if Player hits the stun point of the enemy, then call Stunned on the enemy

    void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			
			this.GetComponentInParent<Enemy>().EnemyStunned();// tell the enemy to be stunned
            this.GetComponentInParent<Enemy>().EnemyDamage(CharacterDamage); //Apply damage
		}
        //make player bounce off head
        other.gameObject.GetComponent<CharacterController2D>().JumpOnEnemy();
    }
}
