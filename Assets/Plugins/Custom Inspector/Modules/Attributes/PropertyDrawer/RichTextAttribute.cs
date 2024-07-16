using System;
using System.Diagnostics;
using UnityEngine;

namespace CustomInspector
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class RichTextAttribute : PropertyAttribute
    {
        // [Multiline]-attribute and [TextArea]-attribute will define height and wrapping of input-box

        public RichTextAttribute()
        {
            order = -1; // right before [Multiline]-attribute and [TextArea]-attribute
        }

        // ----------------------OBSOLETE------------------------------

        // public readonly bool allowMultipleLines = true;
        // public readonly int maxLines;
        /// <summary> </summary>
        /// <param name="allowMultipleLines">If text field should be expanded on every newLine</param>
        /// <param name="maxLines">Even if "allowMultipleLines" is true, "maxLines" defines a maximum amount of lines.</param>
        [Obsolete("Please use the [Multiline]-attribute or [TextArea]-attribute to edit the input-box height and it's wrapping.")]
        public RichTextAttribute(bool allowMultipleLines, int maxLines = 15)
        : this()
        {
            // this.allowMultipleLines = allowMultipleLines;
            // this.maxLines = Math.Max(maxLines, 1);
        }
    }
}
