using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

    int activity = 0;

    Rigidbody pad = null;

	// Use this for initialization
	void Start () {
        pad = transform.Find("Pad").GetComponent<Rigidbody>();
	}
	

	void FixedUpdate () {
        Vector3 padPosition = pad.transform.localPosition;
        float padHeight = getPressed() ? -0.01f : 0f;
        float padSpeed = getPressed() ? 0.1f : 0.03f;
        Debug.Log(padHeight);
        padPosition.z = Mathf.MoveTowards(padPosition.z, padHeight, (padSpeed * Time.fixedDeltaTime));
        pad.MovePosition(pad.transform.parent.TransformPoint(padPosition));
	}

    public bool getPressed() {
        return (activity > 0);
    }


    void OnTriggerEnter(Collider col) {
        if (col.gameObject.CompareTag("Player")) activity += 1;
    }

    void OnTriggerExit(Collider col) {
        if (col.gameObject.CompareTag("Player")) activity -= 1;
    }
}
