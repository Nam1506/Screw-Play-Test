using UnityEditor;
using UnityEngine;

namespace ActionCode.SpriteAnimation.Editor
{
    public static class SaveAnimation
    {
        private const float frameRate = 60F;
        private const float framesBySecond = 1F / frameRate;
        private const string extension = "anim";
        private const string defaultPath = "Assets/Animations";

        public static void SaveAsMecanim(AbstractAnimation animation)
        {
            var hasPrefab = TryLoadPrefabWithSprite(out GameObject prefab, out SpriteRenderer spriteRenderer);
            if (!hasPrefab) return;

            var animationClip = new AnimationClip { frameRate = frameRate };
            var spriteFrames = animation.frames;
            var spriteKeyFrames = new ObjectReferenceKeyframe[spriteFrames.Length + 1];
            var rendererPath = AnimationUtility.CalculateTransformPath(spriteRenderer.transform, prefab.transform);
            var binding = EditorCurveBinding.PPtrCurve(
                rendererPath,
                typeof(SpriteRenderer),
                "m_Sprite"
            );

            if (animation.loop)
            {
                animationClip.wrapMode = WrapMode.Loop;

                var settings = AnimationUtility.GetAnimationClipSettings(animationClip);
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(animationClip, settings);
            }

            var time = 0F;
            for (int i = 0; i < spriteFrames.Length; i++)
            {
                spriteKeyFrames[i] = new ObjectReferenceKeyframe
                {
                    time = time,
                    value = spriteFrames[i].sprite
                };
                time += spriteFrames[i].frames * framesBySecond;
            }

            spriteKeyFrames[spriteKeyFrames.Length - 1] = new ObjectReferenceKeyframe
            {
                time = time,
                value = spriteFrames[spriteFrames.Length - 1].sprite
            };

            AnimationUtility.SetObjectReferenceCurve(animationClip, binding, spriteKeyFrames);
            SaveClip(animationClip);
        }

        private static void SaveClip(AnimationClip clip)
        {
            var path = EditorUtility.SaveFilePanelInProject(
               title: "New Animation",
               defaultName: "NewAnimation." + extension,
               extension,
               message: "Creates a new animation asset",
               defaultPath
            ).Trim();

            var hasInvalidPath = string.IsNullOrEmpty(path);
            if (hasInvalidPath) return;

            AssetDatabase.CreateAsset(clip, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static bool TryLoadPrefabWithSprite(out GameObject prefab, out SpriteRenderer spriteRenderer)
        {
            const string extension = "prefab";
            const string directory = "Assets/Prefabs";

            prefab = null;
            spriteRenderer = null;

            var isInstrutionsOkay = EditorUtility.DisplayDialog(
                title: "Instructions",
                message: "Select a Prefab with a SpriteRenderer component on it. " +
                "The renderer can be attached on any of its children as well.",
                ok: "Ok",
                dialogOptOutDecisionType: DialogOptOutDecisionType.ForThisSession,
                dialogOptOutDecisionStorageKey: "ActionCode.SpriteAnimation.SelectSpritePrefabDecision"
            );

            if (!isInstrutionsOkay) return false;

            var path = EditorUtility.OpenFilePanel(
                title: "Select a Prefab with a SpriteRenderer component.",
                directory,
                extension
            );
            var hasInvalidPath = string.IsNullOrEmpty(path);

            if (hasInvalidPath) return false;

            prefab = PrefabUtility.LoadPrefabContents(path);
            if (prefab == null) return false;

            spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>(includeInactive: true);
            if (spriteRenderer == null) prefab.AddComponent<SpriteRenderer>();

            return true;
        }
    }
}