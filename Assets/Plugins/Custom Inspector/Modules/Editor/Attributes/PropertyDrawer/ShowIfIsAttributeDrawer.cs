using CustomInspector.Extensions;
using System;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfIsAttribute))]
    [CustomPropertyDrawer(typeof(ShowIfIsNotAttribute))]
    public class ShowIfIsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.IsArrayElement())
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property,
                    $"conditional-show not valid on list elements.\nReplace field-type with {typeof(ListContainer<>).FullName} to apply all attributes to whole list instead of to single elements",
                    MessageType.Error);
                return;
            }


            ShowIfIsAttribute sa = (ShowIfIsAttribute)attribute;

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            object refValue;
            try
            {
                refValue = DirtyValue.GetOwner(property).FindRelative(sa.fieldPath).GetValue();
            }
            catch (MissingFieldException e)
            {
                DrawProperties.DrawPropertyWithMessage(position, label: label, property: property,
                                    $"{nameof(ShowIfIsAttribute)}: {e.Message}", MessageType.Error);
                return;
            }

            bool show = (refValue.IsUnityNull() && sa.value.IsUnityNull())
                        || (!refValue.IsUnityNull() && refValue.Equals(sa.value));

            if (show ^ sa.Inverted)
            {
                //Show
                position.height = DrawProperties.GetPropertyHeight(label, property);
                using (new EditorGUI.IndentLevelScope(sa.indent))
                {
                    EditorGUI.BeginChangeCheck();
                    DrawProperties.PropertyField(position, label: label, property: property);
                    if (EditorGUI.EndChangeCheck())
                        property.serializedObject.ApplyModifiedProperties();
                }
                return;
            }
            else
            {
                if (sa.style == DisabledStyle.Invisible) //Hide
                    return;
                else //if(sa.style == DisabledStyle.Disabled) //show disabled
                {
                    position.height = DrawProperties.GetPropertyHeight(label, property);
                    using (new EditorGUI.IndentLevelScope(sa.indent))
                    {
                        DrawProperties.DisabledPropertyField(position, label: label, property: property);
                    }
                }
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.IsArrayElement())
                return DrawProperties.GetPropertyWithMessageHeight(label, property);

            ShowIfIsAttribute sa = (ShowIfIsAttribute)attribute;

            object refValue;
            try
            {
                refValue = DirtyValue.GetOwner(property).FindRelative(sa.fieldPath).GetValue();
            }
            catch (MissingFieldException)
            {
                return DrawProperties.GetPropertyWithMessageHeight(label, property);
            }

            bool show = (refValue == null && sa.value == null)
                        || (refValue != null && refValue.Equals(sa.value));

            if (show ^ sa.Inverted)
            {
                //Show
                return DrawProperties.GetPropertyHeight(label, property);
            }
            else
            {
                if (sa.style == DisabledStyle.Invisible)//Hide
                    return -EditorGUIUtility.standardVerticalSpacing;
                else //if(sa.style == DisabledStyle.Disabled) //show disabled
                {
                    return DrawProperties.GetPropertyHeight(label, property);
                }
            }
        }
    }
}

