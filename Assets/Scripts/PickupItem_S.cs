using UnityEngine;
using System.Collections;

public class PickupItem_S : MonoBehaviour 
{

	public pickupItems[] pickUpItemsArray;
	int i;
	string functionNameToCall;
	 
	// Use this for initialization
	void Start () 
	{
		i = Random.Range(0, pickUpItemsArray.Length+1);
		//GetComponent<SpriteRenderer>().sprite = pickUpItemsArray[i].itemSprt;
		//functionNameToCall = pickUpItemsArray[i].functionName;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}


	void OnTriggerEnter2D(Collider2D col)
	{

		if(col.transform.tag == "Player" )
		{
			switch(functionNameToCall)
			{
			case "health":
				Debug.Log("get some health");
				Player2D.instance.health += 15;
				if(Player2D.instance.health > Player2D.instance.maxHealth)
					Player2D.instance.health = Player2D.instance.maxHealth;
				GetComponent<CircleCollider2D>().enabled = false;
				GetComponent<SpriteRenderer>().enabled = false;

				if(pickUpItemsArray[i].pickupEffect != null)
				{
					Instantiate(pickUpItemsArray[i].pickupEffect, transform.position, transform.rotation);
				}
				break;

			case "RiseShootingSpeed":
				Debug.Log("shootFaster");
				break;

			case "gun":
				Debug.Log("GetGun");
				break;

			case "SpeedUp":
				Debug.Log("goFaster");
				break;
			}
		}
	}

	[System.Serializable]
	public struct pickupItems//sets up a struct(visible in the inpsector thanks to the above line) to deffine each terrain type
	{
		public Sprite itemSprt;
		public string functionName;
		public GameObject pickupEffect;

	}
}
