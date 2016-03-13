using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour, ISerializationCallbackReceiver {

    [System.Serializable]
    public class State {
        public string name = string.Empty;
        public Vector2 editorPosition = Vector2.zero;
    }

    [System.Serializable]
    public class Transition {
        public int fromIndex = -1;
        public int toIndex = -1;

        [System.NonSerialized]
        public State from = null;
        [System.NonSerialized]
        public State to = null;
    }

    public List<State> states = new List<State>();
    public List<Transition> transitions = new List<Transition>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBeforeSerialize() {
        foreach (var transition in transitions) {
            transition.fromIndex = states.IndexOf(transition.from);
            transition.toIndex = states.IndexOf(transition.to);
        }
    }

    public void OnAfterDeserialize() {
        foreach (var transition in transitions) {
            transition.from = states[transition.fromIndex];
            transition.to = states[transition.toIndex];
        }
    }
}
