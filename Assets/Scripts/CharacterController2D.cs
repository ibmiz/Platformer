using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterController2D : MonoBehaviour {

	public float movementSpeed = 10f;
	public float jumpForce = 600f;
    public int characterHealth = 10;
    public int characterDamage = 2;
    public bool characterMove = true;

    // LayerMask to determine what is considered ground for the player
    public LayerMask whatIsGround;

	// Transform just below feet for checking if player is grounded
	public Transform groundCheck;

	// store references to components on the gameObject
	Transform _transform;
	Rigidbody2D _rigidbody;
	Animator _animator;
    AudioSource _audio;

	// Values to store the player motion in X and Y axis
	float MovementX;
	float MovementY;

	// These variables will track the characters current state which will be used for changing animations
	bool facingRight = true;
	bool Grounded = false;
	bool Running = false;
    bool DoubleJump = false;

    // store the layer the player is on 
    int CurrentCharacterLayer;

    // Whats layers are called 'Platform'
    int PlatformLayer;

    public float stunnedTime = 3f;   // how long to wait at a waypoint
    public string stunnedLayer = "StunnedEnemy";  // layer to put enemy on when it is stunned
    public string playerLayer = "Player";  // layer to put enemy on when it is not stunned
    // store the layer number the enemy should be moved to when stunned
    int StunnedLayer;

    public bool Stunned = false;  // boolean flag for stunned

    //Audio variables
    public AudioClip coinSFX;
    public AudioClip jumpSFX;
    public AudioClip deathSFX;
    public AudioClip victorySFX;

    Image UIHealthBar;

    void Awake () {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();
		_rigidbody = GetComponent<Rigidbody2D> ();	
		_animator = GetComponent<Animator>();
        _audio = gameObject.AddComponent<AudioSource>();
        // determine the character's current layer it is on
        CurrentCharacterLayer = this.gameObject.layer;

        // determine what objects are linked to the layer called "Platform"
        PlatformLayer = LayerMask.NameToLayer("Platform");
        
        // determine the stunned enemy layer number
        StunnedLayer = LayerMask.NameToLayer(stunnedLayer);

        UIHealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();
    }

    void Update()
    {
        if (characterMove)
        {
            Movement(); // Call the Movement function every frame to allow updates which change depending on user inputs
            Physics2D.IgnoreLayerCollision(CurrentCharacterLayer, PlatformLayer, (MovementY > 0.0f));//ignores collision while vertical movement is greater than 0
            Flip(); //run flip function to be checked every frame
            UpdateHealthBar();
        }
    }

    // This function is for the characters movement
	void Movement()
	{
		// determine horizontal velocity change based on the horizontal input
		MovementX = CrossPlatformInputManager.GetAxisRaw ("Horizontal");

		// Determine if running based on the horizontal movement
		if (MovementX != 0) 
		{
			Running = true;
		} else {
			Running = false;
		}

		// Turns on or off the animation state of the game depending on its boolean value. If it is true, the running animation will play instead idle animation
		_animator.SetBool("Running", Running);

		// Get current vertical velocity from the rigidbody component
		MovementY = _rigidbody.velocity.y;

		// Check to see if character is grounded by raycasting from the middle of the player down to the groundCheck position and see if collected with gameobjects on the whatIsGround layer
		Grounded = Physics2D.Linecast(_transform.position, groundCheck.position, whatIsGround);  

        if (Grounded) { //If the character is grounded...
            DoubleJump = true; //... it can doubleJump
        }

		// Set the grounded animation states
		_animator.SetBool("Grounded", Grounded);

		if(Grounded && CrossPlatformInputManager.GetButtonDown("Jump")) // If character is grounded and the jump button pressed, then allow jump animation to occur
		{
            Jump(); //Call Jump function to jump again
		}
        else if (DoubleJump && CrossPlatformInputManager.GetButtonDown ("Jump")) //if player can double jump and jump button press jump again
        {
            Jump(); //Jump again 
            DoubleJump = false; //Disable DoubleJump so you can only do it again once on the ground 
        }
        // Change the velocity on the rigidbody wihch is attached to the character
        _rigidbody.velocity = new Vector2(MovementX * movementSpeed, MovementY);
	}

	// Checking to see if the sprite should be flipped
	void Flip()
	{
		// get the current scale to modify
		Vector3 localScale = _transform.localScale;

		if (MovementX > 0) // moving right so face right
		{
			facingRight = true;
		} else if (MovementX < 0) { // moving left so face left
			facingRight = false;
		}

		// check to see if scale x is facing right, if not, multiple by -1 to flip the vector image to the left 
		if (((facingRight) && (localScale.x<0)) || ((!facingRight) && (localScale.x>0))) {
			localScale.x *= -1;
		}
		// update the scale which will provide visible updates to the player during gameplay
		_transform.localScale = localScale;
	}

   void Jump()
    {
        MovementY = 0f; // resetting value beforehand resolves issue of jumpforce being too large
                        
        _rigidbody.AddForce(new Vector2(0, jumpForce));// adds force in the vertical direction

        PlayAudio(jumpSFX);
    }

   public void Damage(int damage) //damage function to be applied to character
    {
        if (characterMove) // if the character can move i.e. not dead
        {
            characterHealth -= damage; //subtract determined damaged amount from health
            CharacterStunned(); //stun character

            if (characterHealth <= 0)
            { // player is now dead, so start dying
                StartCoroutine(KillCharacter());
                PlayAudio(deathSFX);
            }
        }
    }

    // setup the character to be stunned
    public void CharacterStunned()
    {
            // provide the player with feedback that character is stunned
            _animator.SetTrigger("Stunned");
            this.gameObject.tag = ("Untagged");
            // switch layer to stunned layer so no collisions
            this.gameObject.layer = StunnedLayer;
            // start coroutine to be unstunned
            StartCoroutine(UnStun());
    }

    // coroutine to unstun the character
    IEnumerator UnStun()
    {
        yield return new WaitForSeconds(stunnedTime);//wait for a determined amount of time before reverting back 

        // switch layer back to regular layer for regular collisions with the enemy
        this.gameObject.layer = CurrentCharacterLayer;
        // triggers unstun animation
        _animator.SetTrigger("Stand");
        this.tag = ("Player");
    }

    void PlayAudio(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

   public void CollectCoin(int total) //Will be called when a coin is collected
    {
        PlayAudio(coinSFX); //play sound to notify user
        GameManager.Manager.Score(total);//Call Game Manger function Score() to add a point to score total
    }

    // coroutine to kill the player
    IEnumerator KillCharacter()
    {
        if (characterMove)
        {
            // freeze the player
            FreezeCharacter();
            // play the death animation
            _animator.SetTrigger("Death");
            yield return new WaitForSeconds(2.0f); //wait for 2 seconds before running next instruction
            GameManager.Manager.ResetLevel(); //use Game Manager to reset level
        }
    }

    void UpdateHealthBar()
    {
        UIHealthBar.fillAmount = characterHealth/10f;
    }
 //freeze character function
    void FreezeCharacter()
    {
        characterMove = false; //makes the characterMove function false instead of its initial value true
        _rigidbody.isKinematic = true; //disable movement
    }

// Unfreeze character function
    void UnFreezeCharacter()
    {
        characterMove = true; //reverts charactermove variable to true to undo freeze motion
        _rigidbody.isKinematic = false; //allow for movement

    }


    // if the player collides with a MovingPlatform, then make it a child of that platform while it is in contact with it
    void OnCollisionEnter2D(Collision2D child)
    {
        if (child.gameObject.tag == "MovingPlatform") //the game will only make a child of the platform is the tag is "MovingPlatform"
        {
            this.transform.parent = child.transform;
        }
    }

    // if the player exits a collision with a moving platform, then unchild it
    void OnCollisionExit2D(Collision2D unchild)
    {
        if (unchild.gameObject.tag == "MovingPlatform")
        {
            this.transform.parent = null;
        }
    }

    public void JumpOnEnemy()
    {
        Jump();
        
    }

    public void FallDeathZone()
    {
        if (characterMove) //if character can move i.e. not dead...
        {
            characterHealth = 0; //set the health bar to 0 which means that the character can be killed
            StartCoroutine(KillCharacter()); //start Coroutine which kills the character
        }
    }

    // Victory function to be called when collecting key
    public void Victory()
    {
        PlayAudio(victorySFX); //Play victory clip 
        FreezeCharacter(); //Prevent character from moving anymore
        _animator.SetTrigger("Victory"); //Set trigger to play victory animation
        GameManager.Manager.LevelComplete();
    }

    // public function to respawn the player at the appropriate location
    public void RespawnCharacter(Vector3 position)
    {
        UnFreezeCharacter(); //Allow the character to move again
        characterHealth = 10; //Reset the health
        _transform.parent = null; //Prevent character from being a child and therefore prevent it falling through objects
        _transform.position = position; //respawn at starting position
        _animator.SetTrigger("Respawn"); //Trigger the respawn action to set the character to idle
    }

    public void RestoreHealth()
    {
        characterHealth = 10;
    }
    public void IncreaseJump()
    {
        jumpForce = jumpForce+100f;
    }
    public void IncreaseSpeed()
    {
        movementSpeed = movementSpeed + 5f;
    }
}