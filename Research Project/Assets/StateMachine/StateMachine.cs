using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour {

    [System.Serializable]
    public class State {
        public string name = "State";
        public Vector2 editorPosition = Vector2.zero;
    }

    public List<State> states = new List<State>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
