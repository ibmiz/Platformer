using UnityEngine;
using System.Collections;

public class HealthPotion : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D healthpotion)
    {
        if ((healthpotion.tag == "Player") && (healthpotion.gameObject.GetComponent<CharacterController2D>().characterMove))
        {
            // the object will call the restorehealth function assigned to the Robot
            healthpotion.gameObject.GetComponent<CharacterController2D>().RestoreHealth();

            // destroy the object
            DestroyObject(this.gameObject);
        }
    }
}
