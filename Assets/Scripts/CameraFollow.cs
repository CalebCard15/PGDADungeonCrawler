using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(Player2D.instance.transform.position.x, Player2D.instance.transform.position.y, transform.position.z);
	}
}
