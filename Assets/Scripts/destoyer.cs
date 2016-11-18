using UnityEngine;
using System.Collections;

public class destoyer : MonoBehaviour
{
	public float timeTillDestroy;
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(CountDownToDestroy());
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public IEnumerator CountDownToDestroy()
	{
		yield return new WaitForSeconds(timeTillDestroy);
		Destroy(this.gameObject);
			
	}
}
