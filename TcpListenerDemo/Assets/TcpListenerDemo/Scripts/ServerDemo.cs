using UnityEngine;
using System.Collections;

public class ServerDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TcpListenerHelper.Instance.StartListen ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
