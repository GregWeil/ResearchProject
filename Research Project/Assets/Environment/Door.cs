using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    [SerializeField]
    private bool locked = false;

    public AudioSource sound = null;
    public AudioClip soundLock = null;
    public AudioClip soundUnlock = null;

    private bool usedKey = false;

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

    public void unlockDoor(bool withKey = false) {
        usedKey = withKey;
        setLocked(false);
    }

    public void setLocked(bool newLocked) {
        if (locked != newLocked) {
            if (newLocked) sound.PlayOneShot(soundLock);
            else if (!newLocked) sound.PlayOneShot(soundUnlock);
        }
        locked = newLocked;
        hinge.useSpring = locked;
        objLocked.SetActive(locked || usedKey);
        objUnlocked.SetActive(!locked && !usedKey);
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
