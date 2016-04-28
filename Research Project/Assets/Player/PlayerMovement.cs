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
    Animator anim = null;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        cameraTransform = FindObjectOfType<Camera>().transform;
        anim = GetComponentInChildren<Animator>();
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
        body.AddForce(60f * -groundNormal);

        //Jump hover
        if (Input.GetButton("Jump") && !grounded && (body.velocity.y > -2.5f)) {
            body.AddForce(30.0f * Vector3.up);
        }

        //Movement velocity
        Vector3 vel = new Vector3(body.velocity.x, 0.0f, body.velocity.z);
        Vector3 groundVel = Vector3.zero;
        if (groundBody != null) {
            groundVel += groundBody.GetPointVelocity(groundContact);
            groundVel.y = 0f;
        }
        if (grounded) {
            Vector3 goalVel = (5.5f * movement * Mathf.Cos(groundAngle * Mathf.Deg2Rad));
            goalVel += groundVel;
            var accel = (0.5f * (goalVel - vel) / Time.fixedDeltaTime);
            body.AddForce(accel);
        } else {
            Vector3 flatSpeed = new Vector3(body.velocity.x, 0, body.velocity.z);
            body.AddForce(40.0f * movement);
            body.AddForce(-0.5f * flatSpeed);
            if (flatSpeed.magnitude > 5.5f) {
                body.AddForce(-7.5f * flatSpeed);
            }
        }

        //Rotate to face forward
        var rotGoal = body.rotation;
        if (groundBody != null) {
            rotGoal = (body.rotation * Quaternion.AngleAxis(groundBody.angularVelocity.y * Time.fixedDeltaTime * Mathf.Rad2Deg, Vector3.up));
        }
        if (movement.magnitude > 0.1f) {
            rotGoal = (Quaternion.RotateTowards(body.rotation, Quaternion.LookRotation(movement, Vector3.up), (500.0f * Time.fixedDeltaTime)));
        }
        Vector3 rotDelta = (rotGoal * Quaternion.Inverse(body.rotation)).eulerAngles;
        if (rotDelta.x > 180f) rotDelta.x -= 360f;
        if (rotDelta.y > 180f) rotDelta.y -= 360f;
        if (rotDelta.z > 180f) rotDelta.z -= 360f;
        body.angularVelocity = rotDelta;

        //Update animation
        anim.SetBool("Grounded", grounded);
        anim.SetFloat("SpeedGround", (vel - groundVel).magnitude);
        anim.SetFloat("SpeedVertical", body.velocity.y);

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
            if (angle <= Mathf.Min(46f, groundAngle)) {
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

        if (grounded) {
            anim.SetBool("Grounded", grounded);
        }
	}

    public bool getGrounded() {
        return grounded;
    }
}
