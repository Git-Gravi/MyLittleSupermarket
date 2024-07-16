using CustomInspector.Extensions;
using CustomInspector.Helpers;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    /// <summary>
    /// Draws the field name and behind a custom errorMessage
    /// </summary>
    [CustomPropertyDrawer(typeof(MessageBoxAttribute))]
    public class MessageBoxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MessageBoxAttribute hv = (MessageBoxAttribute)attribute;

            EditorGUI.BeginChangeCheck();
            DrawProperties.DrawPropertyWithMessage(position, label, property, hv.content,
                InternalEditorStylesConvert.ToUnityMessageType(hv.type));
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return DrawProperties.GetPropertyWithMessageHeight(label, property);
        }
    }
}