using UnityEngine;
using UnityEditor;
using System.Collections;

public class StateMachineEditor : EditorWindow {

    [MenuItem("Window/State Machine")]
    static void ShowEditor() {
        var editor = GetWindow<StateMachineEditor>();
        editor.Show();
    }

    StateMachine machine = null;

    public void ShowWithTarget(StateMachine target) {
        machine = target;
        Show();
    }

    void OnEnable() {
        titleContent = new GUIContent("State Machine");
    }

    void OnGUI() {
        if (machine == null) {
            GUILayout.Label("No state machine selected!");
        } else {
            var matrix = GUI.matrix;
            //GUI.matrix = GUI.matrix * Matrix4x4.TRS(new Vector3(100, 100, 0), Quaternion.identity, Vector3.one);
            BeginWindows();
            Vector2 stateSize = new Vector2(128, 64);
            for (var i = 0; i < machine.states.Count; ++i) {
                var rect = new Rect(machine.states[i].editorPosition-stateSize/2, stateSize);
                rect = GUI.Window(i, rect, DrawWindow, new GUIContent(), GUI.skin.box);
                machine.states[i].editorPosition = rect.position+stateSize/2;
            }
            EndWindows();
            GUI.matrix = matrix;

            GUILayout.BeginArea(new Rect(0, 0, 250, position.height), GUI.skin.box);
            GUILayout.Label(machine.gameObject.name);
            if (GUILayout.Button("Select Game Object")) {
                Selection.activeGameObject = machine.gameObject;
            }
            GUILayout.Space(32);
            if (GUILayout.Button("Add State")) {
                var state = new StateMachine.State();
                state.name = machine.states.Count.ToString();
                state.editorPosition = position.size / 2;
                machine.states.Add(state);
            }
            GUILayout.EndArea();
        }
    }

    void DrawWindow(int id) {
        GUILayout.Label(machine.states[id].name);
        GUI.DragWindow();
    }
}

