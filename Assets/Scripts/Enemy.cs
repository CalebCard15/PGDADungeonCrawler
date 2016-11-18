using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Enemy : MonoBehaviour
{

	public delegate void HitPlayer(int damage);
	public static event HitPlayer GetHit;

	public GameObject player;

	public float speed = 10.0f;
	public int health;

	bool dead = false;

	public AudioClip[] soundEffects;


	// Use this for initialization
	protected void Start () {
		player = GameObject.Find("Player");
	}



	
	// Update is called once per frame
	void Update () {

	}

	public virtual void Move()
	{

	}

	public void TakeDamage(int damage)
	{
		if(health - damage <= 0 && dead == false)
		{
			GetComponent<AudioSource>().clip = soundEffects[1];
			GetComponent<AudioSource>().Play();
			dead = true;
			player = this.gameObject;
			StartCoroutine(Die());
		}

		health -= damage;
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
