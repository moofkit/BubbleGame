using UnityEngine;
using System.Collections;

public class BubbleBlastController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.transform.particleSystem.duration == gameObject.transform.particleSystem.time){
			Destroy(gameObject);
		}
	}
}
