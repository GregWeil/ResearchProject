﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
    StateMachine.State stateSelected = null;


    public void ShowWithTarget(StateMachine target) {
        machine = target;
		stateSelected = null;
        Show();
    }

    void OnEnable() {
        titleContent = new GUIContent("State Machine");
    }


    //State interaction

    StateMachine.State stateCreate(Vector2 pos) {
        var state = new StateMachine.State();
        state.name = "New State";
        state.editorPosition = pos;
        machine.states.Add(state);
        return state;
    }

    void stateCreate(object pos) {
        stateSelected = stateCreate((Vector2)pos);
    }

    void stateDelete(int index) {
        machine.states.RemoveAt(index);
    }

    void stateDelete(object index) {
        stateDelete((int)index);
    }

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

    void transitionCreate(object data) {
        transitionCreate((TransitionInfo)data);
    }


    //Main GUI event

    void OnGUI() {
        origin = new Vector2((position.width + panelWidth) / 2, position.height / 2);

        if (machine == null) {
            GUILayout.Label("No state machine selected!");
        } else {
            if (Event.current.type == EventType.mouseDown) {
                if (Event.current.mousePosition.x > panelWidth) {
                    if (Event.current.button == 0) {
                        stateSelected = null;
                    } else if (Event.current.button == 1) {
                        Vector2 pos = Event.current.mousePosition - origin;
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("New state"), false, stateCreate, pos);
                        menu.ShowAsContext();
                    }
                }
            }

            DrawTransitionLines();

            DrawStateWindows();

            GUILayout.BeginArea(new Rect(0, 0, panelWidth, position.height), GUI.skin.box);
            DrawPanelContent();
            GUILayout.EndArea();
        }
    }


    //Area drawing

    void DrawTransitionLine(int id) {
        StateMachine.Transition t = machine.transitions[id];

        Handles.DrawLine(t.from.editorPosition + origin, t.to.editorPosition + origin);
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

        if (Event.current.type == EventType.mouseDown) {
            if (Event.current.button == 0) {
                stateSelected = state;
            } else if (Event.current.button == 1) {
                var menu = new GenericMenu();
                if ((stateSelected != state) && (stateSelected != null)) {
                    menu.AddItem(new GUIContent("New transition"), false, transitionCreate, new TransitionInfo(stateSelected, state));
                }
                menu.AddItem(new GUIContent("Remove"), false, stateDelete, id);
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

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


    void DrawPanelContent() {
        //Draw information on the currently selected state or transition

        GUILayout.Label(machine.gameObject.name);
        if (GUILayout.Button("Select Game Object")) {
            Selection.activeGameObject = machine.gameObject;
        }

        GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

        GUILayout.BeginVertical(GUI.skin.box);
        if (stateSelected != null) {
            stateSelected.name = GUILayout.TextField(stateSelected.name);
        } else {
            GUILayout.Label("No state selected");
        }
        GUILayout.EndVertical();
    }

}

