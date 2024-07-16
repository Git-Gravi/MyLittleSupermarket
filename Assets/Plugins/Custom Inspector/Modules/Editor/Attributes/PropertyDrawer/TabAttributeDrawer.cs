using CustomInspector.Extensions;
using CustomInspector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(TabAttribute))]
    public class TabAttributeDrawer : PropertyDrawer
    {
        /// <summary> Distance between outer rect and inner rects </summary>
        const float outerSpacing = 15;
        const float toolbarHeight = 25;




        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            (PropInfo info, HorizontalGroupInfos horizontalGroup) = GetInfo(property);

            if (info.errorMessage != null)
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property, info.errorMessage, MessageType.Error);
                return;
            }
            if (horizontalGroup?.errorMessage != null)
            {
                DrawProperties.DrawPropertyWithMessage(position, label, property, horizontalGroup.errorMessage, MessageType.Error);
                return;
            }

            bool isVisible = info.tabGroupIndex == GetSelected(fieldInfo.DeclaringType);

            //clip not seen
            if (!isVisible && !info.isAllFirst)
                return;

            float thisBlankPropertyHeight = DrawProperties.GetPropertyHeight(label, property);
            // only the first of horizontal group should draw, because in a horizontal alignment the new drawn would override the old
            // -> because tab is always centered and they cannot draw next to each other





            if (horizontalGroup == null || horizontalGroup.isGroupFirst)
            {
                //draw tab
                using (new NewIndentLevel(0))
                {
                    //Background
                    position.height = GetPropertyHeight(property, label);

                    float halfSpacing = EditorGUIUtility.standardVerticalSpacing / 2f;
                    if (!info.isAllFirst)
                    {
                        //mit oberen verbinden
                        position.y -= halfSpacing;
                        position.height += halfSpacing;
                    }


                    //mit unterem verbinden
                    if ((info.isAllFirst && !isVisible) || !info.isTabGroupLast) //allFirst (the toolbar) has to connect all
                        position.height += halfSpacing;

                    float groupHeight = 0; // only the most to the right side property contains the max height of all
                    if (horizontalGroup != null
                        && horizontalGroup.groupLastPath != property.propertyPath) //if group has only one member, he can be first and last and then doesnt need extra height, because he is already most right 
                    {
                        SerializedProperty last = property.serializedObject.FindProperty(horizontalGroup.groupLastPath);
                        groupHeight = DrawProperties.GetPropertyHeight(last) + EditorGUIUtility.standardVerticalSpacing;
                    }
                    position.height += groupHeight;

                    EditorGUI.DrawRect(position, InternalEditorStylesConvert.DarkerBackground);

                    position.height -= groupHeight;

                    //verbindung oben ende
                    if (!info.isAllFirst)
                        position.y += halfSpacing;

                    //abstand zu oben
                    if (info.isAllFirst)
                        position.y += EditorGUIUtility.standardVerticalSpacing;

                    //sides distance
                    position = ExpandRectWidth(position, -outerSpacing);
                    //Toolbar
                    if (info.isAllFirst)
                    {
                        Rect tRect = new(position)
                        {
                            height = toolbarHeight
                        };
                        GUIContent[] guiContents = GetTabGroupNames(fieldInfo.DeclaringType);
                        EditorGUI.BeginChangeCheck();
                        int res = GUI.Toolbar(tRect, GetSelected(fieldInfo.DeclaringType), guiContents);
                        if (EditorGUI.EndChangeCheck())
                            SetSelected(fieldInfo.DeclaringType, res);

                        position.y = tRect.y + toolbarHeight + outerSpacing + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
            else
                //sides distance
                position = ExpandRectWidth(position, -outerSpacing);

            //Draw Property
            if (isVisible)
            {
                position.height = thisBlankPropertyHeight;
                EditorGUI.BeginChangeCheck();
                DrawProperties.PropertyField(position, label, property);
                if (EditorGUI.EndChangeCheck())
                    property.serializedObject.ApplyModifiedProperties();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            (PropInfo info, HorizontalGroupInfos horizontalGroup) = GetInfo(property);

            if (info.errorMessage != null || horizontalGroup?.errorMessage != null)
                return DrawProperties.GetPropertyWithMessageHeight(label, property);

            bool isVisible = info.tabGroupIndex == GetSelected(fieldInfo.DeclaringType);

            float totalHeight = isVisible ?
                        DrawProperties.GetPropertyHeight(label, property) : -EditorGUIUtility.standardVerticalSpacing;

            if (info.isAllFirst)
            {
                totalHeight += EditorGUIUtility.standardVerticalSpacing
                    + toolbarHeight
                    + outerSpacing
                    + EditorGUIUtility.standardVerticalSpacing;
            }
            if (info.isTabGroupLast && isVisible)
                totalHeight += outerSpacing;
            return totalHeight;
        }

        Rect ExpandRectWidth(Rect rect, float value)
        {
            rect.x -= value;
            rect.width += 2 * value;
            return rect;
        }


        /// <summary> all group names for each declaringType </summary>
        readonly static Dictionary<Type, GUIContent[]> tabGroupNames = new();
        static GUIContent[] GetTabGroupNames(Type t)
        {
            if (!tabGroupNames.TryGetValue(t, out GUIContent[] res))
            {
                var names = FindTabGroupNames(t).ToArray();
                res = names.Select(_ => StylesConvert.ToInternalIconName(_))
                           .Select(_ => InternalEditorStylesConvert.IconNameToGUIContent(_))
                           .ToArray();

                tabGroupNames.Add(t, res);
            }
            return res;
        }
        static IEnumerable<string> FindTabGroupNames(Type classType)
        {
            IEnumerable<FieldInfo> fields = classType.GetAllSerializableFields(alsoFromBases: false);
            IEnumerable<TabAttribute> allAttr = fields.Select(_ => _.GetCustomAttribute<TabAttribute>()).Where(_ => _ is not null);
            return allAttr.Select(_ => _.groupName).Distinct();
        }


        ///<summary>DeclaringType to selected group</summary>
        readonly static Dictionary<Type, int> selected = new();
        static int GetSelected(Type classType)
        {
            if (selected.TryGetValue(classType, out int value))
                return value;
            else
                return 0;
        }
        void SetSelected(Type classType, int value)
        {
            if (!selected.TryAdd(classType, value))
                selected[classType] = value;
        }
        readonly static Dictionary<PropertyIdentifier, (PropInfo, HorizontalGroupInfos)> savedInfos = new();

        (PropInfo, HorizontalGroupInfos) GetInfo(SerializedProperty property)
        {
            PropertyIdentifier id = new(property);
            if (!savedInfos.TryGetValue(id, out (PropInfo, HorizontalGroupInfos) res))
            {
                res = (new PropInfo(fieldInfo, (TabAttribute)attribute),
                            HorizontalGroupInfos.CreateNewHorizontalGroupInfos(property, fieldInfo));
                savedInfos.Add(id, res);
            }
            return res;
        }
        class PropInfo
        {
            public int tabGroupIndex;
            /// <summary> Defines, if it has to draw the toolbar </summary>
            public bool isAllFirst;
            /// <summary> Defines, if it has to draw space on the end </summary>
            public bool isTabGroupLast;
            public string errorMessage = null;

            public PropInfo(FieldInfo fieldInfo, TabAttribute attribute)
            {
                //In array/list nothing matters - just error it | other enumerable like one Transform are allowed
                if (fieldInfo.FieldType.IsArray //is array
                    || (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))) //is list
                {
                    errorMessage = "TabAttribute not valid on list elements." +
                        $"\nReplace field-type with {typeof(ListContainer<>).FullName} to apply all attributes to whole list instead of to single elements";
                    return;
                }

                //-
                (isAllFirst, isTabGroupLast) = GetMyPosition(fieldInfo, attribute);
                tabGroupIndex = FindTabGroupNames(fieldInfo.DeclaringType).ToList().IndexOf(attribute.groupName);
                Debug.Assert(tabGroupIndex != -1, $"No group for {fieldInfo.Name} found");


                static (bool isAllFirst, bool isGroupLast) GetMyPosition(FieldInfo fieldInfo, TabAttribute attribute)
                {
                    bool isAllFirst = false;
                    bool isGroupLast = false;

                    var allProps = fieldInfo.DeclaringType.GetAllSerializableFields(alsoFromBases: false)
                        .Select(f => (fieldInfo: f, attr: f.GetCustomAttribute<TabAttribute>()))
                        .Where(ta => ta.attr != null);

                    (FieldInfo fieldInfo, TabAttribute ta) first = allProps.First();
                    if (first.fieldInfo.Name == fieldInfo.Name) // we trust on unity not allowing same same in derived classes (the same field accessed from a derived class seemed to return false on '==' - but i am not sure, so getting the name is safer)
                        isAllFirst = true;

                    var propsInMyGroup = allProps.Where(_ => _.attr.groupName == attribute.groupName);

                    if (!propsInMyGroup.Any())
                    {
                        Debug.LogError($"No properties found for Tab-group '{attribute.groupName}' in '{fieldInfo.DeclaringType}'" +
                                       $"\nError at displaying {fieldInfo.Name}");
                        return (false, false);
                    }
                    var last_iG = propsInMyGroup.Last();
                    if (last_iG.fieldInfo.Name == fieldInfo.Name)
                        isGroupLast = true;

                    return (isAllFirst, isGroupLast);
                }
            }


        }
        class HorizontalGroupInfos
        {
            /// <summary>
            /// if he starts the horizontal alignment
            /// </summary>
            public readonly bool isGroupFirst;
            /// <summary>
            /// the path of the last one in the group
            /// </summary>
            public readonly string groupLastPath;

            public readonly string errorMessage = null;

            private HorizontalGroupInfos(bool isGroupFirst, string groupLastPath)
            { this.isGroupFirst = isGroupFirst; this.groupLastPath = groupLastPath; }

            public static HorizontalGroupInfos CreateNewHorizontalGroupInfos(SerializedProperty property, FieldInfo fieldInfo)
            {
                HorizontalGroupInfos res;

                if (fieldInfo.GetCustomAttribute<HorizontalGroupAttribute>() == null)
                {
                    res = null;
                }
                else
                {
                    HorizontalGroupAttributeDrawer.GroupMember[] group
                                = HorizontalGroupAttributeDrawer.GroupMember.GetGroup(property);

                    Debug.Assert(group.Length > 0, "Horizontal-group could not be found");


                    bool isFirst = group[0].id.propertyPath == property.propertyPath;
                    string groupLast = group[^1].id.propertyPath;

                    res = new HorizontalGroupInfos(isFirst, groupLast);
                }

                return res;
            }
        }
    }
}