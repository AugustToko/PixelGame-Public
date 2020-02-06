using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {
	public bool rotate=true;

	void Start() {
		rotate = true;
	}

	// Update is called once per frame
	void Update () {
		if(rotate == true){
			transform.Rotate ( Vector3.up * ( 7 * Time.deltaTime ) );
		}
	}
}
