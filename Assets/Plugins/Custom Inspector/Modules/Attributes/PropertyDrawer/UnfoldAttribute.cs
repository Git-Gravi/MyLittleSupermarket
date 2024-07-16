using System;
using System.Diagnostics;
using UnityEngine;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class UnfoldAttribute : PropertyAttribute
    {
        public UnfoldAttribute()
        {
            order = -10;
        }
    }
}
