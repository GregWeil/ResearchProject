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
        setLocked(locked);
    }


    public void lockDoor() {
        setLocked(true);
    }

    public void unlockDoor() {
        setLocked(false);
    }

    public void setLocked(bool newLocked) {
        locked = newLocked;
        hinge.useSpring = locked;
        objLocked.SetActive(locked);
        objUnlocked.SetActive(!locked);
    }

    public bool getLocked() {
        return locked;
    }


    void OnValidate() {
        Start();
    }
}

public class DoorModule : StateMachineUtilities.Modules.Module {

    [StateMachineUtilities.Modules.Method("Doors/is locked")]
    public static bool getLocked(Door door) {
        return door.getLocked();
    }

    [StateMachineUtilities.Modules.Method("Door/lock")]
    public static void lockDoor(Door door) {
        door.lockDoor();
    }

    [StateMachineUtilities.Modules.Method("Door/unlock")]
    public static void unlockDoor(Door door) {
        door.unlockDoor();
    }

    [StateMachineUtilities.Modules.Method("Door/set locked")]
    public static void setLocked(Door door, bool locked) {
        door.setLocked(locked);
    }

}
