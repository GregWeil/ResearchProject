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

}
