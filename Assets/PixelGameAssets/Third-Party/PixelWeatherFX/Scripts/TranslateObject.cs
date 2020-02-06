using UnityEngine;
using System.Collections;

public class TranslateObject : MonoBehaviour {
	
	void Update() {
		transform.position = new Vector3(Mathf.PingPong(Time.time*2, 14)-7, transform.position.y, transform.position.z);
	}

}
