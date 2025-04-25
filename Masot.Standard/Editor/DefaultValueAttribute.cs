using System;
using UnityEditor;
using UnityEngine;

namespace Masot.Standard.Editor
{
    public class DefaultValueAttribute : PropertyAttribute
    {
        public object Value { get; set; }

        public DefaultValueAttribute(object value)
        {
            Value = value;
        }
    }

    public class InvalidValueType : Exception
    {
        public InvalidValueType(object? value, SerializedPropertyType type)
            : base($"Expected type {type} but got {value?.GetType()}") { }
    }

    [CustomPropertyDrawer(typeof(DefaultValueAttribute))]
    public class DefaultValueDrawer : PropertyDrawer
    {
        private bool doUpdate = true;

        public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            if (doUpdate)
            {
                var value = (attribute as DefaultValueAttribute)?.Value;

                object _ = property.propertyType switch
                {
                    SerializedPropertyType.Integer => EditorGUI.IntField(position, label, (value as int?) ?? throw new InvalidValueType(value, SerializedPropertyType.Integer)),
                    SerializedPropertyType.String => EditorGUI.TextField(position, label, (value as string) ?? throw new InvalidValueType(value, SerializedPropertyType.String)),
                    SerializedPropertyType.Boolean => EditorGUI.Toggle(position, label, (value as bool?) ?? throw new InvalidValueType(value, SerializedPropertyType.Boolean)),
                    _ => EditorGUI.PropertyField(position, property, label, true)
                };
            }

            EditorGUI.PropertyField(position, property, label, true);
            doUpdate = false;
        }
    }
}
