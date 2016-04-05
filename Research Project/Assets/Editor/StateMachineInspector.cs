using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(StateMachine))]
public class StateMachineInspector : Editor {
    
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
            Rect rValue = new Rect(rLabel.xMax, rect.y, rect.xMax - rLabel.xMax, rect.height);
            rLabel.width -= 8;

            if (active) {
                parameter.name = EditorGUI.TextField(rLabel, parameter.name);
            } else {
                EditorGUI.LabelField(rLabel, parameter.name);
            }

            EditorGUIUtility.labelWidth = 36;
            if (parameter.type == typeof(bool)) {
                parameter.value = EditorGUI.Toggle(rValue, "Bool", (bool)parameter.value);
            } else if (parameter.type == typeof(float)) {
                parameter.value = EditorGUI.FloatField(rValue, "Float", (float)parameter.value);
            } else if (parameter.type == typeof(int)) {
                parameter.value = EditorGUI.IntField(rValue, "Int", (int)parameter.value);
            } else if (parameter.type == typeof(Vector2)) {
                GUILayout.Space(EditorGUIUtility.labelWidth);
                parameter.value = EditorGUI.Vector2Field(rValue, GUIContent.none, (Vector2)parameter.value);
            } else if (parameter.type == typeof(Vector3)) {
                GUILayout.Space(EditorGUIUtility.labelWidth);
                parameter.value = EditorGUI.Vector3Field(rValue, GUIContent.none, (Vector3)parameter.value);
            } else if (parameter.type == typeof(GameObject)) {
                parameter.value = EditorGUI.ObjectField(rValue, (GameObject)parameter.value, typeof(GameObject), true);
            }
            EditorGUIUtility.labelWidth = 0;
        };

        parameterGUI.onAddDropdownCallback = (Rect rect, ReorderableList list) => {
            var menu = new GenericMenu();
            for (var i = 0; i < typeTypes.Length; ++i) {
                var type = typeTypes[i];
                menu.AddItem(new GUIContent(typeNames[i]), false, () => {
                    var param = new StateMachine.Parameter();
                    param.name = "New Parameter";
                    param.type = type;
                    list.list.Add(param);
                });
            }
            menu.ShowAsContext();
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
        
        parameterGUI.DoLayoutList();
    }
}
