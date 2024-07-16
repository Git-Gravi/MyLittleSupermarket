using CustomInspector.Extensions;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string[] sceneNames = GetSceneNames((SceneAttribute)attribute);
            GUIContent[] guiContents = sceneNames.Select((n, i) => new GUIContent($"{n} ({i})")).ToArray();

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.Popup(position, label, property.intValue, guiContents);
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                int index = Array.IndexOf(sceneNames, property.stringValue);
                if (index == -1)
                {
                    index = 0;
                    property.stringValue = sceneNames[0];
                    property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }

                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(position, label, index, guiContents);
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = sceneNames[index];
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property,
                                                    nameof(SceneAttribute) + " is only available on Integers and Strings", MessageType.Error);
            }

            static string[] GetSceneNames(SceneAttribute attribute)
            {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                string[] sceneNames = new string[scenes.Length];
                for (int i = 0; i < scenes.Length; i++)
                {
                    if (attribute.useFullPath)
                        sceneNames[i] = scenes[i].path;
                    else
                        sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
                }
                return sceneNames;
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}