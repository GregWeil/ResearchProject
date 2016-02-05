using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    //Desired velocity
    Vector3 movement = Vector3.zero;

    //Grounded
    bool grounded = false;
    Vector3 groundContact = Vector3.zero;
    Vector3 groundNormal = Vector3.up;
    float groundAngle = 0f;
    GameObject groundObject = null;

    Transform cameraTransform = null;
    Rigidbody body = null;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        cameraTransform = FindObjectOfType<Camera>().transform;
	}
	
    void FixedUpdate () {
        Rigidbody groundBody = null;
        if (!grounded) {
            groundContact = Vector3.zero;
            groundNormal = Vector3.up;
            groundAngle = 0f;
        } else {
            groundBody = groundObject.GetComponent<Rigidbody>();
        }

        //Apply gravity
        body.AddForce(50f * -groundNormal);

        //Update velocity
        Vector3 vel = new Vector3(body.velocity.x, 0.0f, body.velocity.z);
        Vector3 goalVel = (5.0f * movement * 1.0f * Mathf.Cos(groundAngle * Mathf.Deg2Rad));
        if (groundBody != null) {
            goalVel += groundBody.GetPointVelocity(groundContact);
        }
        float accel = grounded ? 15.0f : 10.0f;
        body.AddForce(accel * (goalVel - vel));

        //Rotate to face forward
        if (groundBody != null) {
            body.MoveRotation(body.rotation * Quaternion.Euler(groundBody.angularVelocity * Time.fixedDeltaTime * Mathf.Rad2Deg));
        }
        if (movement.magnitude > 0.1f) {
            body.MoveRotation(Quaternion.RotateTowards(body.rotation, Quaternion.LookRotation(movement, Vector3.up), (500.0f * Time.fixedDeltaTime)));
        }

        //Reset grounded
        grounded = false;
        groundContact = Vector3.zero;
        groundNormal = Vector3.up;
        groundAngle = 180f;
        groundObject = null;
    }

    void OnCollisionStay (Collision col) {
        foreach (var contact in col.contacts) {
            var angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle <= Mathf.Min(45f, groundAngle)) {
                groundContact = contact.point;
                groundNormal = contact.normal;
                groundAngle = angle;
                grounded = true;
                groundObject = col.gameObject;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        ///Update desired movement
        Vector3 moveBase = cameraTransform.TransformDirection(Vector3.right);
        moveBase.y = 0;
        moveBase.Normalize();
        movement = (Input.GetAxis("Horizontal") * moveBase) + (Input.GetAxis("Vertical") * new Vector3(-moveBase.z, 0, moveBase.x));
        if (movement.sqrMagnitude > 1.0f) movement.Normalize();
        
        if (Input.GetButtonDown("Jump") && grounded) {
            body.velocity += (15.0f * Vector3.up);
            grounded = false;
        }

        //Die if below the map
        if (transform.position.y < 0.0f) {
            SendMessage("Damage", Time.deltaTime);
        }
	}
}
