using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Text levelText;
	public Text healthText;

	private GameManager gameManager;
	private Player2D player;


	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player2D(Clone)").GetComponent<Player2D>();
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
//		levelText = GameObject.Find("LevelText").GetComponent<Text>();
//		healthText = GameObject.Find("HealthText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		levelText.text = "Current Level: " + gameManager.currentLevel;
		healthText.text = "Health: " + player.health;
	}
}
