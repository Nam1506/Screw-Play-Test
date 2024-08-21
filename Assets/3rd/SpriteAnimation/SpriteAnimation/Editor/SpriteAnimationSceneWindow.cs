using System;
using UnityEngine;
using UnityEditor;

namespace ActionCode.SpriteAnimation.Editor
{
    /// <summary>
    /// Scene Window Toolbar for Sprite Animation.
    /// </summary>
    public sealed class SpriteAnimationSceneWindow : IDisposable
    {
        private const float FRAME_RATE_TIME = 1F / 60F;

        private double lastRecordedTime;

        private readonly GUIContent title;
        private readonly GUIContent play;
        private readonly GUIContent next;
        private readonly GUIContent previous;
        private readonly GUIContent restart;
        private readonly GUIContent stop;
        private readonly GUIContent loop;
        private readonly GUIContent saveAnimation;
        private readonly AbstractAnimation animation;

        private readonly Vector2 size = new Vector2(225F, 120F);
        private readonly Vector2 padding = new Vector2(22F, 60F);

        public SpriteAnimationSceneWindow(AbstractAnimation animation)
        {
            this.animation = animation;

            title = EditorGUIUtility.TrTextContent("Sprite Animation");
            play = EditorGUIUtility.TrTextContent("", "Plays the animation", "Animation.Play");
            next = EditorGUIUtility.TrTextContent("", "Next frame", "Animation.NextKey");
            previous = EditorGUIUtility.TrTextContent("", "Previous frame", "Animation.PrevKey");
            loop = EditorGUIUtility.TrTextContent("", "Loops the animation", "playLoopOff");
            restart = EditorGUIUtility.TrTextContent("Restart", "Restarts the animation");
            stop = EditorGUIUtility.TrTextContent("Stop", "Stops the animation");
            saveAnimation = EditorGUIUtility.TrTextContent(
                "Save Animation",
                "Saves this as Mecanim Animation asset."
            );

            EditorApplication.update += OnSceneUpdate;
        }

        public void Dispose()
        {
            EditorApplication.update -= OnSceneUpdate;
        }

        public void OnSceneGUI()
        {
            Handles.BeginGUI();
            DisplayWindow();
            Handles.EndGUI();
        }

        private void OnSceneUpdate()
        {
            if (!CanPlayAnimation()) return;

            var frameTime = EditorApplication.timeSinceStartup - lastRecordedTime;
            var canUpdateAnimation = frameTime > FRAME_RATE_TIME;

            if (!canUpdateAnimation) return;

            lastRecordedTime = EditorApplication.timeSinceStartup;
            animation.PlayNextFrame();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private void DisplayWindow()
        {
            var sceneViewSize = SceneView.currentDrawingSceneView.position.size;
            var position = sceneViewSize - size - padding;
            var windowRect = new Rect(position, size);

            GUILayout.Window(id: 0, windowRect, DisplayWindowContent, title);
        }

        private void DisplayWindowContent(int windowID)
        {
            var disableToolbar = !CanPlayAnimation();

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(disableToolbar);
            DisplayAnimationControlsToolbar();
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5F);

            DisplayFields();

            GUILayout.Space(5F);

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(disableToolbar);
            DisplaySaveAsButton();
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        private void DisplayFields()
        {
            EditorGUILayout.FloatField("Current Sprite", animation.CurrentSpriteFrameIndex);
            EditorGUILayout.FloatField("Current Frame", animation.CurrentFrameIndex + 1);
        }

        private void DisplayAnimationControlsToolbar()
        {
            if (GUILayout.Button(previous, EditorStyles.toolbarButton))
            {
                animation.RenderPreviousFrame();
                EditorApplication.QueuePlayerLoopUpdate();
            }

            bool shouldPlay = GUILayout.Toggle(animation.IsPlaying, play, EditorStyles.toolbarButton);

            if (!animation.IsPlaying && shouldPlay) animation.Play();
            else if (animation.IsPlaying && !shouldPlay) animation.Pause();

            if (GUILayout.Button(next, EditorStyles.toolbarButton))
            {
                animation.RenderNextFrame();
                EditorApplication.QueuePlayerLoopUpdate();
            }

            GUILayout.Space(36F);

            animation.loop = GUILayout.Toggle(animation.loop, loop, EditorStyles.toolbarButton);

            if (GUILayout.Button(restart, EditorStyles.toolbarButton)) animation.Restart();
            if (GUILayout.Button(stop, EditorStyles.toolbarButton)) animation.Stop();
        }

        private bool CanPlayAnimation() =>
            !Application.isPlaying &&
            animation &&
            animation.HasFrames() &&
            animation.enabled &&
            animation.gameObject.activeInHierarchy;

        private void DisplaySaveAsButton()
        {
            if (GUILayout.Button(saveAnimation))
                SaveAnimation.SaveAsMecanim(animation);
        }
    }
}