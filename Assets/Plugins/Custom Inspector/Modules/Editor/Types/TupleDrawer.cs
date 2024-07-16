using CustomInspector.Extensions;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(TupleAttribute))]
    [CustomPropertyDrawer(typeof(SerializableTuple<,>))]
    [CustomPropertyDrawer(typeof(SerializableTuple<,,>))]
    [CustomPropertyDrawer(typeof(SerializableTuple<,,,>))]
    [CustomPropertyDrawer(typeof(SerializableTuple<,,,,>))]
    [CustomPropertyDrawer(typeof(SerializableTuple<,,,,,>))]
    public class TupleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new NewIndentLevel(EditorGUI.indentLevel))
            {
                if (!string.IsNullOrEmpty(label.text))
                {
                    position.height = EditorGUIUtility.singleLineHeight;
                    if (label.text == "Item 1" || label.text == "Item 2") //bug fix
                        label = new(PropertyConversions.NameFormat(property.name), property.tooltip);
                    EditorGUI.LabelField(position, label);
                    position.y += position.height;
                    EditorGUI.indentLevel++;
                }
                position = EditorGUI.IndentedRect(position);
                using (new NewIndentLevel(0))
                {
                    EditorGUI.BeginChangeCheck();
                    foreach (SerializedProperty prop in property.GetAllVisibleProperties(false))
                    {
                        DrawProperties.PropertyFieldWithoutLabel(position, prop);
                    }
                    if (EditorGUI.EndChangeCheck())
                        property.serializedObject.ApplyModifiedProperties();
                }
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var allProps = property.GetAllVisibleProperties(false);
            if (!string.IsNullOrEmpty(label.text))
                return EditorGUIUtility.singleLineHeight + allProps.Max(_ => DrawProperties.GetPropertyHeight(_));
            else
                return allProps.Max(_ => DrawProperties.GetPropertyHeight(_));
        }
    }
}
