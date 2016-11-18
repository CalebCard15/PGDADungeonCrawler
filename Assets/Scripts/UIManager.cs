using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : PersistentSingleton<UIManager> {

	public Image healthBar;
	public Image manaBar;
	public Image xpBar;

	private GameManager gameManager;



	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLoadLevel;
	}
	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLoadLevel;
	}

	void OnLoadLevel(Scene scene, LoadSceneMode mode)
	{
		Init();
	}

	// Use this for initialization
	void Start () {
		
		Init();
//		levelText = GameObject.Find("LevelText").GetComponent<Text>();
//		healthText = GameObject.Find("HealthText").GetComponent<Text>();
	}

	void Init()
	{
		
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
		manaBar = GameObject.Find("ManaBar").GetComponent<Image>();
		xpBar = GameObject.Find("XPBar").GetComponent<Image>();
		UpdateUI();
	}

	public void UpdateUI()
	{
		//levelText.text = "Current Level: " + gameManager.currentLevel;
		healthBar.fillAmount = (float)Player2D.instance.health/Player2D.instance.maxHealth;
	}

}
