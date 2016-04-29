using StateMachineUtilities;

public class BooleanFilters : Modules.Module {

    [Modules.Method("Boolean/is true")]
    public static bool isTrue(bool value) {
        return (value == true);
    }

    [Modules.Method("Boolean/is false")]
    public static bool isFalse(bool value) {
        return (value == false);
    }

    [Modules.Method("Boolean/not")]
    public static bool not(bool value) {
        return !value;
    }

    [Modules.Method("Boolean/equal")]
    public static bool isEqual(bool a, bool b) {
        return (a == b);
    }

    [Modules.Method("Boolean/not equal")]
    public static bool isNotEqual(bool a, bool b) {
        return (a != b);
    }

}

public class MathFilters : Modules.Module {

    [Modules.Method("Math/greater than")]
    public static bool isGreaterThan(float a, float b) {
        return (a > b);
    }

    [Modules.Method("Math/less than")]
    public static bool isLessThan(float a, float b) {
        return (a < b);
    }

    [Modules.Method("Math/greater than or equal")]
    public static bool isGreaterThanOrEqualTo(float a, float b) {
        return (a >= b);
    }

    [Modules.Method("Math/less than or equal")]
    public static bool isLessThanOrEqualTo(float a, float b) {
        return (a <= b);
    }

    [Modules.Method("Math/equal")]
    public static bool isEqual(float a, float b) {
        return (a == b);
    }

    [Modules.Method("Math/not equal")]
    public static bool isNotEqual(float a, float b) {
        return (a != b);
    }
    
}
