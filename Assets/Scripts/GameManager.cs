using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement; // include so we can load new scenes
public class GameManager : MonoBehaviour {
	// static reference to game manager so can be called from other scripts directly (not just through gameobject component)
	public static GameManager Manager;

	// game performance
	public int score = 0;
	public int highscore = 0;
	public int startLives = 3;
	public int lives = 3;
    public int health;

	// UI elements to control
	public Text UIScore;
	public Text UIHighScore;
	public GameObject[] UIExtraLives;
	public GameObject UIGamePaused;

    GameObject Character;
    Vector3 Startlocation; //Vector location of where character will respawn to

    // levels to move to on victory and lose
    public string Victory; 
    public string GameOver;

    private bool paused = false; //is the game paused or not
    string CharactersSelection; //this string will determine which character is selected
    public GameObject Robot; //get reference to robot character
    public GameObject Ninja; //get reference to ninja character
    public GameObject Man;

    // set things up here
    void Awake()
    {
        // setup reference to game manager
        if (Manager == null)
            Manager = this.GetComponent<GameManager>(); //Get Manager gameoobject reference

        CharactersSelection = PlayerPrefs.GetString("SelectCharacter"); //get string from character selection screen

        if (CharactersSelection == "Robot") //if user selects robot as character...
        {
            Ninja.SetActive(false);//..disable ninja character
            Ninja.tag = "Untagged"; //change ninja charcter tag

            Robot.SetActive(true);//...enable robot character
            Robot.tag = "Player"; //set Robot to Player tag

            Man.SetActive(false);//...disable man character
            Man.tag = "Untagged"; //change man charcter tag
        }
        else if (CharactersSelection == "Ninja") //if user selects ninja as character...
        {
            Robot.SetActive(false);//..disable robot character
            Robot.tag = "Untagged"; //change robot character tag

            Ninja.SetActive(true);//...enable ninja character
            Ninja.tag = "Player"; //set Ninja to Player tag

            Man.SetActive(false);//...disable man character
            Man.tag = "Untagged"; //change robot charcter tag
        }
        else if (CharactersSelection == "Man")//if user selects man as character
        {
            Man.SetActive(true);//...enable man character
            Man.tag = "Player"; //set Man to Player tag

            Robot.SetActive(false);//..disable robot character
            Robot.tag = "Untagged"; //change robot charcter tag

            Ninja.SetActive(false);//...enable ninja character
            Ninja.tag = "Untagged"; //change ninja charcter tag
        }
        else
        {
            Ninja.SetActive(false);
            Ninja.tag = "Untagged";
            Man.SetActive(false);
            Man.tag = "Untagged";  //...if there is no string value set robot to default character
        }

        refreshCharacterState();
        refreshGUI();
        LifeIndicator();

        Character = GameObject.FindGameObjectWithTag("Player"); //Get reference of gameobject with tag player
        CharacterController2D dynamichealth = Character.GetComponent<CharacterController2D>();
        health = dynamichealth.characterHealth;

        Startlocation = Character.transform.position; //Gets the starting location will be the respawn point 
        UIScore.text = "Score: " + score.ToString();//Appends current score to score on UI
        UIHighScore.text = "Highscore: " + highscore.ToString(); //Appends highscore to highscore on UI
    }

    // get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
    void refreshCharacterState()
    {
        lives = PlayerPrefManager.GetLives();
        health = PlayerPrefManager.GetHealth();
        if (lives <= 0)
        {
            PlayerPrefManager.ResetCharacterState(startLives, false);//Reset characters state
            lives = PlayerPrefManager.GetLives(); //get current amount of lives and test again
        }
        score = PlayerPrefManager.GetScore(); //Gets score of user
        highscore = PlayerPrefManager.GetHighscore(); //Gets the current highscore
    }

    void refreshGUI()
    {
        // set the text elements of the UI
        UIScore.text = "Score: " + score.ToString();//Appends current score to score on UI
        UIHighScore.text = "Highscore: " + highscore.ToString(); //Appends highscore to highscore on UI
    }

    public void Paused()
    {
        if (paused == true) //if pause menu is active
        {
            paused = false;//unpause menu
        }
        else
        {
            paused = true; //else pause menu
        }
    }
    // game loop
    void Update()
    {
        if (paused == true)
        {
        UIGamePaused.SetActive(true); // this brings up the pause UI
        Time.timeScale = 0f; // this pauses the game action
        }

        if (paused == false)
        {
        Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
        UIGamePaused.SetActive(false); // remove the pause UI
        } 
    }

    public void Resume() //For resume button in paused menu
    {
        paused = false;
    }

    public void Restart() //For restart button in paused menu
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void MainMenu() //For main menu button in paused menu
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void SaveOnClick()
    {
        PlayerPrefs.SetFloat("CharacterPosX", Character.transform.position.x);
        PlayerPrefs.SetFloat("CharacterPosY", Character.transform.position.y);
        PlayerPrefs.SetFloat("CharacterPosZ", Character.transform.position.z);
        PlayerPrefManager.SetHealth(health);
        PlayerPrefManager.SetLives(lives);
        PlayerPrefManager.SetScore(score);
        PlayerPrefManager.SetHighscore(highscore);
        PlayerPrefManager.CurrentLevel(SceneManager.GetActiveScene().name);
    }



    // Shows how many lives left
    void LifeIndicator() 
    {
        // From 0 to the length of the array...
        for (int i = 0; i < UIExtraLives.Length; i++)
        {
            if (i < (lives - 1)) //...check if i is one less than the current lives so the grid can be activated
            { UIExtraLives[i].SetActive(true);}
            else
            {UIExtraLives[i].SetActive(false);}
        }
    }

    // A function that will respawn the character or end the level depending on the remaning lives
    public void ResetLevel()
    {
        // remove one life
        lives--;
        LifeIndicator(); //Update life indicator to have one less heart
        if (lives <= 0)
        {
            // save the current player prefs before going to GameOver
            PlayerPrefManager.SaveCharacterState(score, highscore, lives, health);

            // load the GameOver screen
            SceneManager.LoadScene(GameOver);
        }
        else
        {
            //  if the character still has lives, tell the player to respawn
            Character.GetComponent<CharacterController2D>().RespawnCharacter(Startlocation);
        }
    }

    public void Score(int total)
    {
        // increase score
        score += total;

        // update UI
        UIScore.text = "Score: " + score.ToString();

        // if score>highscore then update the highscore UI too
        if (score > highscore)
        {
            highscore = score;
            UIHighScore.text = "Highscore: " + score.ToString();
        }
    }

    // public function for level complete
    public void LevelComplete()
    {
        // save the current player prefs before moving to the next level
        PlayerPrefManager.SaveCharacterState(score, highscore, lives, health);

        // use a coroutine to allow the player to get fanfare before moving to next level
        StartCoroutine(LoadNextLevel());
    }

    // load the nextLevel after delay
    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(Victory);
    }
}
