using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float speed = 10.0f;				//How fast the bullet flies
	public float flightTime = 1.5f;			//How long the bullet can fly before it is destroyed
	public int shotDamage = 15;				//How much damage the bullet does to the enemy

	private float timeSinceSpawn;			
	public Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
		timeSinceSpawn = 0;
		rigidBody.velocity = transform.up * speed;
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceSpawn += Time.deltaTime;
		if(timeSinceSpawn >= flightTime)
		{
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Bullet")
		{
			return;
		}
		if(col.tag == "Enemy")
		{
			col.gameObject.GetComponent<Enemy>().TakeDamage(shotDamage);
		}
		Destroy(this.gameObject);
	}
}
