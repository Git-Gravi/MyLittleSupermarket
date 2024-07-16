using System;
using System.Diagnostics;
using UnityEngine;

namespace CustomInspector
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class SceneAttribute : PropertyAttribute
    {
        public readonly bool useFullPath;
        public SceneAttribute(bool useFullPath = false)
        {
            this.useFullPath = useFullPath;
        }
    }
}
