using UnityEngine;
using System.Collections;

public class OverrideCamera : MonoBehaviour {

    public float angle = 60f;
    public float priority = 0f;

	// Use this for initialization
	void Start () {

	}
	
	void OnTriggerStay(Collider col) {
        if (col.CompareTag("Player")) {
            GameObject.FindObjectOfType<AnchoredCamera>().lockCamera(angle, priority);
        }
    }
}
