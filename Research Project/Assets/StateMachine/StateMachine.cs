using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using StateMachineUtilities;

public class StateMachine : MonoBehaviour, ISerializationCallbackReceiver {

    public List<Parameter> parameters = new List<Parameter>();
    public List<State> states = new List<State>();

    [System.NonSerialized]
    public State initialState = null;
    public int serializedInitialState;

    [System.NonSerialized]
    private State state = null;

    public List<Method> serializedFilters = new List<Method>();
    public List<Object> serializedObjects = new List<Object>();


	// Use this for initialization
	void Start () {
        state = initialState;
	}
	

    private void evaluateAction(Method action) {
        action.method.Invoke(null, action.arguments.Select(arg => evaluateArgument(arg)).ToArray());
    }

    private object evaluateArgument(Argument argument) {
        if (argument.style == Argument.Style.Constant) {
            return argument.value;
        } else if (argument.style == Argument.Style.Parameter) {
            return ((Parameter)argument.value).value;
        } else if (argument.style == Argument.Style.Filter) {
            return evaluateFilter((Method)argument.value);
        }
        return null;
    }

    private object evaluateFilter(Method filter) {
        var arguments = filter.arguments.Select(arg => evaluateArgument(arg)).ToArray();
        return filter.method.Invoke(null, arguments);
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (state != null) {
            foreach (var action in state.actions) {
                evaluateAction(action);
            }
            foreach (var transition in state.transitions) {
                if (transition.conditions.Aggregate(true, (accumulate, cond) => ((bool)evaluateFilter(cond) && accumulate))) {
                    state = transition.to;
                    Debug.Log(state.name);
                    break;
                }
            }
        }
	}


    // Serialization

    private void OnBeforeSerializeMethod(Method method) {
        method.serializedMethodType = method.method.ReflectedType.AssemblyQualifiedName;
        method.serializedMethodName = method.method.Name;
        foreach (var argument in method.arguments) {
            if (argument.style == Argument.Style.Constant) {
                argument.serializedValue = Serialization.serializeObject(this, argument.value, argument.param.ParameterType);
            } else if (argument.style == Argument.Style.Parameter) {
                argument.serializedValue = parameters.IndexOf((Parameter)argument.value).ToString();
            } else if (argument.style == Argument.Style.Filter) {
                argument.serializedValue = serializedFilters.Count.ToString();
                serializedFilters.Add((Method)argument.value);
                OnBeforeSerializeMethod((Method)argument.value);
            }
        }
    }

    private void OnAfterDeserializeMethod(Method method) {
        method.method = System.Type.GetType(method.serializedMethodType).GetMethod(method.serializedMethodName);
        var methodArguments = method.method.GetParameters();
        for (var i = 0; i < methodArguments.Length; ++i) {
            method.arguments[i].param = methodArguments[i];
        }
        foreach (var argument in method.arguments) {
            if (argument.style == Argument.Style.Constant) {
                argument.value = Serialization.deserializeObject(this, argument.serializedValue, argument.param.ParameterType);
            } else if (argument.style == Argument.Style.Parameter) {
                argument.value = parameters[int.Parse(argument.serializedValue)];
            } else if (argument.style == Argument.Style.Filter) {
                argument.value = serializedFilters[int.Parse(argument.serializedValue)];
                OnAfterDeserializeMethod((Method)argument.value);
            }
        }
    }

    public void OnBeforeSerialize() {
        serializedFilters.Clear();
        serializedInitialState = states.IndexOf(initialState);
        foreach (var param in parameters) {
            param.serializedType = param.type.AssemblyQualifiedName;
            param.serializedValue = Serialization.serializeObject(this, param.value, param.type);
        }
        foreach (var state in states) {
            foreach (var action in state.actions) {
                OnBeforeSerializeMethod(action);
            }
            foreach (var transition in state.transitions) {
                transition.serializedTo = states.IndexOf(transition.to);
                foreach (var condition in transition.conditions) {
                    OnBeforeSerializeMethod(condition);
                }
            }
        }
    }

    public void OnAfterDeserialize() {
        if (serializedInitialState >= 0) {
            initialState = states[serializedInitialState];
        }
        foreach (var param in parameters) {
            param.type = System.Type.GetType(param.serializedType);
            param.value = Serialization.deserializeObject(this, param.serializedValue, param.type);
        }
        foreach (var state in states) {
            foreach (var action in state.actions) {
                OnAfterDeserializeMethod(action);
            }
            foreach (var transition in state.transitions) {
                transition.to = states[transition.serializedTo];
                transition.from = state;
                foreach (var condition in transition.conditions) {
                    OnAfterDeserializeMethod(condition);
                }
            }
        }
    }
}
