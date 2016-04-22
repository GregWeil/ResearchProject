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

    public List<Filter> serializedFilters = new List<Filter>();


	// Use this for initialization
	void Start () {
        state = initialState;
	}
	

    private object evaluateArgument(Argument argument) {
        if (argument.style == Argument.Style.Constant) {
            return argument.value;
        } else if (argument.style == Argument.Style.Parameter) {
            return ((Parameter)argument.value).value;
        } else if (argument.style == Argument.Style.Filter) {
            return evaluateFilter((Filter)argument.value);
        }
        return null;
    }

    private object evaluateFilter(Filter filter) {
        var arguments = filter.arguments.Select(arg => evaluateArgument(arg)).ToArray();
        return filter.method.Invoke(null, arguments);
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (state != null) {
            Debug.Log(state.name);
            foreach (var transition in state.transitions) {
                if (transition.conditions.Aggregate(true, (accumulate, cond) => (bool)evaluateFilter(cond) && accumulate)) {
                    state = transition.to;
                    break;
                }
            }
        }
	}


    // Serialization

    private void OnBeforeSerializeFilter(Filter filter) {
        filter.serializedMethodType = filter.method.ReflectedType.AssemblyQualifiedName;
        filter.serializedMethodName = filter.method.Name;
        foreach (var argument in filter.arguments) {
            if (argument.style == Argument.Style.Constant) {
                argument.serializedValue = Serialization.serializeObject(argument.value, argument.param.ParameterType);
            } else if (argument.style == Argument.Style.Parameter) {
                argument.serializedValue = parameters.IndexOf((Parameter)argument.value).ToString();
            } else if (argument.style == Argument.Style.Filter) {
                argument.serializedValue = serializedFilters.Count.ToString();
                serializedFilters.Add((Filter)argument.value);
                OnBeforeSerializeFilter((Filter)argument.value);
            }
        }
    }

    private void OnAfterDeserializeFilter(Filter filter) {
        filter.method = System.Type.GetType(filter.serializedMethodType).GetMethod(filter.serializedMethodName);
        var methodArguments = filter.method.GetParameters();
        for (var i = 0; i < methodArguments.Length; ++i) {
            filter.arguments[i].param = methodArguments[i];
        }
        foreach (var argument in filter.arguments) {
            if (argument.style == Argument.Style.Constant) {
                argument.value = Serialization.deserializeObject(argument.serializedValue, argument.param.ParameterType);
            } else if (argument.style == Argument.Style.Parameter) {
                argument.value = parameters[int.Parse(argument.serializedValue)];
            } else if (argument.style == Argument.Style.Filter) {
                argument.value = serializedFilters[int.Parse(argument.serializedValue)];
                OnAfterDeserializeFilter((Filter)argument.value);
            }
        }
    }

    public void OnBeforeSerialize() {
        serializedFilters.Clear();
        foreach (var param in parameters) {
            param.serializedType = param.type.AssemblyQualifiedName;
            param.serializedValue = Serialization.serializeObject(param.value, param.type);
        }
        foreach (var state in states) {
            foreach (var transition in state.transitions) {
                transition.serializedTo = states.IndexOf(transition.to);
                foreach (var condition in transition.conditions) {
                    OnBeforeSerializeFilter(condition);
                }
            }
        }
        serializedInitialState = states.IndexOf(initialState);
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
                    OnAfterDeserializeFilter(condition);
                }
            }
        }
        if (serializedInitialState >= 0) {
            initialState = states[serializedInitialState];
        }
    }
}
