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

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Parameters");
        
        foreach (var param in machine.parameters) {
            EditorGUILayout.BeginHorizontal();
            param.name = EditorGUILayout.TextField(param.name);
            if (param.type == typeof(bool)) {
                param.value = EditorGUILayout.Toggle((bool)param.value);
            } else if (param.type == typeof(float)) {
                param.value = EditorGUILayout.FloatField((float)param.value);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add")) {
            machine.parameters.Add(new StateMachine.Parameter());
        }
        if (GUILayout.Button("Remove")) {
            machine.parameters.RemoveAt(machine.parameters.Count-1);
        }
        EditorGUILayout.EndHorizontal();
    }
}
