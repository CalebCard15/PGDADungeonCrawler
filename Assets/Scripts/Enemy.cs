using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Enemy : MonoBehaviour
{

	public delegate void HitPlayer(int damage);
	public static event HitPlayer GetHit;



	public float speed = 10.0f;
	public int maxHealth;
	public int health;

	public bool dead = false;

	public AudioClip[] soundEffects;


	// Use this for initialization
	protected void Start () {
		
	}



	
	// Update is called once per frame
	void Update () {

	}

	public virtual void Move()
	{

	}

	public void TakeDamage(int damage)
	{
		
		health -= damage;
		UIManager.instance.EnemyHealthSet(this);
		if(health <= 0 && dead == false)
		{
			GetComponent<AudioSource>().clip = soundEffects[1];
			GetComponent<AudioSource>().Play();
			dead = true;
			UIManager.instance.EnemyHealthSet(this);
			StartCoroutine(Die());
		}


	}

	protected virtual void Hit(int damage)
	{
		GetComponent<AudioSource>().clip = soundEffects[0];
		GetComponent<AudioSource>().Play();	
		HitPlayer hit = GetHit;
		if(hit != null)
		{
			hit(damage);
		}
	}

	public IEnumerator Die()
	{
		yield return new WaitForSeconds(.7f);
		Destroy(this.gameObject);
	}



}
