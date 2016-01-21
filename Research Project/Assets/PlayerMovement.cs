using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed = 7.5f;
    public float acceleration = 100.0f;
    public float rotationSpeed = 540.0f;

    public float jumpVelocity = 15.0f;
    public float gravity = -50.0f;

    public Transform cameraTransform = null;

    CharacterController controller = null;
    Vector3 movement = Vector3.zero;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        //Apply gravity
        movement.y += gravity * Time.deltaTime;
        if (controller.isGrounded) movement.y = 0.0f;

        //Movement from input
        Vector3 moveBase = cameraTransform.TransformDirection(Vector3.right);
        moveBase.y = 0;
        moveBase.Normalize();
        Vector3 goalMovement = (Input.GetAxis("Horizontal") * moveBase) + (Input.GetAxis("Vertical") * new Vector3(-moveBase.z, 0, moveBase.x));
        Vector3 currentMovement = Vector3.MoveTowards(movement, (goalMovement * moveSpeed), (acceleration * Time.deltaTime));
        movement = new Vector3(currentMovement.x, movement.y, currentMovement.z);
        if (controller.isGrounded && Input.GetButton("Jump")) {
            movement.y += jumpVelocity;
        }

        //Rotate to face forward
        if (goalMovement.sqrMagnitude > 0.1f) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(goalMovement, Vector3.up), (rotationSpeed * Time.deltaTime));
        }

        //Apply movement
        controller.Move(movement * Time.deltaTime);
	}
}
