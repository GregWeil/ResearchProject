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

    Vector2 origin = Vector2.zero;
    int stateSelected = -1;


    public void ShowWithTarget(StateMachine target) {
        machine = target;
        Show();
    }

    void OnEnable() {
        titleContent = new GUIContent("State Machine");
    }


    //State interaction

    int stateCreate(Vector2 pos) {
        var state = new StateMachine.State();
        state.name = "New State " + machine.states.Count.ToString();
        state.editorPosition = pos;
        machine.states.Add(state);
        return machine.states.Count;
    }

    void stateDelete(int index) {
        machine.states.RemoveAt(index);
        if (stateSelected == index) {
            stateSelected = -1;
        } else if (stateSelected > index) {
            stateSelected -= 1;
        }
    }

    void stateCreate(object pos) {
        int state = stateCreate((Vector2)pos);
        stateSelected = state;
    }

    void stateDelete(object index) {
        stateDelete((int)index);
    }


    //Main GUI draw

    void OnGUI() {
        origin = new Vector2((position.width + panelWidth) / 2, position.height / 2);

        if (machine == null) {
            GUILayout.Label("No state machine selected!");
        } else {
            if (Event.current.type == EventType.mouseDown) {
                if (Event.current.mousePosition.x > panelWidth) {
                    stateSelected = -1;
                    if (Event.current.button == 1) {
                        Vector2 pos = Event.current.mousePosition - origin;
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("New state"), false, stateCreate, pos);
                        menu.ShowAsContext();
                    }
                }
            }

            DrawStateWindows();

            GUILayout.BeginArea(new Rect(0, 0, panelWidth, position.height), GUI.skin.box);
            DrawPanelContent();
            GUILayout.EndArea();
        }
    }


    //Area drawing

    void DrawStateWindow(int id) {
        //Draw a single state, and handle interactions

        if (Event.current.type == EventType.mouseDown) {
            stateSelected = id;
            if (Event.current.button == 1) {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove"), false, stateDelete, id);
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        GUILayout.Label(machine.states[id].name);

        GUI.DragWindow();
    }

    void DrawStateWindows() {
        //Draw all states

        BeginWindows();
        Vector2 stateOffset = origin - (stateSize / 2);
        for (var i = 0; i < machine.states.Count; ++i) {
            var rect = new Rect(machine.states[i].editorPosition + stateOffset, stateSize);
            rect = GUI.Window(i, rect, DrawStateWindow, GUIContent.none);
            machine.states[i].editorPosition = rect.position - stateOffset;
        }
        EndWindows();
    }


    void DrawPanelContent() {
        //Draw information on the currently selected state or transition

        GUILayout.Label(machine.gameObject.name);
        if (GUILayout.Button("Select Game Object")) {
            Selection.activeGameObject = machine.gameObject;
        }

        GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

        GUILayout.BeginVertical(GUI.skin.box);
        if (stateSelected >= machine.states.Count) stateSelected = -1;
        if (stateSelected >= 0) {
            var state = machine.states[stateSelected];
            state.name = GUILayout.TextField(state.name);
        } else {
            GUILayout.Label("No state selected");
        }
        GUILayout.EndVertical();
    }

}

