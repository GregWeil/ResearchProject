using UnityEngine;
using UnityEditor;
using System.Collections;

public class StateMachineEditor : EditorWindow {

    Rect windowRect = new Rect(512, 128, 128, 128);

    [MenuItem("Window/State Machine")]
    static void ShowEditor() {
        var editor = GetWindow<StateMachineEditor>();
        editor.Show();
    }

    void OnEnable() {
        titleContent = new GUIContent("State Machine");
    }

    void OnGUI() {
        BeginWindows();
            windowRect = GUILayout.Window(0, windowRect, DrawWindow, "Window");
        EndWindows();

        GUILayout.BeginArea(new Rect(0, 0, 250, position.height), GUI.skin.box);
            GUILayout.Label("Hello");
            GUILayout.Button("Button");
        GUILayout.EndArea();
    }

    void DrawWindow(int id) {
        GUILayout.Button("Button");
        GUILayout.Label("Content");
        GUI.DragWindow();
    }
}

