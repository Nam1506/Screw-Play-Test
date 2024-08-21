#if UI_MODULE
using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.SpriteAnimation
{
    /// <summary>
    /// UI Image Animation component. 
    /// Animates the sprite property inside an <see cref="Image"/> component.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public sealed class ImageAnimation : AbstractAnimation
    {
        [SerializeField, Tooltip("Image Renderer component used to animate Sprite frames.")]
        private Image imageRenderer;

        public override Sprite Sprite
        {
            get => imageRenderer.sprite;
            protected set => imageRenderer.sprite = value;
        }

        private void Reset() => imageRenderer = GetComponent<Image>();
    }
}
#endif