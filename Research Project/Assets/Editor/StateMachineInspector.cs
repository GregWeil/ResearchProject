using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using StateMachineUtilities;

[CustomEditor(typeof(StateMachine))]
public class StateMachineInspector : Editor {
    
    System.Type[] types = { typeof(bool), typeof(float), typeof(int), typeof(Vector3), typeof(GameObject) };
    Dictionary<System.Type, string> typeNames = new Dictionary<System.Type, string> { { typeof(float), "Float" }, { typeof(int), "Integer" } };
    
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
            
            Rect rLabel = new Rect(rect.x, (rect.y + (rect.height / 10)), (rect.width / 2), (rect.height * 0.75f));
            Rect rValue = new Rect(rLabel.xMax, rLabel.y, (rect.xMax - rLabel.xMax), rLabel.height);
            rLabel.width -= 8;

            if (active) {
                EditorGUI.BeginChangeCheck();
                string name = EditorGUI.TextField(rLabel, parameter.name);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(machine, "Rename Parameter");
                    parameter.name = name;
                }
            } else {
                EditorGUI.LabelField(rLabel, parameter.name);
            }

            EditorGUI.BeginChangeCheck();
            object value = parameter.value;
            EditorGUIUtility.labelWidth = 36;
            if (parameter.type == typeof(bool)) {
                value = EditorGUI.Toggle(rValue, "Bool", (bool)value);
            } else if (parameter.type == typeof(float)) {
                value = EditorGUI.FloatField(rValue, "Float", (float)value);
            } else if (parameter.type == typeof(int)) {
                value = EditorGUI.IntField(rValue, "Int", (int)value);
            } else if (parameter.type == typeof(Vector2)) {
                value = EditorGUI.Vector2Field(rValue, GUIContent.none, (Vector2)value);
            } else if (parameter.type == typeof(Vector3)) {
                value = EditorGUI.Vector3Field(rValue, GUIContent.none, (Vector3)value);
            } else if (parameter.type.IsSubclassOf(typeof(Object))) {
                value = EditorGUI.ObjectField(rValue, GUIContent.none, (Object)value, parameter.type, true);
            }
            EditorGUIUtility.labelWidth = 0;
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(machine, "Modify Parameter");
                parameter.value = value;
            }
        };

        parameterGUI.onAddDropdownCallback = (Rect rect, UnityEditorInternal.ReorderableList list) => {
            var menu = new GenericMenu();
            for (var i = 0; i < types.Length; ++i) {
                var type = types[i];
                var name = typeNames.ContainsKey(type) ? typeNames[type] : type.Name;
                menu.AddItem(new GUIContent(name), false, () => {
                    Undo.RecordObject(machine, "Add Parameter");
                    var param = new Parameter();
                    param.name = "New " + name + " Parameter";
                    param.type = type;
                    list.list.Add(param);
                    Undo.IncrementCurrentGroup();
                });
            }
            menu.ShowAsContext();
        };
        parameterGUI.onRemoveCallback = (UnityEditorInternal.ReorderableList list) => {
            //Keep a list of everywhere the parameter is used
            List<Argument> uses = new List<Argument>();

            //Build the list of parameter uses
            List<Method> methods = new List<Method>();
            methods.AddRange(machine.states.SelectMany(state => state.actions));
            methods.AddRange(machine.states.SelectMany(state => state.transitions).SelectMany(transition => transition.conditions));
            while (methods.Count > 0) {
                Method method = methods[0];
                foreach (var argument in method.arguments) {
                    if (argument.style == Argument.Style.Parameter) {
                        if (argument.value == list.list[list.index]) {
                            uses.Add(argument);
                        }
                    } else if (argument.style == Argument.Style.Filter) {
                        methods.Add((Method)argument.value);
                    }
                }
                methods.RemoveAt(0);
            }

            //If it is being used, confirm with the user
            if (uses.Count > 0) {
                if (!EditorUtility.DisplayDialog("Delete Parameter?", "The parameter '" + ((Parameter)list.list[list.index]).name +
                    "' is referenced in " + uses.Count + " " + (uses.Count > 1 ? "places" : "place") + ". Are you sure?", "Yes", "Cancel"))
                {
                    return;
                }
            }

            Undo.RecordObject(machine, "Remove Parameter");
            foreach (var argument in uses) {
                argument.style = Argument.Style.Constant;
            }
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
