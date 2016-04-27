using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    [SerializeField]
    private bool locked = false;

    HingeJoint hinge = null;
    GameObject objLocked = null;
    GameObject objUnlocked = null;

	// Use this for initialization
	void Start () {
        hinge = GetComponentInChildren<HingeJoint>();
        objLocked = transform.Find("Door").Find("Locked").gameObject;
        objUnlocked = transform.Find("Door").Find("Unlocked").gameObject;
        setDoor(locked);
    }


    public void lockDoor() {
        setDoor(true);
    }

    public void unlockDoor() {
        setDoor(false);
    }

    public void setDoor(bool newLocked) {
        locked = newLocked;
        hinge.useSpring = locked;
        objLocked.SetActive(locked);
        objUnlocked.SetActive(!locked);
    }


    void OnValidate() {
        Start();
    }
}
