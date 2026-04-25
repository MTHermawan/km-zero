using System;
using System.Reflection;
using UnityEngine;

public class ButtonAttribute : PropertyAttribute
{
    /// <summary>
    /// Gets or Sets the name of the method.
    /// </summary>
    public string MethodName { get; set; }

    /// <summary>
    /// Gets or Sets a custom name for the button.
    /// </summary>
    public string? CustomName { get; set; }

    public ButtonAttribute(string methodName)
    {
        MethodName = methodName;
    }
}