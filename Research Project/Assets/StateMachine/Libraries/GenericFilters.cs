using UnityEngine;
using StateMachineUtilities;

public class GenericFilters : Module {

    [Method("is true")]
    public static bool isTrue(bool value) {
        return value;
    }

    [Method("greater than")]
    public static bool isGreaterThan(float a, float b) {
        return (a > b);
    }

    [Method("longer than")]
    public static bool isLongerThan(Vector3 vector, float length) {
        return (vector.magnitude > length);
    }

    [Method("vec3 magnitude")]
    public static float vec3Magnitude(Vector3 vector) {
        return vector.magnitude;
    }

}
