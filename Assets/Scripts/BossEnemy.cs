using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class BossEnemy : MonoBehaviour {


    public float health = 100; //health of boss
    public int damage = 5; //damage it deals
    public Transform[] attacklocations; //array of attack locations
    public float speed; //speed of boss movement
    public GameObject bullet; //bullet prefab reference
    public Transform startpoint; //where bullets are fired from
    GameObject character; //get reference of character
    Vector3 characterPos; //get character position
    public bool vulnerable; //boolean if vulnerable or not
    bool dead; //boolean if boss is dead or not
    public Image bosshealthBar; // Reference to the sprite renderer of the health bar.

    public void Awake()
    {
    }
    // Use this for initialization
    void Start()
    {
        character = GameObject.FindGameObjectWithTag("Player"); //get character component reference
        vulnerable = false; //make boss invulnerable
        bosshealthBar = GameObject.FindGameObjectWithTag("BossHealthBar").GetComponent<Image>();
        StartCoroutine("Attack1"); //start boss attack sequence
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !dead) //if health is less than or equal to 0 and is not currently dead...
        {
            dead = true; //make boss dead
            GetComponent<SpriteRenderer>().color = Color.red; //...make red to show dead
            StopAllCoroutines(); //Stop all attack sequences
            character.gameObject.GetComponent<CharacterController2D>().Victory();//Call victory function to end level
        }
    }

    public IEnumerator Attack1() //first part of attack
    {
        while (transform.position.x != attacklocations[0].position.x) //while location is not equal to attack location
        {
            //move boss towards attack location
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(attacklocations[0].position.x, transform.position.y), speed);
            yield return null;
        }

        int counter = 0;
        while (counter < 5) //while the counter is less than 5
        {
            Instantiate(bullet, startpoint.position, startpoint.rotation); // fire a bullet
            counter++;
            yield return new WaitForSeconds(1f);//wait for 1 second before firing another bullet
        }
        StartCoroutine("Attack2"); //call coroutine attack 2
    }
    public IEnumerator Attack2() //second part of attack sequence
    {
        StopCoroutine("Attack1"); //stop attack 1 couroutine
        GetComponent<Rigidbody2D>().isKinematic = true; //make the boss kinematic so it can jump
        while (transform.position != attacklocations[2].position) // while boss location not equal to second location...
        {
            transform.position = Vector2.MoveTowards(transform.position, attacklocations[2].position, speed); //...move the boss towards it
            yield return null;

        }

        characterPos = character.transform.position; //get character position

        yield return new WaitForSeconds(1f);
        GetComponent<Rigidbody2D>().isKinematic = false; //make boss fall down 
        if (characterPos.x > attacklocations[1].position.x && characterPos.x < attacklocations[0].position.x) //if character is within range
        {
            while (transform.position.x != characterPos.x) // while character position is not equal to boss location...
            {   //...move boss towards location
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(characterPos.x, transform.position.y), speed);
                yield return null;
            }
        }
        else
        {
            yield return null;
        }
        StartCoroutine("Attack3"); //call third attack sequence
    }
    public IEnumerator Attack3() //third attack sequence
    {
        StopCoroutine("Attack2"); //stop second attack sequnce
        Transform locationdependent; //make location variable
        if (transform.position.x > character.transform.position.x) //if location of character is to the left of the boss
            locationdependent = attacklocations[1]; //make the next location attacklocation[1]
        else
            locationdependent = attacklocations[0]; //else set to other location as it is on right side
        while (transform.position.x != locationdependent.position.x)
        {   //move toward that position
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(locationdependent.position.x, transform.position.y), speed);
            yield return null;
        }
        StartCoroutine("BossIsVulnerable"); //make boss vulnerable
    }

    public IEnumerator BossIsVulnerable()
    {
        StopCoroutine("Attack3");
        this.tag = "Untagged";
        vulnerable = true;
        GetComponent<SpriteRenderer>().color = Color.gray;
        yield return new WaitForSeconds(4);
        this.tag = "Enemy";
        vulnerable = false;
        StartCoroutine("Attack1");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && vulnerable) //if boss collides with character and is vulnerable
        {
            health -= 30; //deal damage
            UpdateHealthBar();
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (collision.gameObject.tag == "Player" && !vulnerable) // if boss is not vulnerable and collides with character
        {
            collision.gameObject.GetComponent<CharacterController2D>().Damage(damage); //deal damage to character
        }
    }

    public void UpdateHealthBar()
    {
        bosshealthBar.fillAmount = health / 100f;
    }
}
