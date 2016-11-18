using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player2D : MonoBehaviour {

	public GameObject bullet;
	public GameObject bulletSpawn;

	public int health = 100;

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
		health = 100;
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
		print(damage);
		if(health - damage <= 0)
		{
			//Die!!
			Enemy.GetHit -= Hit;
			gameManager.currentLevel = 1;
			SceneManager.LoadScene("Scene 2D");
		}

		health -= damage;
		print(health); 
	}


}
