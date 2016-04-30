using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

    public AudioSource soundPress = null;
    public AudioSource soundRelease = null;

    int activity = 0;

    bool pressedLast = false;

    Transform pad = null;

	// Use this for initialization
	void Start () {
        pad = transform.Find("Pad").transform;
	}
	

    void Update () {
        if (getPressed() != pressedLast) {
            if (getPressed()) soundPress.Play();
            else soundRelease.Play();
            pressedLast = getPressed();
        }

        Vector3 padPosition = pad.transform.localPosition;
        float padHeight = getPressed() ? -0.01f : 0f;
        float padSpeed = getPressed() ? 0.1f : 0.03f;
        padPosition.z = Mathf.MoveTowards(padPosition.z, padHeight, (padSpeed * Time.deltaTime));
        pad.localPosition = padPosition;
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

public class ButtonModule : StateMachineUtilities.Modules.Module {

    [StateMachineUtilities.Modules.Method("Buttons/is held")]
    public static bool getPressed(Button button) {
        return button.getPressed();
    }

}
