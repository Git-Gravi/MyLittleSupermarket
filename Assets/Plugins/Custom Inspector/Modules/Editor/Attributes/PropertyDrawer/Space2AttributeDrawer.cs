using CustomInspector.Extensions;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(Space2Attribute))]
    public class Space2AttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Space2Attribute sa = (Space2Attribute)attribute;
            position.y += sa.pixels;
            position.height -= sa.pixels;

            EditorGUI.BeginChangeCheck();
            DrawProperties.PropertyField(position, label, property);
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Space2Attribute sa = (Space2Attribute)attribute;
            return DrawProperties.GetPropertyHeight(label, property) + sa.pixels;
        }
    }
}
