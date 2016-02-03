using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    //Desired velocity
    Vector3 movement = Vector3.zero;

    //Grounded
    bool grounded = false;
    Vector3 groundNormal = Vector3.up;
    float groundAngle = 0f;

    Transform cameraTransform = null;
    Rigidbody body = null;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        cameraTransform = FindObjectOfType<Camera>().transform;
	}
	
    void FixedUpdate () {
        body.AddForce(50.0f * -groundNormal);

        //Update velocity
        Vector3 vel = new Vector3(body.velocity.x, 0.0f, body.velocity.z);
        body.AddForce(15.0f * ((5.0f * movement) - vel));

        //Rotate to face forward
        if (movement.sqrMagnitude > 0.1f) {
            body.MoveRotation(Quaternion.RotateTowards(body.rotation, Quaternion.LookRotation(movement, Vector3.up), (500.0f * Time.fixedDeltaTime)));
        }

        //Reset grounded
        grounded = false;
        if (!grounded) {
            groundNormal = Vector3.up;
            groundAngle = 180f;
        }
    }

    void OnCollisionStay (Collision col) {
        foreach (var contact in col.contacts) {
            var angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle < Mathf.Min(45f, groundAngle)) {
                groundAngle = angle;
                groundNormal = contact.normal;
                grounded = true;
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
            body.velocity += (15.0f * groundNormal);
            grounded = false;
        }

        //Die if below the map
        if (transform.position.y < 0.0f) {
            SendMessage("Damage", Time.deltaTime);
        }
	}
}
