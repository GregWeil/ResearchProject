using UnityEngine;
using System.Collections;

public class OverrideCamera : MonoBehaviour {

    Transform anchor = null;

	// Use this for initialization
	void Start () {
        anchor = transform.Find("Anchor");
	}
	
	void OnTriggerStay(Collider col) {
        if (col.CompareTag("Player")) {
            GameObject.FindObjectOfType<AnchoredCamera>().lockCamera(anchor.position, anchor.rotation, 0.25f);
        }
    }
}
