using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupConfig
{
    public static readonly float TIME_FADE_MASK = 0.4f;

    public static readonly float FADE_MASK_VALUE = 0.8f;

    public static int MAX_POPUP_OPEN;
}

public class PopupRemoteConfig
{
    public int levelStart;
    public bool appOpen;
    public int order;
    public int interval;
    public int daily;
}