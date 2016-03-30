using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StateMachine))]
public class StateMachineInspector : Editor {

    int typeSelected = 0;
    string[] typeNames = { "Bool", "Float", "Integer" };
    System.Type[] typeTypes = { typeof(bool), typeof(float), typeof(int) };

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

        EditorGUIUtility.labelWidth = 16;
        foreach (var param in machine.parameters) {
            EditorGUILayout.BeginHorizontal();
            param.name = EditorGUILayout.TextField(param.name, GUILayout.Width(Screen.width-128));
            if (param.type == typeof(bool)) {
                param.value = EditorGUILayout.Toggle(" ", (bool)param.value);
            } else if (param.type == typeof(float)) {
                param.value = EditorGUILayout.FloatField(" ", (float)param.value);
            } else if (param.type == typeof(int)) {
                param.value = EditorGUILayout.IntField(" ", (int)param.value);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUIUtility.labelWidth = 0;

        EditorGUILayout.BeginHorizontal();
        typeSelected = EditorGUILayout.Popup(typeSelected, typeNames, GUILayout.Width(64));
        if (GUILayout.Button("Add")) {
            var param = new StateMachine.Parameter();
            param.name = "New Parameter";
            param.type = typeTypes[typeSelected];
            machine.parameters.Add(param);
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Remove")) {
            machine.parameters.RemoveAt(machine.parameters.Count-1);
        }
        EditorGUILayout.EndHorizontal();
    }
}
