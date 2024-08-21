using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionCode.SpriteAnimation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteAnimationCustom : AbstractAnimation
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
