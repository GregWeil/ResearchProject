using UnityEngine;
using System.Collections.Generic;

using StateMachineUtilities;

public class StateMachine : MonoBehaviour, ISerializationCallbackReceiver {

    public List<Parameter> parameters = new List<Parameter>();
    public List<State> states = new List<State>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    // Serialization

    public void OnBeforeSerialize() {
        foreach (var param in parameters) {
            param.serializedType = param.type.AssemblyQualifiedName;
            param.serializedValue = Serialization.serializeObject(param.value, param.type);
        }
        foreach (var state in states) {
            foreach (var transition in state.transitions) {
                transition.serializedTo = states.IndexOf(transition.to);
                foreach (var condition in transition.conditions) {
                    condition.serializedMethodType = condition.method.ReflectedType.AssemblyQualifiedName;
                    condition.serializedMethodName = condition.method.Name;
                    foreach (var argument in condition.arguments) {
                        if (argument.style == Argument.Style.Constant) {
                            argument.serializedValue = Serialization.serializeObject(argument.value, argument.param.ParameterType);
                        } else if (argument.style == Argument.Style.Parameter) {
                            argument.serializedValue = parameters.IndexOf((Parameter)argument.value).ToString();
                        }
                    }
                }
            }
        }
    }

    public void OnAfterDeserialize() {
        foreach (var param in parameters) {
            param.type = System.Type.GetType(param.serializedType);
            param.value = Serialization.deserializeObject(param.serializedValue, param.type);
        }
        foreach (var state in states) {
            foreach (var transition in state.transitions) {
                transition.to = states[transition.serializedTo];
                transition.from = state;
                foreach (var condition in transition.conditions) {
                    condition.method = System.Type.GetType(condition.serializedMethodType).GetMethod(condition.serializedMethodName);
                    var methodArguments = condition.method.GetParameters();
                    for (var i = 0; i < methodArguments.Length; ++i) {
                        condition.arguments[i].param = methodArguments[i];
                    }
                    foreach (var argument in condition.arguments) {
                        if (argument.style == Argument.Style.Constant) {
                            argument.value = Serialization.deserializeObject(argument.serializedValue, argument.param.ParameterType);
                        } else if (argument.style == Argument.Style.Parameter) {
                            argument.value = parameters[int.Parse(argument.serializedValue)];
                        }
                    }
                }
            }
        }
    }
}
