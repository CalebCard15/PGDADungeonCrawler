using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {

	public GameManager manager;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
			manager.LoadNextLevel();
		}
	}
}
