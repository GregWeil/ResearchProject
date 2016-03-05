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

    Rect windowRect = new Rect(512, 128, 128, 128);

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
            BeginWindows();
                windowRect = GUILayout.Window(0, windowRect, DrawWindow, "Window", GUI.skin.box);
            EndWindows();

            GUILayout.BeginArea(new Rect(0, 0, 250, position.height), GUI.skin.box);
            GUILayout.Label(machine.gameObject.name);
            if (GUILayout.Button("Select Game Object")) {
                Selection.activeGameObject = machine.gameObject;
            }
            GUILayout.EndArea();
        }
    }

    void DrawWindow(int id) {
        GUILayout.Button("Button");
        GUILayout.Label("Content");
        GUI.DragWindow();
    }
}

