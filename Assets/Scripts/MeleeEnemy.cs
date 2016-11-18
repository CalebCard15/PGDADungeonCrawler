using UnityEngine;
using System.Collections;

public class MeleeEnemy : Enemy {



	public float attackSpeed = 1.5f;
	public float timeSinceAttack = 0.0f;

	private Animator animator;

	// Use this for initialization
	void Start () {
		base.Start();
		health = 100;

		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceAttack += Time.deltaTime;
		Move();
	}

	public override void Move() 
	{
		
		if(Vector3.Distance(transform.position, player.transform.position) < 10)
		{
			transform.up = player.transform.position - transform.position;
			transform.Translate(Vector3.up * 2 * Time.deltaTime);
			animator.SetBool("IsWalking", true);
		}
		else
		{
			animator.SetBool("IsWalking", false);
		}


	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.transform.tag == "Player")
		{
			Attack(player.GetComponent<Player2D>());
		}
	}
		

	void OnCollisionStay2D(Collision2D col)
	{
		if(col.transform.tag == "Player")
		{
			Attack(player.GetComponent<Player2D>());
		}
	}

	public void Attack(Player2D player)
	{
		
		if(timeSinceAttack >= attackSpeed)
		{
			animator.SetBool("IsAttacking", true);
		}
	}

	public void dealDamage()
	{
		Hit(10);
		timeSinceAttack = 0.0f;
	}

	public void ChangeAnimatorBool()
	{
		animator.SetBool("IsAttacking", false);
	}
}
