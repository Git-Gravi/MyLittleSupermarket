using System;
using System.Diagnostics;
using UnityEngine;

namespace CustomInspector
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        /// <summary>
        /// Path to an Animator or to an AnimatorController
        /// </summary>
        public readonly string animatorPath;

        public AnimatorParameterAttribute(string animatorPath)
        {
            this.animatorPath = animatorPath;
        }
    }
}
