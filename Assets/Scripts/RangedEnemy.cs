using UnityEngine;
using System.Collections;

public class RangedEnemy : Enemy {

	public GameObject projectile;

	public float attackSpeed = 1.0f;
	public float timeSinceAttack = 0.0f;

	// Use this for initialization
	void Start () {
		base.Start();
		health = 60;
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	public override void Move()
	{
		if(Vector3.Distance(transform.position, Player2D.instance.transform.position) < 8)
		{
			transform.up = Player2D.instance.transform.position - transform.position;
			transform.Translate(Vector3.up * speed * Time.deltaTime);
		}
	}

	void Attack()
	{
		Instantiate(projectile); 
	}
}
