using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

	public int currentLevel;
	public Canvas pauseScreen;
	public Canvas deathScreen;
	public Text deathText;

	private bool isPaused;

	void Awake()
	{
		DontDestroyOnLoad(this);
		isPaused = false;

		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		currentLevel = 1;
	}

	void Update()
	{
		if(isPaused)
		{
			Time.timeScale = 0f;
			pauseScreen.enabled = true;
		}
		else
		{
			Time.timeScale = 1f;
			pauseScreen.enabled = false;
		}

		if(isPaused && Input.GetKeyDown(KeyCode.R))
		{
			Time.timeScale = 1f;
			RestartGame();
		}
		
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			isPaused = !isPaused;
		}
	}

	public void RestartGame()
	{
		currentLevel = 1;
		UIManager.instance.ReactivateBars();
		Player2D.instance.health = 100;
		SceneManager.LoadScene("Scene 2D");
		pauseScreen = GetComponentInChildren<Canvas>();
	}

	public void LoadNextLevel()
	{
		currentLevel++;
		UIManager.instance.ReactivateBars();
		SceneManager.LoadScene("Scene 2D");
	}



}
