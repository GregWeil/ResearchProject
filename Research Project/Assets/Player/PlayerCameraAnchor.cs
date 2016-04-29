using UnityEngine;
using System.Collections;

public class PlayerCameraAnchor : MonoBehaviour {

    Transform player = null;

    Vector3 speed = Vector3.zero;

    float hTime = 0.05f;
    float vTime = 0.2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
	    if (player != null) {
            var position = (player.position + (1.5f * Vector3.up));
            if (!player.GetComponentInChildren<PlayerMovement>().getGrounded()) {
                position.y = transform.position.y;
            }
            position.x = Mathf.SmoothDamp(transform.position.x, position.x, ref speed.x, hTime);
            position.y = Mathf.SmoothDamp(transform.position.y, position.y, ref speed.y, vTime);
            position.z = Mathf.SmoothDamp(transform.position.z, position.z, ref speed.z, hTime);
            transform.position = position;
        } else {
            transform.position += (speed * Time.deltaTime);
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) {
                player = playerObj.transform;
                transform.position = player.position;
                speed = Vector3.zero;
            }
        }
	}
}
