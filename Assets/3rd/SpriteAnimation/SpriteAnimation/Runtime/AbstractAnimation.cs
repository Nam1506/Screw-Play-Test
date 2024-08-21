using UnityEngine;

namespace ActionCode.SpriteAnimation
{
    /// <summary>
    /// Abstract class to provide animation behaviour.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AbstractAnimation : MonoBehaviour
    {
        [Tooltip("Should start playing the animation when this Component is enabled?")]
        public bool playOnEnabled = true;
        [Tooltip("Should play animation again after it ends?")]
        public bool loop = true;
        [SerializeField, Tooltip("Always start the animation at this frame.")]
        private int startFrame = 0;
        [Tooltip("The animation playback type.")]
        public PlaybackType playback = PlaybackType.Forward;
        [Tooltip("Sprite Frames list. Each one contains the sprite and the frame count.")]
        [ContextMenuItem("Remove All", "RemoveAllFrames")]
        public SpriteFrame[] frames = new SpriteFrame[0];

        /// <summary>
        /// Is the animation playing?
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Is the animation paused?
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Is the animation stopped?
        /// </summary>
        public bool IsStoped { get; private set; }

        /// <summary>
        /// The current frame index.
        /// </summary>
        public int CurrentFrameIndex { get; private set; }

        /// <summary>
        /// The current sprite frame index.
        /// </summary>
        public int CurrentSpriteFrameIndex { get; private set; }

        /// <summary>
        /// The current Sprite Frame.
        /// </summary>
        public SpriteFrame CurrentSpriteFrame => frames[CurrentSpriteFrameIndex];

        /// <summary>
        /// The Sprite been rendered.
        /// </summary>
        public abstract Sprite Sprite { get; protected set; }

        /// <summary>
        /// Always start the animation at this frame.
        /// </summary>
        public int StartFrame
        {
            get => startFrame;
            set => startFrame = HasFrames() ? Mathf.Clamp(value, 0, frames.Length - 1) : 0;
        }

        private void OnEnable()
        {
            Stop();
            if (playOnEnabled) Play();
        }

        private void FixedUpdate() => PlayNextFrame();

        private void OnValidate()
        {
            // Validates startFrame between 0 and frames.Length - 1
            StartFrame = startFrame;
        }

        /// <summary>
        /// Starts to play the animation.
        /// </summary>
        public void Play()
        {
            IsPlaying = true;
            IsPaused = false;
            IsStoped = false;
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            IsPlaying = false;
            IsStoped = false;
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
            IsStoped = true;

            switch (playback)
            {
                case PlaybackType.Forward:
                    CurrentFrameIndex = -1;
                    CurrentSpriteFrameIndex = 0;
                    Sprite = HasFrames() ? frames[0].sprite : null;
                    break;

                case PlaybackType.Backward:
                    ResetIndexesBackwards();
                    var lastFrameIndex = frames.Length - 1;
                    Sprite = HasFrames() ? frames[lastFrameIndex].sprite : null;
                    break;
            }
        }

        /// <summary>
        /// Restarts the animation by stopping and playing it.
        /// </summary>
        public void Restart()
        {
            Stop();
            Play();
        }

        /// <summary>
        /// Checks if can play animation.
        /// </summary>
        /// <returns>True if can play. False otherwise.</returns>
        public bool CanPlay() => !IsPlaying && HasFrames();

        /// <summary>
        /// Checks if there are frames.
        /// </summary>
        /// <returns>True if frame count is bigger than 0. False otherwise.</returns>
        public bool HasFrames() => frames.Length > 0;

        /// <summary>
        /// Checks if SpriteRenderer component is rendering any sprite.
        /// </summary>
        /// <returns>True if a sprite has been rendering. False otherwise.</returns>
        public bool IsRenderingSprite() => Sprite != null;

        /// <summary>
        /// Plays the animation next frame.
        /// </summary>
        public void PlayNextFrame()
        {
            if (!IsPlaying) return;

            if (playback == PlaybackType.Forward) RenderNextFrame();
            else if (playback == PlaybackType.Backward) RenderPreviousFrame();
        }

        /// <summary>
        /// Renders the previous frame.
        /// </summary>
        public void RenderPreviousFrame()
        {
            if (!HasFrames()) return;

            IsStoped = false;
            var hasPreviousFrame = --CurrentFrameIndex < 0;

            if (hasPreviousFrame)
            {
                CurrentFrameIndex = CurrentSpriteFrame.frames - 1;

                if (CurrentSpriteFrameIndex > 0) CurrentSpriteFrameIndex--;
                else
                {
                    if (loop) ResetIndexesBackwards();
                    else Pause();
                }

                Sprite = CurrentSpriteFrame.sprite;
            }
        }

        /// <summary>
        /// Renders the next frame.
        /// </summary>
        public void RenderNextFrame()
        {
            if (!HasFrames()) return;

            IsStoped = false;
            var hasNextFrame = ++CurrentFrameIndex >= CurrentSpriteFrame.frames;

            if (hasNextFrame)
            {
                CurrentFrameIndex = 0;
                var hasNextSprite = CurrentSpriteFrameIndex < frames.Length - 1;

                if (hasNextSprite) CurrentSpriteFrameIndex++;
                else
                {
                    if (loop) CurrentSpriteFrameIndex = StartFrame;
                    else Pause();
                }

                Sprite = CurrentSpriteFrame.sprite;
            }
        }

        private void ResetIndexesBackwards()
        {
            var hasStartFrame = StartFrame != 0;
            CurrentSpriteFrameIndex = hasStartFrame ? StartFrame : frames.Length - 1;
            SpriteFrame lastFrame = CurrentSpriteFrame;
            CurrentFrameIndex = lastFrame.frames - 1;
        }

        protected void RemoveAllFrames() => frames = new SpriteFrame[0];
    }
}