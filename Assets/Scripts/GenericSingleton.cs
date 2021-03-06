using UnityEngine;
using System.Collections;

public class GenericSingleton<T> : MonoBehaviour where T : Component {

	public static T instance { get; private set; }

	public virtual void Awake()
	{
		if(instance == null)
		{
			instance = this as T;
		}
		else
		{
			Destroy(gameObject);
		}
	}

}
