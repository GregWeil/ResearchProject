using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour {

    [System.Serializable]
    public class State {
        public string name = "State";
        public Vector2 editorPosition = Vector2.zero;
    }

    [System.Serializable]
    public class Transition {
        public State from = null;
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
}
