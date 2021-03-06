﻿using UnityEngine;
using System.Collections;

public class MeleeEnemy : Enemy {



	public float attackSpeed = 1.5f;
	public float timeSinceAttack = 0.0f;

	private bool isAttacking;
	private Animator animator;

	// Use this for initialization
	void Start () {
		base.Start();
		isAttacking = false;
		maxHealth = 100;
		health = maxHealth;

		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceAttack += Time.deltaTime;
		Move();
	}

	public override void Move() 
	{
		
		if(Vector3.Distance(transform.position, Player2D.instance.transform.position) < 10 && !isAttacking)
		{
			if(dead)
				transform.up = transform.position - transform.position;
			else
				transform.up = Player2D.instance.transform.position - transform.position;
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
			Attack();
		}
	}
		

	void OnCollisionStay2D(Collision2D col)
	{
		if(col.transform.tag == "Player")
		{
			Attack();
		}
	}

	public void Attack()
	{
		
		if(timeSinceAttack >= attackSpeed)
		{
			isAttacking = true;
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
		isAttacking = false;
		animator.SetBool("IsAttacking", false);
	}
}
