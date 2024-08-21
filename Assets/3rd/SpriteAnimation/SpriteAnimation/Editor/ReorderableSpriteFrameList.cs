using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace ActionCode.SpriteAnimation.Editor
{
    /// <summary>
    /// Reorderable list for <see cref="SpriteFrame"/>.
    /// </summary>
    public sealed class ReorderableSpriteFrameList : ReorderableList, IDisposable
    {
        /// <summary>
        /// Event fired sprites are dropped inside this field.
        /// <para>The param is the first Sprite selected.</para>
        /// </summary>
        public event Action<Sprite> OnDropSprites;

        private readonly GUIStyle lableStyle;
        private readonly string spritePropertyName;
        private readonly string framesPropertyName;

        public ReorderableSpriteFrameList(SerializedObject serializedObject, string propertyName) :
            base(
                serializedObject,
                elements: serializedObject.FindProperty(propertyName),
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true
            )
        {
            spritePropertyName = nameof(SpriteFrame.sprite);
            framesPropertyName = nameof(SpriteFrame.frames);

            lableStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            lableStyle.normal.textColor = Color.white;

            drawHeaderCallback += HandleDrawHeader;
            drawElementCallback += HandleDrawElement;
            drawNoneElementCallback += HandleDrawNoneElement;
        }

        public void Dispose()
        {
            drawHeaderCallback -= HandleDrawHeader;
            drawElementCallback -= HandleDrawElement;
            drawNoneElementCallback -= HandleDrawNoneElement;
        }

        private void HandleDrawElement(Rect position, int index, bool isActive, bool isFocuse)
        {
            position.y += 2f;
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty spriteFrameProperty = serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(position, spriteFrameProperty);
        }

        private void HandleDrawHeader(Rect position)
        {
            var leftPosition = new Rect(
                position.x,
                position.y,
                position.width * SpriteFrameDrawer.SPRITE_WIDTH_RATIO,
                position.height
            );
            var rightPosition = new Rect(
                leftPosition.xMax,
                position.y,
                position.width * (1F - SpriteFrameDrawer.SPRITE_WIDTH_RATIO),
                position.height
            );

            EditorGUI.LabelField(leftPosition, "Sprites", lableStyle);
            EditorGUI.LabelField(rightPosition, "Frames", lableStyle);
        }

        private void HandleDrawNoneElement(Rect position)
        {
            EditorGUI.LabelField(position, "Drag and drop Sprites here.", lableStyle);

            var evt = Event.current;
            var isInsideDropPosition = position.Contains(evt.mousePosition);

            if (!isInsideDropPosition) return;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                    EvaluateDragging();
                    break;

                case EventType.DragPerform:
                    PerformDropping();
                    break;
            }
        }

        private void PerformDropping()
        {
            int index = 0;
            DragAndDrop.AcceptDrag();

            foreach (var reference in DragAndDrop.objectReferences)
            {
                if (reference == null) continue;

                if (reference is Sprite)
                {
                    AddSpriteReference(reference, index);
                    index++;
                }
                else if (reference is Texture2D texture)
                {
                    var sprites = LoadAllSprites(texture);
                    for (int i = 0; i < sprites.Length; i++)
                    {
                        AddSpriteReference(sprites[i], index);
                        index++;
                    }
                }
            }

            var firstReference = DragAndDrop.objectReferences[0];
            var sprite = firstReference as Sprite;
            if (sprite == null && firstReference is Texture2D dragTexture)
            {
                var sprites = LoadAllSprites(dragTexture);
                sprite = sprites.Length > 0 ? sprites[0] as Sprite : default;
            }

            OnDropSprites?.Invoke(sprite);
        }

        private void AddSpriteReference(Object sprite, int index)
        {
            const int defaultFramesCount = 3;

            serializedProperty.InsertArrayElementAtIndex(index);
            SerializedProperty serializedSpriteFrame = serializedProperty.GetArrayElementAtIndex(index);

            serializedSpriteFrame.FindPropertyRelative(spritePropertyName).objectReferenceValue = sprite;
            serializedSpriteFrame.FindPropertyRelative(framesPropertyName).intValue = defaultFramesCount;
        }

        private static void EvaluateDragging()
        {
            DragAndDrop.visualMode = IsValidObjectReference(DragAndDrop.objectReferences[0]) ?
                DragAndDropVisualMode.Copy :
                DragAndDropVisualMode.Rejected;
        }

        private static bool IsValidObjectReference(Object reference) =>
            reference is Sprite || reference is Texture2D;

        private static Object[] LoadAllSprites(Texture2D texture)
        {
            var path = AssetDatabase.GetAssetPath(texture);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            return assets.Where(asset => asset is Sprite).ToArray();
        }
    }
}