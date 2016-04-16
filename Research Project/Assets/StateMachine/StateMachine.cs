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
