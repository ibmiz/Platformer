using UnityEngine;
using System.Collections;

public class JumpPotion : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D jumppotion)
    {
        if ((jumppotion.tag == "Player") && (jumppotion.gameObject.GetComponent<CharacterController2D>().characterMove))
        {
            // the object will call the restorehealth function assigned to the Robot
            jumppotion.gameObject.GetComponent<CharacterController2D>().IncreaseJump();

            // destroy the object
            DestroyObject(this.gameObject);
        }
    }
}
