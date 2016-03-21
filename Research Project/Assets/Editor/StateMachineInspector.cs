using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StateMachine))]
public class StateMachineInspector : Editor {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    public override void OnInspectorGUI() {
        //Uncomment for debugging
        //base.OnInspectorGUI();

        var machine = (StateMachine)target;

        if (GUILayout.Button("Open Editor")) {
            StateMachineEditor.ShowWithTarget(machine);
        }
    }
}
