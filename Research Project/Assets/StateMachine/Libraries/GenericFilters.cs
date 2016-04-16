using UnityEngine;
using System.Collections;

public class GenericFilters : StateMachineModule {

    [StateMachineMethod("is true")]
    public static bool isTrue(bool value) {
        return value;
    }

    [StateMachineMethod("greater than")]
    public static bool isGreaterThan(float a, float b) {
        return (a > b);
    }
}
