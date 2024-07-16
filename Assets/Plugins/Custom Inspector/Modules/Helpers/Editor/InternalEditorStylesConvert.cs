using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Helpers
{
    public static class InternalEditorStylesConvert
    {
        public static Color DarkerBackground
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                    return new Color(0.12f, 0.12f, 0.12f); //Default Background is Color(0.22f, 0.22f, 0.22f)
                else
                    return new Color(0.66f, 0.66f, 0.66f); //Default Background is Color(0.76f, 0.76f, 0.76f)
            }
        }
        public static GUIContent IconNameToGUIContent(string iconName)
        {
            Texture2D iconTexture = (Texture2D)typeof(EditorGUIUtility)
                            .GetMethod("LoadIcon", BindingFlags.NonPublic | BindingFlags.Static)
                            .Invoke(null, new object[] { iconName });

            if (iconTexture != null)
                return new GUIContent() { image = iconTexture };
            else
                return new GUIContent(iconName);
        }
        public static MessageType ToUnityMessageType(MessageBoxType type)
        {
            return type switch
            {
                MessageBoxType.None => MessageType.None,
                MessageBoxType.Info => MessageType.Info,
                MessageBoxType.Warning => MessageType.Warning,
                MessageBoxType.Error => MessageType.Error,
                _ => throw new NotImplementedException(type.ToString())
            };
        }
    }
}
