using UnityEngine;
using StateMachineUtilities;

public class VectorFilters : Modules.Module {

    [Modules.Method("Vectors/vec3 magnitude")]
    public static float vec3Magnitude(Vector3 vector) {
        return vector.magnitude;
    }

    [Modules.Method("Vectors/vec2 magnitude")]
    public static float vec2Magnitude(Vector2 vector) {
        return vector.magnitude;
    }

    [Modules.Method("Vectors/vec3 distance")]
    public static float vec3Distance(Vector3 a, Vector3 b) {
        return Vector3.Distance(a, b);
    }

    [Modules.Method("Vectors/vec3 multiply")]
    public static Vector3 vec3Multiply(Vector3 a, Vector3 b) {
        return Vector3.Scale(a, b);
    }

    [Modules.Method("Vectors/vec3 difference")]
    public static Vector3 vec3Difference(Vector3 a, Vector3 b) {
        return (a - b);
    }

    [Modules.Method("Vectors/vec3 addition")]
    public static Vector3 vec3Add(Vector3 a, Vector3 b) {
        return (a + b);
    }

    [Modules.Method("Vectors/vec3 normalize")]
    public static Vector3 vec3Normalize(Vector3 vector) {
        return vector.normalized;
    }

}

public class GameObjectFilters : Modules.Module {

    [Modules.Method("GameObject/exists")]
    public static bool exists(GameObject obj) {
        return (obj != null);
    }

    [Modules.Method("GameObject/get position")]
    public static Vector3 position(GameObject obj) {
        return obj.transform.position;
    }

    [Modules.Method("GameObject/distance to")]
    public static float distance(GameObject a, GameObject b) {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

}
