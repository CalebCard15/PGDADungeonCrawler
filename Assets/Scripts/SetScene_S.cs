using UnityEngine;
using System.Collections;

public class SetScene_S : MonoBehaviour
{
	public Light MainLight;
	// Use this for initialization
	void Start () 
	{
		MainLight.intensity = Random.Range(.35f, 2.76f);
		MainLight.transform.Rotate(Random.Range(-90, 91), Random.Range(-90, 91), 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
