using UnityEngine;
using System.Collections;

public class PlayerCameraAnchor : MonoBehaviour {

    Transform player = null;

    float ySpeed = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (player != null) {
            var position = (player.position + Vector3.up);
            if (!player.GetComponentInChildren<PlayerMovement>().getGrounded()) {
                position.y = transform.position.y;
            }
            position.y = Mathf.SmoothDamp(transform.position.y, position.y, ref ySpeed, 0.2f);
            transform.position = position;
        } else {
            var playerObj = GameObject.Find("Player");
            if (playerObj != null) {
                player = playerObj.transform;
                transform.position = player.position;
            }
        }
	}
}
