using UnityEngine;
using System.Collections;
[System.Serializable]
public static class PlayerPrefManager {
    public static int GetLives() { //Get current lives
        if (PlayerPrefs.HasKey("Lives")) {
            return PlayerPrefs.GetInt("Lives");
        } else {
            return 0;
        }
    }

    public static void SetLives(int lives) { //This will set the starting amount of lives
        PlayerPrefs.SetInt("Lives", lives);
    }

    public static int GetScore() { //Gets current score of user
        if (PlayerPrefs.HasKey("Score")) {
            return PlayerPrefs.GetInt("Score");
        } else {
            return 0;
        }
    }

    public static void SetScore(int score) { //Sets score value which is updated onto the UI
        PlayerPrefs.SetInt("Score", score);
    }

    public static int GetHighscore() { //Gets saved highscore
        if (PlayerPrefs.HasKey("Highscore"))
        {
            return PlayerPrefs.GetInt("Highscore");
        } else {
            return 0;
        }
    }

    public static void SetHighscore(int highscore) { //Set the new highscore if broken
        PlayerPrefs.SetInt("Highscore", highscore);
    }

    public static int GetHealth() //Gets characters health
    {
        if (PlayerPrefs.HasKey("Health"))
        {
            return PlayerPrefs.GetInt("Health");
        }
        else {
            return 0;
        }
    }

    public static void SetHealth(int health) //Sets characters health
    {
        PlayerPrefs.SetInt("Health", health);
    }
    // store the current state into PlayerPrefs
    public static void SaveCharacterState(int score, int highScore, int lives, int Health) {
        // saves the current score and amount of lives and health to PlayerPrefs for moving to next level
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("Lives", lives);
        PlayerPrefs.SetInt("Highscore", highScore);
        PlayerPrefs.SetInt("Health", Health);
    }
	// reset back to defaults
	public static void ResetCharacterState(int startLives, bool resetHighscore) {
		PlayerPrefs.SetInt("Lives",startLives);
		PlayerPrefs.SetInt("Score", 0);

		if (resetHighscore)
			PlayerPrefs.SetInt("Highscore", 0);
	}

    public static void ResetScore()
    {
        PlayerPrefs.SetInt("Score", 0);
    }

    public static void SelectCharacter(string Character)
    {
        PlayerPrefs.SetString("SelectCharacter",Character);
    }

    public static void CurrentLevel(string level)
    {
        PlayerPrefs.SetString("CurrentLevel", level);
    }
}
