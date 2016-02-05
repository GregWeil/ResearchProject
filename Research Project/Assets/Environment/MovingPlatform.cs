using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    Rigidbody body = null;
    Vector3 basePos = Vector3.zero;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        basePos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        body.MovePosition(basePos + 0.5f * Vector3.forward * Time.time);
        body.MoveRotation(body.rotation * Quaternion.AngleAxis(10f * Time.fixedDeltaTime, Vector3.up));
    }
}
