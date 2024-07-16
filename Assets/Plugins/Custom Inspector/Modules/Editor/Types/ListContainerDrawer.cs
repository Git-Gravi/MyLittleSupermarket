using CustomInspector.Extensions;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(ListContainer<>))]
    [CustomPropertyDrawer(typeof(ListContainerAttribute))]
    public class ListContainerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = property.FindPropertyRelative("values");
            Debug.Assert(list != null, "List not found in ListContainer");
            EditorGUI.BeginChangeCheck();
            DrawProperties.PropertyField(position, label, list);
            if (EditorGUI.EndChangeCheck())
                list.serializedObject.ApplyModifiedProperties();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var list = property.FindPropertyRelative("values");
            Debug.Assert(list != null, "List not found in ListContainer");
            return DrawProperties.GetPropertyHeight(label, list);
        }
    }
}
