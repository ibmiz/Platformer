using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class getScore : MonoBehaviour {
    public int score;
    public int highscore;
    public Text UIScore;
    public Text UIHighScore;
	// Use this for initialization
	void Start () {
        score = PlayerPrefManager.GetScore();
        highscore = PlayerPrefManager.GetHighscore();
        UIScore.text = "Score: " + score.ToString();
        UIHighScore.text = "Highscore: " + highscore.ToString();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}