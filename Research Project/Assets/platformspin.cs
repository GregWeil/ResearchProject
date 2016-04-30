using UnityEngine;
using System.Collections;

public class platformspin : MonoBehaviour {
    public Vector3 axis = Vector3.up;
    public float speed = 30f;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        GetComponent<Rigidbody>().MoveRotation(transform.rotation * Quaternion.AngleAxis(speed * Time.fixedDeltaTime, axis));
    }
}
