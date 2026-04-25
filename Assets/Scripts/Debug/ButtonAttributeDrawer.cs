using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A custom property drawer for the <see cref="ButtonAttribute"/> class.
/// </summary>
[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var buttonAttribute = (ButtonAttribute)attribute;

        // Get the specified custom name, if none was specified use the target property's name.
        var buttonName = buttonAttribute.CustomName ?? property.name;

        if (GUI.Button(position, buttonName)) {
            InvokeSpecifiedMethod(property);
        }
    }

    /// <summary>
    /// Invokes the specified method.
    /// </summary>
    /// <param name="property"></param>
    private void InvokeSpecifiedMethod(SerializedProperty property)
    {
        var buttonAttribute = (ButtonAttribute)attribute;

        var target = property.serializedObject.targetObject;
        var type = target.GetType();
        var methodInfo = GetMethod(type, buttonAttribute.MethodName);

        if (methodInfo == null) {
            Debug.LogError($@"The specified method was not found '{buttonAttribute.MethodName}'");

            return;
        }

        if (methodInfo.GetParameters().Length > 0) {
            Debug.LogError("The specified method must be parameterless.");

            return;
        }

        // Invoke the parameterless method of the target.
        methodInfo.Invoke(target, Array.Empty<object>());
    }

    private static MethodInfo GetMethod(Type type, string methodName)
    {
        // Get a method using a set of binging flags.
        return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }
}