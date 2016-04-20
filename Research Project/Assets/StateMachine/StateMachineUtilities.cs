﻿using UnityEngine;
using System.Collections.Generic;

namespace StateMachineUtilities {

    [System.Serializable]
    public class Parameter {
        public string name = string.Empty;
        [System.NonSerialized]
        public System.Type type = typeof(float);
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
        public System.Reflection.ParameterInfo param = null;
        public object value {
            get {
                System.Type reqType = param.ParameterType;
                if (style == Style.Filter) {
                    reqType = typeof(Filter);
                } else if (style == Style.Parameter) {
                    reqType = typeof(Parameter);
                }
                if ((internalValue == null) || (internalValue.GetType() != reqType)) {
                    if (style == Style.Constant) {
                        internalValue = System.Activator.CreateInstance(reqType);
                    } else {
                        internalValue = null;
                    }
                }
                return internalValue;
            }
            set {
                System.Type reqType = param.ParameterType;
                if (style == Style.Filter) {
                    reqType = typeof(Filter);
                } else if (style == Style.Parameter) {
                    reqType = typeof(Parameter);
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


    //Serialization methods

    public class Serialization {

        public static string serializeObject(object value, System.Type type) {
            var serializer = new System.Xml.Serialization.XmlSerializer(type);
            var writer = new System.IO.StringWriter();
            serializer.Serialize(writer, System.Convert.ChangeType(value, type));
            return writer.ToString();
        }

        public static object deserializeObject(string value, System.Type type) {
            var serializer = new System.Xml.Serialization.XmlSerializer(type);
            var reader = new System.IO.StringReader(value);
            return serializer.Deserialize(reader);
        }

    }


    //Module definitions

    //Inherit from this when defining conditions and actions
    public class Module { }

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class Method : System.Attribute {
        public readonly string name = string.Empty;

        public Method(string name) {
            this.name = name;
        }

    }

}