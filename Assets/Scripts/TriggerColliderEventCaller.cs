using UnityEngine;
using UnityEngine.Events;

public class TriggerColliderEventCaller : MonoBehaviour 
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

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (specificCollider == true)//if we are looking for a specific collider...
		{
			foreach (string tag in colliderTagsToReactTo) //look through our list of specific colliders
			{
				//if the tag of The collider we hit is the same as any of the tags in our array....
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
