using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour, ISerializationCallbackReceiver {

    [System.Serializable]
    public class Parameter {
        public string name = string.Empty;
        [System.NonSerialized]
        public System.Type type = typeof(float);
        public object value {
            get {
                if ((internalValue == null) || (internalValue.GetType() != type)) {
                    internalValue = System.Activator.CreateInstance(type);
                }
                return internalValue;
            }
            set {
                if (value.GetType() == type) {
                    internalValue = value;
                } else {
                    throw new System.Exception("Invalid type for parameter");
                }
            }
        }

        [System.NonSerialized]
        private object internalValue = null;

        public string serializedType;
        public string serializedValue;
    }

    [System.Serializable]
    public class State {
        public string name = string.Empty;
        public List<Transition> transitions = new List<Transition>();
        public Vector2 position = Vector2.zero;
    }

    [System.Serializable]
    public class Transition {
        [System.NonSerialized]
        public State from = null;
        [System.NonSerialized]
        public State to = null;
        
        public List<Filter> conditions = new List<Filter>();

        public int serializedTo;
    }

    [System.Serializable]
    public class Filter {
        [System.NonSerialized]
        public System.Reflection.MethodInfo method = null;
        public Argument[] arguments = null;

        public string serializedMethodType;
        public string serializedMethodName;
    }

    [System.Serializable]
    public class Argument {
        public enum Style {
            Constant, Parameter, Filter
        }

        public Style style = Style.Constant;
        [System.NonSerialized]
        public System.Type type = null;
        public object value {
            get {
                if (((internalValue == null) || (internalValue.GetType() != type)) && (style == Style.Constant)) {
                    internalValue = System.Activator.CreateInstance(type);
                }
                return internalValue;
            }
            set {
                System.Type reqType = type;
                if (style == Style.Filter) {
                    type = typeof(Filter);
                } else if (style == Style.Parameter) {
                    type = typeof(Parameter);
                }
                if (value.GetType() == reqType) {
                    internalValue = value;
                } else {
                    throw new System.Exception("Invalid type for argument");
                }
            }
        }

        [System.NonSerialized]
        private object internalValue = null;

        public string serializedValue;
    }

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
            var serializer = new System.Xml.Serialization.XmlSerializer(param.type);
            var writer = new System.IO.StringWriter();
            serializer.Serialize(writer, System.Convert.ChangeType(param.value, param.type));
            param.serializedValue = writer.ToString();
        }
        foreach (var state in states) {
            foreach (var transition in state.transitions) {
                transition.serializedTo = states.IndexOf(transition.to);
                foreach (var condition in transition.conditions) {
                    condition.serializedMethodType = condition.method.ReflectedType.AssemblyQualifiedName;
                    condition.serializedMethodName = condition.method.Name;
                }
            }
        }
    }

    public void OnAfterDeserialize() {
        foreach (var param in parameters) {
            param.type = System.Type.GetType(param.serializedType);
            var serializer = new System.Xml.Serialization.XmlSerializer(param.type);
            var reader = new System.IO.StringReader(param.serializedValue);
            param.value = serializer.Deserialize(reader);
        }
        foreach (var state in states) {
            foreach (var transition in state.transitions) {
                transition.to = states[transition.serializedTo];
                transition.from = state;
                foreach (var condition in transition.conditions) {
                    condition.method = System.Type.GetType(condition.serializedMethodType).GetMethod(condition.serializedMethodName);
                }
            }
        }
    }
}
