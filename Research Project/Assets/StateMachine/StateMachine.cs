﻿using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour, ISerializationCallbackReceiver {

    [System.Serializable]
    public class State {
        public string name = string.Empty;
        public List<Transition> transitions = new List<Transition>();
        public Vector2 editorPosition = Vector2.zero;
    }

    [System.Serializable]
    public class Transition {
        [System.NonSerialized]
        public State from = null;
        [System.NonSerialized]
        public State to = null;

        public int toIndex = -1;
    }

    public List<State> states = new List<State>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBeforeSerialize() {
        foreach (var state in states) {
            foreach (var transition in state.transitions) {
                transition.toIndex = states.IndexOf(transition.to);
            }
        }
    }

    public void OnAfterDeserialize() {
        foreach (var state in states) {
            foreach (var transition in state.transitions) {
                transition.to = states[transition.toIndex];
                transition.from = state;
            }
        }
    }
}
