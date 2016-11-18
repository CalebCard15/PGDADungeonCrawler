using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CollissionEventCaller : MonoBehaviour 
{

	public UnityEvent callEventScript;
	public string[] colliderTagsToReactTo;
	public bool specificCollider;


	void Awake()
	{
		if (callEventScript == null) 
		{
			callEventScript = new UnityEvent ();
		}
	}

	// Use this for initialization
	void Start () 
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
	public void OnCollisionEnter2D(Collision2D col)
	{
		//if we are looking for specific colliders
		if (specificCollider == true)
		{
			//look through our list of specific colliders
			foreach (string tag in colliderTagsToReactTo) 
			{
				//if the tag of hte collider we hit is the same as any of the tags in our array....
				if (col.gameObject.tag == tag)
				{
					callEventScript.Invoke ();//plays event
				}
			}
		}
		else//this will play the event reguardless of tag if we are not looking for a specific tag.
		{
			callEventScript.Invoke ();
		}
	}
}
