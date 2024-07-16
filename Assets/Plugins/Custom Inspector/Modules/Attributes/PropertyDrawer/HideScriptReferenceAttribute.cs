using System;
using System.Diagnostics;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class HideScriptReferenceAttribute : Attribute
    {
    }
}
