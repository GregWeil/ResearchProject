using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

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
        initConditionGUI();
    }


    //===========================================
    //Editor properties
    //===========================================

    Vector2 stateSize = new Vector2(128, 64);
    float panelWidth = 250;
    float transitionSpacing = 10;

    StateMachine machine = null;
    Vector2 origin = Vector2.zero;

    StateMachine.State _stateSelected = null;
    StateMachine.Transition _transitionSelected = null;

    ReorderableList conditionGUI = null;

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

            if (_transitionSelected != null) {
                conditionGUI.list = _transitionSelected.conditions;
            }
        }
    }


    //===========================================
    //State interaction
    //===========================================

    StateMachine.State stateCreate(Vector2 pos) {
        var state = new StateMachine.State();
        state.name = "New State";
        state.position = pos;
        machine.states.Add(state);
        return state;
    }

    void stateCreateUser(object pos) {
        Undo.RecordObject(machine, "Create State");
        stateSelected = stateCreate((Vector2)pos);
        Undo.IncrementCurrentGroup();
    }

    void stateDelete(StateMachine.State state) {
        //Remove transitions from this state
        foreach (var transition in state.transitions.ToArray()) {
            transitionDelete(transition);
        }

        //Remove transitions to this state
        foreach (var transition in machine.states.SelectMany(s => s.transitions).ToArray()) {
            if (transition.to == state) {
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
            Undo.IncrementCurrentGroup();
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
        transition.from.transitions.Add(transition);
        return transition;
    }

    void transitionCreateUser(object data) {
        Undo.RecordObject(machine, "Create Transition");
        transitionSelected = transitionCreate((TransitionInfo)data);
        Undo.IncrementCurrentGroup();
    }

    void transitionDelete(StateMachine.Transition transition) {
        transition.from.transitions.Remove(transition);

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
            Undo.IncrementCurrentGroup();
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
                if (!transitionSelected.from.transitions.Contains(transitionSelected)) {
                    transitionSelected = null;
                }
            }
        }
        Repaint();
    }

    void eventState(StateMachine.State state, Event e) {
        //Handle events involving a state

        if (e.type == EventType.MouseDown) {
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
        } else if (e.type == EventType.MouseDrag) {
            Undo.RecordObject(machine, "Move State");
        }
    }

    void eventTransition(StateMachine.Transition transition, Event e) {
        //Handle events involving a transition

        if (e.type == EventType.MouseDown) {
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
                var rect = new Rect(state.position - stateSize / 2, stateSize);
                if (rect.Contains(pos)) {
                    //Don't handle the event here
                    //Do that in the window so layering works
                    //eventState(state, e);
                    return;
                }
            }

            foreach (var transition in machine.states.SelectMany(state => state.transitions)) {
                var from = transition.from.position;
                var to = transition.to.position;
                var index = transition.from.transitions.Where(t => t.to == transition.to).ToList().IndexOf(transition) + 0.5f;
                var offset = (index * transitionSpacing * (Vector2)(Quaternion.Euler(0, 0, 90f) * (to - from).normalized));

                if (HandleUtility.DistancePointToLineSegment(pos, from + offset, to + offset) < (transitionSpacing / 2)) {
                    eventTransition(transition, e);
                    return;
                }
            }
        }

        if (e.type == EventType.MouseDown) {
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
            if ((Event.current.type != EventType.Used) && (Event.current.type != EventType.Ignore)) {
                DrawPanelContent();
            }
            GUILayout.EndArea();

            eventWindow(Event.current);
            if (Event.current.type == EventType.MouseDown) {
                Repaint();
            }
        }
    }


    //===========================================
    //Area drawing
    //===========================================

    void DrawTransitionLine(StateMachine.Transition t) {
        //Draw a single transition, outlining it if selected
        var index = t.from.transitions.Where(transition => transition.to == t.to).ToList().IndexOf(t) + 0.5f;
        var from = t.from.position;
        var to = t.to.position;

        var forward = (to - from).normalized;
        var side = (Vector2)(Quaternion.Euler(0, 0, 90f) * forward);

        var offset = (transitionSpacing * side * index);
        from += offset;
        to += offset;

        var center = Vector2.Lerp(from, to, 0.5f);
        forward *= 5f;
        side *= 5f;

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

        foreach (var transition in machine.states.SelectMany(state => state.transitions)) {
            DrawTransitionLine(transition);
        }
    }


    void DrawStateWindow(int id) {
        //Draw a single state, and handle interactions
        var state = machine.states[id];

        GUILayout.Label(state.name);

        eventState(state, Event.current);

        GUI.DragWindow();
    }

    void DrawStateWindows() {
        //Draw all states

        if (stateSelected != null) {
            GUI.FocusWindow(machine.states.IndexOf(stateSelected));
        } else {
            GUI.FocusWindow(-1);
        }

        BeginWindows();
        Vector2 stateOffset = origin - (stateSize / 2);
        for (var i = 0; i < machine.states.Count; ++i) {
            var rect = new Rect(machine.states[i].position + stateOffset, stateSize);
            rect = GUI.Window(i, rect, DrawStateWindow, GUIContent.none);
            machine.states[i].position = rect.position - stateOffset;
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
            if ((Event.current.type != EventType.Layout) && (Event.current.type != EventType.Repaint)) {
                Undo.RecordObject(machine, "Modify State");
            }
            stateSelected.name = EditorGUILayout.TextField(stateSelected.name);
            EditorGUILayout.Space();
        } else if (transitionSelected != null) {
            if ((Event.current.type != EventType.Layout) && (Event.current.type != EventType.Repaint)) {
                Undo.RecordObject(machine, "Modify Transition");
            }
            EditorGUILayout.LabelField(transitionSelected.from.name);
            EditorGUILayout.LabelField(transitionSelected.to.name);
            EditorGUILayout.Space();
            conditionGUI.DoLayoutList();
        } else {
            EditorGUILayout.LabelField("Nothing selected");
        }
        EditorGUILayout.EndVertical();
    }

    //Transition conditions

    void initConditionGUI() {
        conditionGUI = new ReorderableList(null, typeof(StateMachine.Filter));
        conditionGUI.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Conditions");
        };

        conditionGUI.drawElementCallback = (Rect rect, int index, bool active, bool focused) => {
            var cond = (StateMachine.Filter)conditionGUI.list[index];
            foreach (StateMachineMethod attr in cond.method.GetCustomAttributes(true)
                    .Where(attr => attr is StateMachineMethod)) {
                EditorGUI.LabelField(rect, attr.name);
            }
        };

        conditionGUI.onAddDropdownCallback = (Rect rect, ReorderableList list) => {
            var menu = new GenericMenu();
            foreach (System.Reflection.MethodInfo method in
                    System.Reflection.Assembly.GetAssembly(typeof(StateMachineModule)).GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(StateMachineModule)))
                    .SelectMany(type => type.GetMethods())) {
                foreach (StateMachineMethod attr in method.GetCustomAttributes(true)
                        .Where(attr => attr is StateMachineMethod)) {
                    var theMethod = method; //Use the right thing for the inline function when iterating
                    menu.AddItem(new GUIContent(attr.name), false, () => {
                        Undo.RecordObject(machine, "Add Condition");
                        var cond = new StateMachine.Filter();
                        cond.method = theMethod;
                        list.list.Add(cond);
                        Undo.IncrementCurrentGroup();
                    });
                }
            }
            menu.ShowAsContext();
        };
        conditionGUI.onRemoveCallback = (ReorderableList list) => {
            Undo.RecordObject(machine, "Remove Condition");
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            Undo.IncrementCurrentGroup();
        };
    }
}

