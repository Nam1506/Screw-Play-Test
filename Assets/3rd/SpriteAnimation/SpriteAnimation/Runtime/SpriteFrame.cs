using UnityEngine;

namespace ActionCode.SpriteAnimation
{
    /// <summary>
    /// Sprite Frame information used by <see cref="SpriteAnimation"/>. 
    /// </summary>
    [System.Serializable]
    public struct SpriteFrame
    {
        [Tooltip("The sprite asset.")]
        public Sprite sprite;
        [Tooltip("Numbers of frames for this sprite."), Min(1)]
        public int frames;

        /// <summary>
        /// Initialize the struct.
        /// </summary>
        /// <param name="sprite">The sprite reference.</param>
        /// <param name="frames">The number of frames. Default is 1.</param>
        public SpriteFrame(Sprite sprite, int frames = 1)
        {
            this.sprite = sprite;
            this.frames = frames;
        }
    }
}