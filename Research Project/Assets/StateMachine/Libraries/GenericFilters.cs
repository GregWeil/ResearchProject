using UnityEngine;
using StateMachineUtilities;

public class GenericFilters : Modules.Module {

    [Modules.Method("is true")]
    public static bool isTrue(bool value) {
        return value;
    }

    [Modules.Method("greater than")]
    public static bool isGreaterThan(float a, float b) {
        return (a > b);
    }

    [Modules.Method("longer than")]
    public static bool isLongerThan(Vector3 vector, float length) {
        return (vector.magnitude > length);
    }

    [Modules.Method("vec3 magnitude")]
    public static float vec3Magnitude(Vector3 vector) {
        return vector.magnitude;
    }

}
