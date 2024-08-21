using UnityEditor;
using UnityEngine;

namespace ActionCode.SpriteAnimation.Editor
{
    [CustomEditor(typeof(AbstractAnimation), editorForChildClasses: true)]
    public sealed class AbstractAnimationEditor : UnityEditor.Editor
    {
        private AbstractAnimation animation;
        private SpriteAnimationSceneWindow animationWindow;
        private ReorderableSpriteFrameList reorderableFrameList;

        private readonly string framesPropertyName;

        public AbstractAnimationEditor()
        {
            framesPropertyName = nameof(AbstractAnimation.frames);
        }

        private void OnEnable()
        {
            animation = (AbstractAnimation)target;

            animationWindow = new SpriteAnimationSceneWindow(animation);
            reorderableFrameList = new ReorderableSpriteFrameList(serializedObject, framesPropertyName);

            reorderableFrameList.OnDropSprites += HandleDropSprites;
        }

        private void OnDisable()
        {
            reorderableFrameList.OnDropSprites -= HandleDropSprites;

            animationWindow.Dispose();
            reorderableFrameList.Dispose();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, framesPropertyName);

            EditorGUILayout.Space();
            reorderableFrameList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI() => animationWindow.OnSceneGUI();

        private void HandleDropSprites(Sprite sprite)
        {
            var hasSpriteRenderer = animation.TryGetComponent(out SpriteRenderer spriteRenderer);
            if (hasSpriteRenderer)
            {
                var noSprite = spriteRenderer.sprite == null;
                if (noSprite) spriteRenderer.sprite = sprite;
                return;
            }

#if UI_MODULE
            var hasImageRenderer = animation.TryGetComponent(out UnityEngine.UI.Image imageRenderer);
            if (hasImageRenderer)
            {
                var noSprite = imageRenderer.sprite == null;
                if (noSprite) imageRenderer.sprite = sprite;
            }
#endif
        }
    }
}