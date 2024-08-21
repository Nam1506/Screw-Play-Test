using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptPos : ToggleScript
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 posOn;
    [SerializeField] private Vector2 posOff;

    public override void OnChanged(bool val)
    {
        rectTransform.anchoredPosition = val ? posOn : posOff;
    }
}
