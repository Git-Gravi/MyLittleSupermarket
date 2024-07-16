using CustomInspector.Extensions;
using CustomInspector.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(RichTextAttribute))]
    public class RichTextAttributeDrawer : PropertyDrawer
    {
        const float toggleWidth = 17;

        static Vector2 scrollPos = Vector2.zero;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            (StyleInfo general, GUIStyle gUI) styleInfo = GetStyleInfo(property, fieldInfo);


            if (property.propertyType != SerializedPropertyType.String)
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property, $"{nameof(RichTextAttribute)} only valid on strings.", MessageType.Error);
                return;
            }

            Rect labelRect = new(position)
            {
                width = EditorGUIUtility.labelWidth,
                height = EditorGUIUtility.singleLineHeight,
            };
            if (label.text == "")
            {
                //still show the foldout that uses space, if no space (position.x) is available
                labelRect.width = Mathf.Clamp(18 - position.x, 0, 18);
            }

            //The boolean below
            Rect infoRect = new(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };
            infoRect.y = position.y + position.height - infoRect.height - EditorGUIUtility.standardVerticalSpacing;


            Rect textRect = new()
            {
                x = labelRect.x + labelRect.width + 2,
                y = position.y,
                width = position.width - labelRect.width,
                height = position.height,
            };

            styleInfo.general.lastWidth = Mathf.Max(textRect.width, 10);

            if (property.isExpanded) //draw info
                textRect.height = position.height - (infoRect.height + EditorGUIUtility.standardVerticalSpacing);

            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label);


            EditorGUI.BeginChangeCheck();
            using (new NewIndentLevel(0))
            {
                float neededHeight = styleInfo.gUI.CalcHeight(new GUIContent(property.stringValue), textRect.width - Common.scrollbarThickness);

                if (neededHeight <= textRect.height)
                {
                    property.stringValue = styleInfo.general.preventEnterConfirm ?
                                    EditorGUI.TextArea(textRect, property.stringValue, styleInfo.gUI)
                                    : EditorGUI.TextField(textRect, property.stringValue, styleInfo.gUI);
                }
                else
                {
                    Rect innerPosition = new Rect(0, 0, textRect.width - Common.scrollbarThickness, neededHeight); //position in the scrollbar

                    using (var scrollScope = new GUI.ScrollViewScope(textRect, scrollPos, innerPosition))
                    {
                        scrollPos = scrollScope.scrollPosition;

                        property.stringValue = styleInfo.general.preventEnterConfirm ?
                                        EditorGUI.TextArea(innerPosition, property.stringValue, styleInfo.gUI)
                                        : EditorGUI.TextField(innerPosition, property.stringValue, styleInfo.gUI);
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            if (property.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope(1))
                    property.isExpanded = !EditorGUI.Toggle(infoRect, new GUIContent("Use Rich text", "Richttext is currently disabled for this textfield. Click the toggle to active it."), !property.isExpanded);
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
                return DrawProperties.GetPropertyWithMessageHeight(label, property);

            (StyleInfo general, GUIStyle gUI) style = GetStyleInfo(property, fieldInfo);
            string[] contentLines = property.stringValue.Split('\n');

            string[] filled;
            if (contentLines.Length < style.general.minLines)
            {
                filled = new string[style.general.minLines];
                Array.Copy(contentLines, filled, contentLines.Length);
            }
            else if (contentLines.Length > style.general.maxLines)
            {
                filled = new string[style.general.maxLines];
                Array.Copy(contentLines, filled, style.general.maxLines);
            }
            else
                filled = contentLines;


            float height = style.gUI.CalcHeight(new GUIContent(string.Join('\n', filled)), style.general.lastWidth);
            if (height > 1000) //cap, if the user mad a mistake
                height = 1000;
            if (property.isExpanded)
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return height;
        }


        static Dictionary<ExactPropertyIdentifier, StyleInfo> styleInfos = new();
        static (StyleInfo general, GUIStyle gUI) GetStyleInfo(SerializedProperty property, FieldInfo fieldInfo)
        {
            ExactPropertyIdentifier id = new(property);
            if (!styleInfos.TryGetValue(id, out var styleInfo))
            {
                styleInfo = new(fieldInfo);
                styleInfos.Add(id, styleInfo);
            }
            GUIStyle gUIStyle = new(GUI.skin.textField)
            {
                richText = !property.isExpanded,
                wordWrap = styleInfo.wordWrap,
            };
            return (styleInfo, gUIStyle);
        }
        class StyleInfo
        {
            /// <summary>
            /// If pressing enter should insert '\n' instead of confirming selection
            /// </summary>
            public readonly bool preventEnterConfirm = false;
            /// <summary>
            /// If a scrollbar should be added when not all text is showing
            /// </summary>
            public readonly bool insertScrollBar = false;
            /// <summary>
            /// If words should be moved in next line if available width is too low
            /// </summary>
            public readonly bool wordWrap = false;
            /// <summary>
            /// Min and max height of input-box measured in lines
            /// </summary>
            public readonly int minLines = 1, maxLines = 1; //keep in mind that a line-height's can vary because of richText - we just take the first n lines as the height


            public float lastWidth = 200;

            public StyleInfo(FieldInfo fieldInfo)
            {
                var multilineA = fieldInfo.GetCustomAttribute<MultilineAttribute>();
                if (multilineA != null)
                {
                    wordWrap = false;
                    preventEnterConfirm = true;
                    minLines = maxLines = multilineA.lines;
                    return;
                }

                var textAreaA = fieldInfo.GetCustomAttribute<TextAreaAttribute>();
                if (textAreaA != null)
                {
                    wordWrap = true;
                    preventEnterConfirm = true;
                    minLines = textAreaA.minLines;
                    maxLines = textAreaA.maxLines;
                    return;
                }
            }
        }
        class ExactPropertyIdentifier //should also differentiate between two instances of same object shown in two inspector windows (because we store inspector-width)
        {
            readonly SerializedObject targetObject;
            readonly string propertyPath;
            public ExactPropertyIdentifier(SerializedProperty property)
            {
                targetObject = property.serializedObject;
                propertyPath = property.propertyPath;
            }
            public override bool Equals(object o)
            {
                if (o is ExactPropertyIdentifier other)
                {
                    return targetObject == other.targetObject && propertyPath == other.propertyPath;
                }
                else return false;
            }
            public override int GetHashCode() => HashCode.Combine(targetObject, propertyPath);
        }
    }
}
