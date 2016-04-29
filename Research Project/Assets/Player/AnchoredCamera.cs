using UnityEngine;
using System.Collections;

public class AnchoredCamera : MonoBehaviour {

    public Transform target = null;

    float distMin = 5f;
    float distMax = 30f;

    float distance = 20f;
    float angle = 0f;

    float lockTime = -1f;
    Vector3 lockPosition = Vector3.zero;
    Quaternion lockRotation = Quaternion.identity;
	
    void SetPosition() {
        if (lockTime < 0f) {
            transform.position = target.position;
            transform.eulerAngles = new Vector3(30, angle, 0);
            transform.Translate(new Vector3(0, 0, -distance));
        } else {
            transform.position = lockPosition;
            transform.rotation = lockRotation;
        }
    }

	// Update is called once per frame
    void Update () {
        if (lockTime < 0f) {
            angle += (100 * Input.GetAxis("Camera X") * Time.deltaTime);
            distance += (25 * Input.GetAxis("Camera Y") * Time.deltaTime);

            if (Input.GetMouseButton(1)) {
                angle += (10 * Input.GetAxis("Mouse X"));
            }
            distance -= (10 * Input.GetAxis("Mouse ScrollWheel"));

            distance = Mathf.Clamp(distance, distMin, distMax);
        }
        lockTime -= Time.deltaTime;
    }

    void LateUpdate () {
        SetPosition();
    }

	void OnPreCull () {
        SetPosition();
	}

    void OnValidate() {
        SetPosition();
    }

    public void lockCamera(Vector3 position, Quaternion rotation, float time = 0f) {
        lockPosition = position;
        lockRotation = rotation;
        lockTime = time;
    }
}
