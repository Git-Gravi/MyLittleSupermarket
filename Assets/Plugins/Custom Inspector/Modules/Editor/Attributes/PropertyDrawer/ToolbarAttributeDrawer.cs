using UnityEditor;
using UnityEngine;


namespace CustomInspector.Editor
{

    [CustomPropertyDrawer(typeof(ToolbarAttribute))]
    public class ToolbarAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ToolbarAttribute ta = (ToolbarAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.Boolean)
            {
                Rect rect = CapSize(position, 2);

                EditorGUI.BeginChangeCheck();
                bool res = GUI.Toolbar(rect, property.boolValue ? 0 : 1, new string[] { property.name, "None" }) == 0;
                if (EditorGUI.EndChangeCheck())
                    property.boolValue = res;
            }
            else if (property.propertyType == SerializedPropertyType.Enum)
            {
                string[] names = property.enumNames;
                Rect rect = CapSize(position, names.Length);

                EditorGUI.BeginChangeCheck();
                int res = GUI.Toolbar(rect, property.enumValueIndex, names);
                if (EditorGUI.EndChangeCheck())
                {
                    property.enumValueIndex = res;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUI.HelpBox(position, $"({property.propertyType} not supported)", MessageType.Error);
            }

            Rect CapSize(Rect position, int choicesAmount) //place it a bit smalle in the middle
            {
                position.y += ta.space;
                position.height = ta.height;

                float maxWidth = choicesAmount * ta.maxWidth;

                if (position.width <= maxWidth)
                    return position;

                else
                    return new(position.x + (position.width - maxWidth) / 2,
                             position.y + ta.space,
                             maxWidth, ta.height);
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ToolbarAttribute ta = (ToolbarAttribute)attribute;
            return ta.height + 2 * ta.space;
        }
    }
}