using CustomInspector.Extensions;
using UnityEditor;
using UnityEngine;

namespace CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleAttributeDrawer : PropertyDrawer
    {
        const float underlineThickness = 1;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TitleAttribute t = (TitleAttribute)attribute;

            GUIContent title = new(t.content, t.tooltip);

            //spacing
            float upperSpacing = Mathf.Max(t.upperSpacing, 0);
            position.y += upperSpacing;
            position.height -= upperSpacing;

            //style
            GUIStyle titleStyle = new(EditorStyles.boldLabel)
            {
                fontSize = t.fontSize,
                alignment = StylesConvert.AlignmentToAnchor(t.alignment),
            };

            //Draw title
            Rect titleRect = EditorGUI.IndentedRect(position);
            using (new NewIndentLevel(0))
            {
                Vector2 titleSize = titleStyle.CalcSize(title);
                titleRect.height = titleSize.y;

                EditorGUI.LabelField(titleRect, title, titleStyle);

                if (t.underlined)
                {
                    Rect lineRect = new()
                    {
                        y = titleRect.y + titleRect.height,
                        width = titleSize.x,
                        height = underlineThickness,

                    };
                    switch (t.alignment)
                    {
                        case TextAlignment.Left:
                            lineRect.x = titleRect.x;
                            break;
                        case TextAlignment.Center:
                            lineRect.x = titleRect.x + (titleRect.width - titleSize.x) / 2f - 1f; //-1 is a untiy fix
                            break;
                        case TextAlignment.Right:
                            lineRect.x = titleRect.x + (titleRect.width - titleSize.x);
                            break;
                    }
                    EditorGUI.DrawRect(lineRect, StylesConvert.ToColor(FixedColor.CloudWhite));
                    position.y += underlineThickness;
                }


                //Draw propertyfield
                position.y += titleRect.height + EditorGUIUtility.standardVerticalSpacing;
                position.height -= titleRect.height + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.BeginChangeCheck();
            DrawProperties.PropertyField(position, label, property);
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            TitleAttribute t = (TitleAttribute)attribute;

            //style
            GUIStyle titleStyle = new(EditorStyles.boldLabel);
            titleStyle.fontSize = t.fontSize;

            //Title
            float labelHeight = titleStyle.CalcSize(new GUIContent(t.content, t.tooltip)).y;

            if (t.underlined)
                labelHeight += underlineThickness;

            return Mathf.Max(t.upperSpacing, 0)
                    + labelHeight + EditorGUIUtility.standardVerticalSpacing
                    + EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
