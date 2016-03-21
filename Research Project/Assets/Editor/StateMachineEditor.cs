using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StateMachineEditor : EditorWindow {

    [MenuItem("Window/State Machine")]
    public static void ShowEditor() {
        var editor = GetWindow<StateMachineEditor>();
        editor.Show();
    }

    public static void ShowWithTarget(StateMachine target) {
        var editor = GetWindow<StateMachineEditor>();
        editor.machine = target;
        editor.stateSelected = null;
        editor.transitionSelected = null;
        editor.Show();
    }

    void OnEnable() {
        titleContent = new GUIContent("State Machine");
        Undo.undoRedoPerformed += eventUndoRedo;
    }


    //===========================================
    //Editor properties
    //===========================================

    Vector2 stateSize = new Vector2(128, 64);
    float panelWidth = 250;
    Vector2 origin = Vector2.zero;

    StateMachine machine = null;

    StateMachine.State _stateSelected = null;
    StateMachine.Transition _transitionSelected = null;

    //External getter/setter for selection
    //Make sure only one thing is ever selected

    StateMachine.State stateSelected {
        get { return _stateSelected; }
        set {
            _stateSelected = value;
            _transitionSelected = null;
            EditorGUI.FocusTextInControl("");
        }
    }
    StateMachine.Transition transitionSelected {
        get { return _transitionSelected; }
        set {
            _transitionSelected = value;
            _stateSelected = null;
            EditorGUI.FocusTextInControl("");
        }
    }


    //===========================================
    //State interaction
    //===========================================

    StateMachine.State stateCreate(Vector2 pos) {
        var state = new StateMachine.State();
        state.name = "New State";
        state.editorPosition = pos;
        machine.states.Add(state);
        return state;
    }

    void stateCreateUser(object pos) {
        Undo.RecordObject(machine, "Create State");
        stateSelected = stateCreate((Vector2)pos);
    }

    void stateDelete(StateMachine.State state) {
        foreach (var transition in machine.transitions.ToArray()) {
            if ((transition.from == state) || (transition.to == state)) {
                transitionDelete(transition);
            }
        }

        machine.states.Remove(state);

        if (stateSelected == state) {
            stateSelected = null;
        }
    }

    void stateDeleteUser(object state) {
        var s = (StateMachine.State)state;
        if (EditorUtility.DisplayDialog("Delete state?", "Do you want to remove state '"
            + s.name + "' and its transitions?", "Ok", "Cancel"))
        {
            Undo.RecordObject(machine, "Delete State");
            stateDelete(s);
        }
    }


    //===========================================
    //Transition interaction
    //===========================================

    struct TransitionInfo {
        public StateMachine.State from, to;
        public TransitionInfo(StateMachine.State f, StateMachine.State t) {
            from = f; to = t;
        }
    }

    StateMachine.Transition transitionCreate(TransitionInfo data) {
        var transition = new StateMachine.Transition();
        transition.from = data.from;
        transition.to = data.to;
        machine.transitions.Add(transition);
        return transition;
    }

    void transitionCreateUser(object data) {
        Undo.RecordObject(machine, "Create Transition");
        transitionSelected = transitionCreate((TransitionInfo)data);
    }

    void transitionDelete(StateMachine.Transition transition) {
        machine.transitions.Remove(transition);

        if (transitionSelected == transition) {
            transitionSelected = null;
        }
    }

    void transitionDeleteUser(object transition) {
        var t = (StateMachine.Transition)transition;
        if (EditorUtility.DisplayDialog("Delete transition?",
            "Are you sure you want to remove the transition from '" +
            t.from.name + "' to '" + t.to.name + "'?", "Ok", "Cancel"))
        {
            Undo.RecordObject(machine, "Delete Transition");
            transitionDelete(t);
        }
    }


    //===========================================
    //Event handling
    //===========================================

    void eventUndoRedo() {
        if (machine != null) {
            //Ensure selected state still exists
            if (stateSelected != null) {
                if (!machine.states.Contains(stateSelected)) {
                    stateSelected = null;
                }
            }
            //Ensure selected transition still exists
            if (transitionSelected != null) {
                if (!machine.transitions.Contains(transitionSelected)) {
                    transitionSelected = null;
                }
            }
        }
        Repaint();
    }

    void eventState(StateMachine.State state, Event e) {
        //Handle events involving a state

        if (e.type == EventType.mouseDown) {
            if (e.button == 0) {
                stateSelected = state;
            } else if (e.button == 1) {
                var menu = new GenericMenu();
                if ((stateSelected != state) && (stateSelected != null)) {
                    menu.AddItem(new GUIContent("New transition"), false, transitionCreateUser, new TransitionInfo(stateSelected, state));
                }
                menu.AddItem(new GUIContent("Remove state"), false, stateDeleteUser, state);
                menu.ShowAsContext();
            }
        }
    }

    void eventTransition(StateMachine.Transition transition, Event e) {
        //Handle events involving a transition

        if (e.type == EventType.mouseDown) {
            if (e.button == 0) {
                transitionSelected = transition;
            } else if (e.button == 1) {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove transition"), false, transitionDeleteUser, transition);
                menu.ShowAsContext();
            }
        }
    }

    void eventWindow(Event e) {
        //Handle events involving the full editor window
        var pos = (e.mousePosition - origin);

        if (e.mousePosition.x > panelWidth) {
            foreach (var state in machine.states) {
                var rect = new Rect(state.editorPosition - stateSize / 2, stateSize);
                if (rect.Contains(pos)) {
                    //Don't handle the event here
                    //Do that in the window so layering works
                    //eventState(state, e);
                    return;
                }
            }

            foreach (var transition in machine.transitions) {
                var from = transition.from.editorPosition;
                var to = transition.to.editorPosition;
                if (HandleUtility.DistancePointToLineSegment(pos, from, to) < 10f) {
                    eventTransition(transition, e);
                    return;
                }
            }
        }

        if (e.type == EventType.mouseDown) {
            if (e.mousePosition.x > panelWidth) {
                if (e.button == 0) {
                    stateSelected = null;
                    transitionSelected = null;
                } else if (e.button == 1) {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("New state"), false, stateCreateUser, pos);
                    menu.ShowAsContext();
                }
            }
        }
    }


    //===========================================
    //Main GUI event
    //===========================================

    void OnGUI() {
        origin = new Vector2(Mathf.Round((position.width + panelWidth) / 2), Mathf.Round(position.height / 2));

        if (machine == null) {
            GUILayout.Label("No state machine selected!");
        } else {
            DrawTransitionLines();
            DrawStateWindows();
            
            GUILayout.BeginArea(new Rect(0, 0, panelWidth, position.height), GUI.skin.box);
            DrawPanelContent();
            GUILayout.EndArea();

            eventWindow(Event.current);
        }
    }


    //===========================================
    //Area drawing
    //===========================================

    void DrawTransitionLine(int id) {
        //Draw a single transition, outlining it if selected
        var t = machine.transitions[id];
        var from = t.from.editorPosition;
        var to = t.to.editorPosition;

        var center = Vector2.Lerp(from, to, 0.5f);
        var forward = (5f * (to - from).normalized);
        var side = (Vector2)(Quaternion.Euler(0, 0, 90f) * forward);

        if (t == transitionSelected) {
            var color = Handles.color;
            Handles.color = Color.black;
            Handles.DrawAAPolyLine(7.5f, from + origin, to + origin);
            Handles.color = color;
        }

        Handles.DrawAAPolyLine(5f, from + origin, to + origin);
        Handles.DrawAAConvexPolygon(center + forward + origin,
            center - forward + side + origin, center - forward - side + origin);
    }

    void DrawTransitionLines() {
        //Draw all transitions

        for (var i = 0; i < machine.transitions.Count; ++i) {
            DrawTransitionLine(i);
        }
    }


    void DrawStateWindow(int id) {
        //Draw a single state, and handle interactions
        var state = machine.states[id];

        eventState(state, Event.current);

        GUILayout.Label(state.name);

        GUI.DragWindow();
    }

    void DrawStateWindows() {
        //Draw all states

        if (stateSelected != null) {
            GUI.FocusWindow(machine.states.IndexOf(stateSelected));
        }

        BeginWindows();
        Vector2 stateOffset = origin - (stateSize / 2);
        for (var i = 0; i < machine.states.Count; ++i) {
            var rect = new Rect(machine.states[i].editorPosition + stateOffset, stateSize);
            rect = GUI.Window(i, rect, DrawStateWindow, GUIContent.none);
            machine.states[i].editorPosition = rect.position - stateOffset;
        }
        EndWindows();
    }
    

    //===========================================
    //Draw the side panel
    //===========================================

    void DrawPanelContent() {
        //Draw information on the currently selected state or transition

        EditorGUILayout.LabelField(machine.gameObject.name);
        if (GUILayout.Button("Select Game Object")) {
            Selection.activeGameObject = machine.gameObject;
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        if (stateSelected != null) {
            stateSelected.name = EditorGUILayout.DelayedTextField(stateSelected.name);
        } else if (transitionSelected != null) {
            EditorGUILayout.LabelField(transitionSelected.from.name);
            EditorGUILayout.LabelField(transitionSelected.to.name);
        } else {
            EditorGUILayout.LabelField("Nothing selected");
        }
        EditorGUILayout.EndVertical();
    }

}

