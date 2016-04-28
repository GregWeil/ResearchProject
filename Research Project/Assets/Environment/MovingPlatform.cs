using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public MovingPlatformWaypoint target = null;

    float speed = 5f;

    Rigidbody body = null;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (target != null) {
            if (target.next != null) {
                if (transform.position == target.transform.position) {
                    if (transform.rotation == target.transform.rotation) {
                        target = target.next;
                    }
                }
            }
            var fromPos = transform.position;
            var toPos = target.transform.position;
            var newPos = Vector3.MoveTowards(fromPos, toPos, (speed * Time.fixedDeltaTime));
            var prop = Vector3.Distance(fromPos, newPos) / Vector3.Distance(fromPos, toPos);
            var newRot = Quaternion.Lerp(transform.rotation, target.transform.rotation, prop);
            body.MovePosition(newPos);
            body.MoveRotation(newRot);
        }
    }
}
