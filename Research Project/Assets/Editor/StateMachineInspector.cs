using UnityEngine;
using UnityEditor;
using System.Linq;

using StateMachineUtilities;

[CustomEditor(typeof(StateMachine))]
public class StateMachineInspector : Editor {
    
    string[] typeNames = { "Bool", "Float", "Integer", "Vector2", "Vector3", "Door", "Button" };
    System.Type[] typeTypes = { typeof(bool), typeof(float), typeof(int), typeof(Vector2), typeof(Vector3), typeof(Door), typeof(Button) };
    
    UnityEditorInternal.ReorderableList parameterGUI;
    
	// Use this for initialization
	void OnEnable() {
        var machine = (StateMachine)target;

        parameterGUI = new UnityEditorInternal.ReorderableList(machine.parameters, typeof(Parameter));
        parameterGUI.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Parameters");
        };

        parameterGUI.drawElementCallback = (Rect rect, int index, bool active, bool focused) => {
            var parameter = (Parameter)parameterGUI.list[index];
            Undo.RecordObject(machine, "Modify Parameter");
            
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
                parameter.value = EditorGUI.Vector2Field(rValue, GUIContent.none, (Vector2)parameter.value);
            } else if (parameter.type == typeof(Vector3)) {
                parameter.value = EditorGUI.Vector3Field(rValue, GUIContent.none, (Vector3)parameter.value);
            } else if (parameter.type.IsSubclassOf(typeof(Object))) {
                parameter.value = EditorGUI.ObjectField(rValue, GUIContent.none, (Object)parameter.value, parameter.type, true);
            }
            EditorGUIUtility.labelWidth = 0;
        };

        parameterGUI.onAddDropdownCallback = (Rect rect, UnityEditorInternal.ReorderableList list) => {
            var menu = new GenericMenu();
            for (var i = 0; i < Mathf.Min(typeTypes.Length, typeNames.Length); ++i) {
                var type = typeTypes[i];
                menu.AddItem(new GUIContent(typeNames[i]), false, () => {
                    Undo.RecordObject(machine, "Add Parameter");
                    var param = new Parameter();
                    param.name = "New Parameter";
                    param.type = type;
                    list.list.Add(param);
                    Undo.IncrementCurrentGroup();
                });
            }
            menu.ShowAsContext();
        };
        parameterGUI.onRemoveCallback = (UnityEditorInternal.ReorderableList list) => {
            Undo.RecordObject(machine, "Remove Parameter");
            UnityEditorInternal.ReorderableList.defaultBehaviours.DoRemoveButton(list);
            Undo.IncrementCurrentGroup();
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

        if (machine.states.Count > 0) {
            var oldIndex = machine.states.IndexOf(machine.initialState);
            if (oldIndex < 0) oldIndex = 0;
            var newIndex = EditorGUILayout.Popup("Initial state", oldIndex, machine.states.Select(state => state.name).ToArray());
            machine.initialState = machine.states[newIndex];
        } else {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Popup("Initial state", 0, new string[] { "No states" });
            EditorGUI.EndDisabledGroup();
        }
        
        parameterGUI.DoLayoutList();
    }
}
