using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public int damageAmount = 2; // this variable deals 10 damage to the character
    public float moveSpeed = 3f;
    public int enemyHealth = 5;

	public GameObject stunnedCheck; // what gameobject is the stunnedCheck

	public float stunnedTime = 3f;   // how long to wait at a waypoint
	public string stunnedLayer = "StunnedEnemy";  // layer to put enemy on when it is stunned
	public string playerLayer = "Player";  // layer to put enemy on when it is not stunned
	
	public bool Stunned = false;  // boolean flag for stunned

    public GameObject[] Waypoints; // defines movement waypoints

    public float waitAtWaypoint = 1f;   // time enemy waits at a any waypoint

    public bool Waypointsloop = true; // check if enemy is looping between waypoints

    // store references to components on the gameObject
    Transform Transform;
    Rigidbody2D Rigidbody;
    Animator Animator;
    AudioSource Audio;

    // movement tracking
    int WaypointIndex = 0; // used as index for My_Waypoints
    float MoveTime;
    float MovementX = 0f;
    bool isMoving = true;

    // store the layer number the enemy is on (setup in Awake)
    int enemyLayer;
	// store the layer number the enemy should be moved to when stunned
	int StunnedLayer;

    //Audio variables
    public AudioClip stunSFX;
    public AudioClip attackSFX;

    void Awake() {
		Rigidbody = GetComponent<Rigidbody2D> ();
		Animator = GetComponent<Animator>();
        Transform = GetComponent<Transform>();
        Audio = gameObject.AddComponent<AudioSource>();

        // determine the enemies specified layer
        enemyLayer = this.gameObject.layer;
		// determine the stunned enemy layer number
		StunnedLayer = LayerMask.NameToLayer(stunnedLayer);
   
		// prevents collision between player and enemy while in stunned state
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), StunnedLayer, true);

    }
	
	void Update ()
    {
        if (!Stunned)
        {
            if (Time.time >= MoveTime)
            {
                EnemyMovement();
            }
            else
            {
                Animator.SetBool("Moving", false);
            }
        }
    }

    void playAudio(AudioClip clip)
    {
        Audio.PlayOneShot(clip);
    }

    void EnemyMovement()
    {
        // if the enemy can move
        if ((Waypoints.Length != 0) && (isMoving))
        {

            // make enemy face the waypoint
            EnemyFlip(MovementX);

            // determine distance between waypoint and enemy
            MovementX = Waypoints[WaypointIndex].transform.position.x - Transform.position.x;

            // if the enemy is close enough to waypoint, make it's new target the next waypoint
            if (Mathf.Abs(MovementX) <= 0.05f)
            {
                // When arriving at a waypoint, set velocity to 0
                Rigidbody.velocity = new Vector2(0, 0);

                // increment to next index in array
                WaypointIndex++;

                // reset waypoint back to 0 for looping
                if (WaypointIndex >= Waypoints.Length)
                {
                    if (Waypointsloop)
                        WaypointIndex = 0;
                    else
                        isMoving = false;
                }

                // setup wait time at current waypoint
                MoveTime = Time.time + waitAtWaypoint;
            }

            else
            {
                // set enemy animation to moving
                Animator.SetBool("Moving", true);

                // Add velocity to enemy
                Rigidbody.velocity = new Vector2(Transform.localScale.x * moveSpeed, Rigidbody.velocity.y);
            }

        }
    }

    void OnCollisionEnter2D(Collision2D child)
    {
        if (child.gameObject.tag == "MovingPlatform")
        {
            this.transform.parent = child.transform;
        }
    }

    // if the enemy exits a collision with a moving platform, then unchild it
    void OnCollisionExit2D(Collision2D UnChild)
    {
        if (UnChild.gameObject.tag == "MovingPlatform")
        {
            this.transform.parent = null;
        }
    }

    public void EnemyDamage(int characterdamage) //damage function to be applied to enemy
    {
        enemyHealth -= characterdamage; //subtract determined damaged amount from health

        if (enemyHealth <= 0) // if the enemy health is less than or equal to 0...
        {
           StartCoroutine(KillEnemy()); //call coroutine to kill Enemy
        }
    }

    IEnumerator KillEnemy()
    {
        Animator.SetTrigger("Death"); //Set death trigger, which plays death animation
        yield return new WaitForSeconds(2.0f); //wait for 2 seconds before running next instruction
        Destroy(gameObject); //Destroy the enemy gameObject which removes it from the level
    }

    // flip the enemy to face torward the direction he is moving in
    void EnemyFlip(float MovementX)
    {

        // get the current scale
        Vector3 localScale = Transform.localScale;

        if ((MovementX > 0f) && (localScale.x < 0f))
            localScale.x *= -1;
        else if ((MovementX < 0f) && (localScale.x > 0f))
            localScale.x *= -1;

        // update the scale
        Transform.localScale = localScale;
    }
    
    // Attacking character
    void OnTriggerEnter2D(Collider2D collision)
	{
		if (!Stunned && (collision.tag == "Player"))
		{
			CharacterController2D character = collision.gameObject.GetComponent<CharacterController2D>();
            playAudio(attackSFX); //play attack sound
            // stop moving
            Rigidbody.velocity = new Vector2(0, 0);

            // apply damage to the player
            character.Damage(damageAmount);
		}
	}
    // setup the enemy to be stunned
    public void EnemyStunned()
	{
		if (!Stunned) 
		{
			Stunned = true;
            playAudio(stunSFX); //play stunned audio
            // provide the player with feedback that enemy is stunned
            Animator.SetTrigger("Stunned");
			
			// stop moving
			Rigidbody.velocity = new Vector2(0, 0);
			
			// switch layer to stunned layer so no collisions with the character
			this.gameObject.layer = StunnedLayer;
			stunnedCheck.layer = StunnedLayer;

			// start coroutine to be unstunned
			StartCoroutine (UnStun());

		}
	}
	
	// coroutine to unstun the enemy and stand back up
	IEnumerator UnStun()
	{
		yield return new WaitForSeconds(stunnedTime);//wait for a determined amount of time before reverting the enemy back 
		
		// enemy is not stunned
		Stunned = false;
		
		// switch layer back to regular layer for regular collisions with the character
		this.gameObject.layer = enemyLayer;
		stunnedCheck.layer = enemyLayer;
		
		// triggers unstun animation
		Animator.SetTrigger("Stand");
	}
}
