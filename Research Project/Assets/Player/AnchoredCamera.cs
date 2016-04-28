using UnityEngine;
using System.Collections;

public class AnchoredCamera : MonoBehaviour {

    public Transform target = null;

    float distMin = 5f;
    float distMax = 30f;

    float distance = 20f;
    float angle = 0f;

	// Use this for initialization
	void Start () {
	    
	}
	
    void SetPosition() {
        transform.position = target.position;
        transform.eulerAngles = new Vector3(30, angle, 0);
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
        SetPosition();
	}

    void OnValidate() {
        SetPosition();
    }
}
