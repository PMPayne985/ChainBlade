using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
    public float w = 30;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * w * Time.deltaTime,Space.Self);
	}
}
