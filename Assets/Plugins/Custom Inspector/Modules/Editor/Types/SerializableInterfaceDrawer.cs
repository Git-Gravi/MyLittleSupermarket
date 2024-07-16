using CustomInspector.Extensions;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceAttribute))]
    [CustomPropertyDrawer(typeof(SerializableInterface<>))]
    public class SerializableInterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType.IsGenericType
                && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(SerializableInterface<>))
            {
                var referenceProperty = property.FindPropertyRelative("serializedReference");
                Debug.Assert(referenceProperty != null);

                EditorGUI.BeginChangeCheck();
                var res = EditorGUI.ObjectField(position, label, referenceProperty.objectReferenceValue, fieldInfo.FieldType.GetGenericArguments()[0], true);
                if (EditorGUI.EndChangeCheck())
                {
                    referenceProperty.objectReferenceValue = res;
                    referenceProperty.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property, nameof(InterfaceAttribute) + " is only valid for SerializableInterface<>", MessageType.Error);
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType.IsGenericType
                && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(SerializableInterface<>))
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return DrawProperties.GetPropertyWithMessageHeight(label, property);
            }
        }
    }
}
