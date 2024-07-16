using CustomInspector.Extensions;
using System;
using System.Diagnostics;
using UnityEngine;

namespace CustomInspector
{
    /// <summary>
    /// Hide and show groups of fields based on a toolbar-selection
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class TabAttribute : PropertyAttribute
    {
        public readonly string groupName = null;

        [Obsolete("Please use the [BackgroundColor]-attribute instead")]
        public FixedColor backgroundColor;

        public TabAttribute(string groupName)
        {
            order = HorizontalGroupAttribute.defaultOrder - 1; //has to be drawn before HorizontalGroup, because tab-table is always centered
            this.groupName = groupName;
        }
        public TabAttribute(InspectorIcon icon)
        {
            groupName = icon.ToInternalIconName();
        }
    }
}