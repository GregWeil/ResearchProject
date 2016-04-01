using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(StateMachine))]
public class StateMachineInspector : Editor {
    
    int typeSelected = 0;
    string[] typeNames = { "Bool", "Float", "Integer", "Vector2", "Vector3" };
    System.Type[] typeTypes = { typeof(bool), typeof(float), typeof(int), typeof(Vector2), typeof(Vector3) };
    
    ReorderableList parameterGUI;
    
	// Use this for initialization
	void OnEnable() {
        var machine = (StateMachine)target;

        parameterGUI = new ReorderableList(machine.parameters, typeof(StateMachine.Parameter));
        parameterGUI.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Parameters");
        };
        parameterGUI.drawElementCallback = (Rect rect, int index, bool active, bool focused) => {
            var parameter = (StateMachine.Parameter)parameterGUI.list[index];
            Rect rLabel = new Rect(rect.x, rect.y, rect.width / 2, rect.height);
            if (active) {
                parameter.name = EditorGUI.TextField(rLabel, parameter.name);
            } else {
                EditorGUI.LabelField(rLabel, parameter.name);
            }
            Rect rValue = new Rect(rLabel.xMax, rect.y, rect.xMax - rLabel.xMax, rect.height);
            if (parameter.type == typeof(bool)) {
                parameter.value = EditorGUI.Toggle(rValue, (bool)parameter.value);
            } else if (parameter.type == typeof(float)) {
                parameter.value = EditorGUI.FloatField(rValue, (float)parameter.value);
            } else if (parameter.type == typeof(int)) {
                parameter.value = EditorGUI.IntField(rValue, (int)parameter.value);
            } else if (parameter.type == typeof(Vector2)) {
                GUILayout.Space(EditorGUIUtility.labelWidth);
                parameter.value = EditorGUI.Vector2Field(rValue, GUIContent.none, (Vector2)parameter.value);
            } else if (parameter.type == typeof(Vector3)) {
                GUILayout.Space(EditorGUIUtility.labelWidth);
                parameter.value = EditorGUI.Vector3Field(rValue, GUIContent.none, (Vector3)parameter.value);
            } else if (parameter.type == typeof(GameObject)) {
                parameter.value = EditorGUI.ObjectField(rValue, (GameObject)parameter.value, typeof(GameObject), true);
            }
        };
    }

    public override void OnInspectorGUI() {
        //Uncomment for debugging
        base.OnInspectorGUI();

        var machine = (StateMachine)target;

        if (GUILayout.Button("Open Editor")) {
            StateMachineEditor.ShowWithTarget(machine);
        }

        EditorGUILayout.Space();

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

        EditorGUILayout.Space();
        
        parameterGUI.DoLayoutList();
    }
}
