using UnityEngine;
using UnityEditor;
using System.Collections;

public class StateMachineEditor : EditorWindow {

    [MenuItem("Window/State Machine")]
    static void ShowEditor() {
        var editor = GetWindow<StateMachineEditor>();
        editor.Show();
    }


    Vector2 stateSize = new Vector2(128, 64);
    float panelWidth = 250;

    StateMachine machine = null;
    int stateSelected = -1;

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
            if (Event.current.type == EventType.mouseDown) {
                if (Event.current.mousePosition.x > panelWidth) {
                    stateSelected = -1;
                }
            }

            DrawStateWindows();

            GUILayout.BeginArea(new Rect(0, 0, panelWidth, position.height), GUI.skin.box);
            DrawPanelContent();
            GUILayout.EndArea();
        }
    }


    void DrawStateWindow(int id) {
        if (Event.current.type == EventType.mouseDown) {
            stateSelected = id;
        }
        GUILayout.Label(machine.states[id].name);
        GUI.DragWindow();
    }

    void DrawStateWindows() {
        BeginWindows();
        Vector2 stateOffset = (position.size / 2) - (stateSize / 2);
        stateOffset.x += panelWidth / 2;
        for (var i = 0; i < machine.states.Count; ++i) {
            var rect = new Rect(machine.states[i].editorPosition + stateOffset, stateSize);
            rect = GUI.Window(i, rect, DrawStateWindow, new GUIContent());
            machine.states[i].editorPosition = rect.position - stateOffset;
        }
        EndWindows();
    }


    void DrawPanelContent() {
        GUILayout.Label(machine.gameObject.name);
        if (GUILayout.Button("Select Game Object")) {
            Selection.activeGameObject = machine.gameObject;
        }

        GUILayout.Space(16);

        if (GUILayout.Button("Add State")) {
            var state = new StateMachine.State();
            state.name = machine.states.Count.ToString();
            state.editorPosition = Vector2.zero;
            machine.states.Add(state);
        }

        GUILayout.Space(16);

        if (stateSelected >= machine.states.Count) stateSelected = -1;
        if (stateSelected >= 0) {
            var state = machine.states[stateSelected];
            GUILayout.Label(state.name);
            if (GUILayout.Button("Remove state")) {
                machine.states.RemoveAt(stateSelected);
                stateSelected = -1;
            }
        }
    }

}

