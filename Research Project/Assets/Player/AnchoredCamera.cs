using UnityEngine;
using System.Collections;

public class AnchoredCamera : MonoBehaviour {

    public Transform target = null;

    float distMin = 5f;
    float distMax = 30f;

    float distance = 20f;
    float angle = 0f;

    float tilt = 0f;
    float tiltSpeed = 0f;
    float tiltNeutral = 30f;

    float lockTime = -1f;
    float lockPriority = float.MinValue;
    float lockTilt = 0f;

    void Start () {
        tilt = tiltNeutral;
    }

	
    void SetPosition() {
        transform.position = target.position;
        transform.eulerAngles = new Vector3(tilt, angle, 0);
        transform.Translate(new Vector3(0, 0, -distance));
    }

	// Update is called once per frame
    void Update () {
        angle += (100 * Input.GetAxis("Camera X") * Time.deltaTime);
        distance += (25 * Input.GetAxis("Camera Y") * Time.deltaTime);

        if (Input.GetMouseButton(1)) {
            angle += (10 * Input.GetAxis("Mouse X"));
        }
        distance -= (10 * Input.GetAxis("Mouse ScrollWheel"));

        distance = Mathf.Clamp(distance, distMin, distMax);

        float tiltGoal = (lockTime > 0f) ? lockTilt : tiltNeutral;
        tilt = Mathf.SmoothDamp(tilt, tiltGoal, ref tiltSpeed, 0.4f);

        lockTime -= Time.deltaTime;
        if (lockTime < 0) lockPriority = float.MinValue;
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


    public void lockCamera(float tilt, float priority = 0f, float time = 0.1f) {
        if (priority >= lockPriority) {
            lockTilt = tilt;
            lockTime = time;
            lockPriority = priority;
        }
    }
}
