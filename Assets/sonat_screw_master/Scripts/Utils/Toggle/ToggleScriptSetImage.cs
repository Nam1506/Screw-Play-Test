using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptSetImage : ToggleScript
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite imageOn;
    [SerializeField] private Sprite imageOff;

    public override void OnChanged(bool val)
    {
        image.sprite = val ? imageOn : imageOff;
    }
}
