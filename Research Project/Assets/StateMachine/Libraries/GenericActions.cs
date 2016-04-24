using UnityEngine;
using StateMachineUtilities;

public class GenericActions : Modules.Module {

    [Modules.Method("debug output")]
    public static void debugOutput(object value) {
        Debug.Log(value);
    }

}
