using UnityEditor;
using UnityEngine;

namespace ActionCode.SpriteAnimation.Editor
{
    /// <summary>
    /// Custom property drawer for <see cref="SpriteFrame"/>.
    /// <para>Draws the Sprite and Frame properties next to each other, in the same line.</para>
    /// </summary>
    [CustomPropertyDrawer(typeof(SpriteFrame))]
    public class SpriteFrameDrawer : PropertyDrawer
    {
        public const float SPRITE_WIDTH_RATIO = 0.85F;
        public const float FIELDS_SEPARATION = 4F;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty spriteProperty = property.FindPropertyRelative("sprite");
            SerializedProperty framesProperty = property.FindPropertyRelative("frames");

            var emptyLabel = GUIContent.none;
            bool isFromArray = property.propertyPath.Contains("Array");

            if (isFromArray) label.text = string.Empty;

            label = EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var spriteFieldPosition = new Rect(position.x, position.y, position.width * SPRITE_WIDTH_RATIO, position.height);
            var framesFieldPosition = new Rect(
                spriteFieldPosition.xMax + FIELDS_SEPARATION,
                position.y,
                position.width - spriteFieldPosition.width - FIELDS_SEPARATION,
                position.height
            );

            EditorGUI.ObjectField(spriteFieldPosition, spriteProperty, emptyLabel);
            EditorGUI.PropertyField(framesFieldPosition, framesProperty, emptyLabel);

            EditorGUI.EndProperty();
        }
    }
}