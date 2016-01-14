using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed = 7.5f;
    public float acceleration = 100.0f;

    public float jumpVelocity = 15.0f;
    public float gravity = -50.0f;

    CharacterController controller = null;
    Vector3 movement = Vector3.zero;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        movement.y += gravity * Time.deltaTime;
        if (controller.isGrounded && Input.GetButton("Jump")) {
            movement.y += jumpVelocity;
        }

        Vector2 goalMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 currentMovement = Vector2.MoveTowards(new Vector2(movement.x, movement.z),
            (goalMovement * moveSpeed), (acceleration * Time.deltaTime));
        movement = new Vector3(currentMovement.x, movement.y, currentMovement.y);

        controller.Move(movement * Time.deltaTime);
        if (controller.isGrounded) movement.y = 0.0f;
	}
}
