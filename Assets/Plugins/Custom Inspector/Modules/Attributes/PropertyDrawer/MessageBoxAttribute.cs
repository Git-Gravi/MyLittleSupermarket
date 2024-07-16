using UnityEngine;
using System.Diagnostics;
using System;

namespace CustomInspector
{
    public enum MessageBoxType { None, Info, Warning, Error }
    /// <summary>
    /// Draw a message box in the inspector. You can do it instead of the field or additionally
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class MessageBoxAttribute : PropertyAttribute
    {
        public readonly string content;
        public readonly MessageBoxType type;

        public MessageBoxAttribute(string content, MessageBoxType type = MessageBoxType.Error)
        {
            order = -10;

            this.content = content;
            this.type = type;
        }
    }
}