using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToggleScriptFade : ToggleScript
{
    public float duration;

    public override void OnChanged(bool val)
    {
        Helper.FadeObject(val, GetComponent<CanvasGroup>(), duration);
    }
}
