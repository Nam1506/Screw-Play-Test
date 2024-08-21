using UnityEngine;

namespace ActionCode.SpriteAnimation
{
    /// <summary>
    /// Sprite Animation component. 
    /// Animates the sprite property inside a <see cref="SpriteRenderer"/> component.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteAnimation : AbstractAnimation
    {
        [SerializeField, Tooltip("Sprite Renderer used to animate Sprite frames.")]
        private SpriteRenderer spriteRenderer;

        public override Sprite Sprite
        {
            get => spriteRenderer.sprite;
            protected set => spriteRenderer.sprite = value;
        }

        private void Reset() => spriteRenderer = GetComponent<SpriteRenderer>();
    }
}