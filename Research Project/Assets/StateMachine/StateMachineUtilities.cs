using System;

//Inherit from this when defining conditions and actions
public class StateMachineModule { }

[AttributeUsage(AttributeTargets.Method)]
public class StateMachineMethod : Attribute {
    public readonly string name = string.Empty;

    public StateMachineMethod(string name) {
        this.name = name;
    }
}
