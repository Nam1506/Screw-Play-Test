using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptActive : ToggleScript
{
    public override void OnChanged(bool val)
    {
        gameObject.SetActive(val);
    }
}
