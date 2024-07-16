using CustomInspector.Extensions;
using CustomInspector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var apa = (AnimatorParameterAttribute)attribute;
            PropInfo info = GetInfo(property, apa);
            if (info.errorMessage != null)
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property, info.errorMessage, MessageType.Error);
            }
            else
            {
                List<string> paramNames = info.parameterNames(property);
                if (paramNames == null)
                {
                    property.stringValue = "<missing>";
                    DrawProperties.DrawPropertyWithMessage(position,
                                                            label,
                                                            property,
                                                            $"AnimatorParameter: Animator or AnimatorController on {apa.animatorPath} is null. Please fill it in the inspector",
                                                            MessageType.Error,
                                                            disabled: true);
                }
                else if (paramNames.Any())
                {
                    int index = paramNames.IndexOf(property.stringValue);
                    if (index == -1) //not found
                    {
                        index = 0;
                        property.stringValue = paramNames[0];
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUI.BeginChangeCheck();
                    index = EditorGUI.Popup(position, label, index, paramNames.Select(name => new GUIContent(name)).ToArray());
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.stringValue = paramNames[index];
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                else //no params existing
                {
                    property.stringValue = "<invalid>";
                    DrawProperties.DrawPropertyWithMessage(position,
                                                            label,
                                                            property,
                                                            $"AnimatorParameter: {apa.animatorPath} has no animator parameters.",
                                                            MessageType.Error,
                                                            disabled: true);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            PropInfo info = GetInfo(property, (AnimatorParameterAttribute)attribute);
            if (info.errorMessage != null)
            {
                return DrawProperties.GetPropertyWithMessageHeight(label, property);
            }
            else
            {
                var paramNames = info.parameterNames(property);
                if (paramNames != null && paramNames.Any())
                    return EditorGUIUtility.singleLineHeight;
                else
                    return DrawProperties.GetPropertyWithMessageHeight(label, property);
            }
        }

        readonly static Dictionary<PropertyIdentifier, PropInfo> savedInfo = new();
        static PropInfo GetInfo(SerializedProperty property, AnimatorParameterAttribute attribute)
        {
            PropertyIdentifier id = new PropertyIdentifier(property);

            if (!savedInfo.TryGetValue(id, out PropInfo info))
            {
                info = new(property, attribute);
                savedInfo.Add(id, info);
            }
            return info;
        }

        class PropInfo
        {
            public readonly string errorMessage;
            public readonly Func<SerializedProperty, List<string>> parameterNames = null;
            public PropInfo(SerializedProperty property, AnimatorParameterAttribute attribute)
            {
                SerializedProperty prop;
                PropertyValues.IFindProperties owner = property.GetOwnerAsFinder();
                try
                {
                    prop = owner.FindPropertyRelative(attribute.animatorPath);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                    return;
                }
                if (prop == null)
                {
                    errorMessage = $"AnimatorParameter: {attribute.animatorPath} was not found on {owner.Name}";
                    return;
                }

                Type type = DirtyValue.GetType(prop);
                if (type == typeof(Animator))
                {
                    parameterNames = p =>
                    {
                        SerializedProperty animator = p.GetOwnerAsFinder().FindPropertyRelative(attribute.animatorPath);
                        Animator value = animator.GetValue() as Animator;
                        if (value != null)
                            return value.parameters?.Select(p => p.name).ToList(); //they are null no controller is assigned
                        else
                            return null;
                    };
                }
                else if (type == typeof(AnimatorController))
                {
                    parameterNames = p =>
                    {
                        SerializedProperty animatorController = p.GetOwnerAsFinder().FindPropertyRelative(attribute.animatorPath);
                        AnimatorController value = animatorController.GetValue() as AnimatorController;
                        if (value != null)
                            return value.parameters.Select(p => p.name).ToList();
                        else
                            return null;
                    };
                }
                else
                {
                    errorMessage = $"AnimatorParameter: Type {type} is invalid. {attribute.animatorPath} in {owner.Name} must be of type Animator or AnimatorController";
                }
            }
        }
    }
}
