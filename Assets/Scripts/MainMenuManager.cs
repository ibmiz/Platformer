using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour {
    public GameObject Level1image;
    public GameObject Level2image;
    public GameObject Level3image;
    private string loading;
    public string CharacterSelection;
	void Awake()
	{
        Level1image.SetActive(false);
        Level2image.SetActive(false);
        Level3image.SetActive(false);
	}

    public void Level1 ()
    {
        Level1image.SetActive(true);
        Level2image.SetActive(false);
        Level3image.SetActive(false);
        loading = "Level 1";
    }

    public void Level2()
    {
        Level1image.SetActive(false);
        Level2image.SetActive(true);
        Level3image.SetActive(false);
        loading = "Level 2";
    }

    public void Level3()
    {
        Level1image.SetActive(false);
        Level2image.SetActive(false);
        Level3image.SetActive(true);
        loading = "Level 3";
    }
    public void SelectCharacter(string select)
    {
        PlayerPrefManager.SelectCharacter(select);
    }
    public void Level()
    {
        SceneManager.LoadScene(loading);
    } 
	// load the specified Unity level
	public void loadLevel(string load)
	{
		// load the specified level
		SceneManager.LoadScene(load);
	}
    public void NewGame()
    {
        PlayerPrefManager.ResetScore();
    }
    public void onclick()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("CurrentLevel"));
    }
}
