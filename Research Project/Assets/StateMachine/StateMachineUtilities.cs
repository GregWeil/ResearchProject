using UnityEngine;
using System;
using System.Collections.Generic;

namespace StateMachineUtilities {

    [System.Serializable]
    public class Parameter {
        public string name = string.Empty;
        [System.NonSerialized]
        public Type type = typeof(float);
        public object value {
            get {
                if ((internalValue == null) || (internalValue.GetType() != type)) {
                    internalValue = System.Activator.CreateInstance(type);
                }
                return internalValue;
            }
            set {
                if (value.GetType() == type) {
                    internalValue = value;
                } else {
                    throw new System.Exception("Invalid type for parameter");
                }
            }
        }

        [System.NonSerialized]
        private object internalValue = null;

        public string serializedType;
        public string serializedValue;
    }

    [System.Serializable]
    public class State {
        public string name = string.Empty;
        public List<Transition> transitions = new List<Transition>();
        public Vector2 position = Vector2.zero;
    }

    [System.Serializable]
    public class Transition {
        [System.NonSerialized]
        public State from = null;
        [System.NonSerialized]
        public State to = null;

        public List<Filter> conditions = new List<Filter>();

        public int serializedTo;
    }

    [System.Serializable]
    public class Filter {
        [System.NonSerialized]
        public System.Reflection.MethodInfo method = null;
        public Argument[] arguments = null;

        public string serializedMethodType;
        public string serializedMethodName;
    }

    [System.Serializable]
    public class Argument {
        public enum Style {
            Constant, Parameter, Filter
        }

        public Style style = Style.Constant;
        [System.NonSerialized]
        public System.Type type = null;
        public object value {
            get {
                if (((internalValue == null) || (internalValue.GetType() != type)) && (style == Style.Constant)) {
                    internalValue = System.Activator.CreateInstance(type);
                }
                return internalValue;
            }
            set {
                System.Type reqType = type;
                if (style == Style.Filter) {
                    type = typeof(Filter);
                } else if (style == Style.Parameter) {
                    type = typeof(Parameter);
                }
                if (value.GetType() == reqType) {
                    internalValue = value;
                } else {
                    throw new System.Exception("Invalid type for argument");
                }
            }
        }

        [System.NonSerialized]
        private object internalValue = null;

        public string serializedValue;
    }


    //Inherit from this when defining conditions and actions
    public class Module { }

    [AttributeUsage(AttributeTargets.Method)]
    public class Method : Attribute {
        public readonly string name = string.Empty;

        public Method(string name) {
            this.name = name;
        }

    }

}