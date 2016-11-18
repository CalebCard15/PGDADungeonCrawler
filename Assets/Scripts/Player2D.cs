using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player2D : PersistentSingleton<Player2D> {

	public GameObject bullet;
	public GameObject bulletSpawn;

	public int maxHealth = 100;
	public int health;

	public float secondsPerShot = .5f;
	private float timeSinceLastShot;
	private GameManager gameManager;

	private Animator playerAnim;

		

	Rigidbody2D rigidbody;
	Vector2 velocity;
	
	void Start () {
		rigidbody = GetComponent<Rigidbody2D> ();
		Enemy.GetHit += Hit;
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		health = maxHealth;
		playerAnim = GetComponent<Animator>();
	}

	void Update () {
		timeSinceLastShot += Time.deltaTime;
		LookAtMouse();
		velocity = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical")).normalized * 10;
		if(Input.GetMouseButton(0) && timeSinceLastShot >= secondsPerShot)
		{
			Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
			timeSinceLastShot = 0f;
		}

		if(Input.GetAxisRaw ("Horizontal") != 0 || Input.GetAxisRaw ("Vertical") != 0)
		{
			playerAnim.SetBool("Walking", true);
		}
		else
		{
			playerAnim.SetBool("Walking", false);
		}
	}

	void FixedUpdate() {
		rigidbody.MovePosition (rigidbody.position + velocity * Time.fixedDeltaTime);
	}

	void LookAtMouse()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;
		transform.up = mousePos - transform.position;
	}

	void Hit(int damage)
	{
		health -= damage;
		UIManager.instance.UpdateUI();

		if(health <= 0)
		{
			//Die!!
			Enemy.GetHit -= Hit;
			gameManager.currentLevel = 1;
			SceneManager.LoadScene("Scene 2D");
		}



	}


}
