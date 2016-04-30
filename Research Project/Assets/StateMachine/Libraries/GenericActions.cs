using UnityEngine;
using StateMachineUtilities;

public class GenericActions : Modules.Module {

    [Modules.Method("debug output")]
    public static void debugOutput(object value) {
        Debug.Log(value);
    }

    [Modules.Method("GameObject/set active")]
    public static void setActive(GameObject obj, bool active) {
        obj.SetActive(active);
    }

    [Modules.Method("Audio/play sound")]
    public static void playSound(AudioSource sound) {
        sound.Play();
    }

}
