using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggleBarCollection : MonoBehaviour
{
    public UICollection.ETab eTab;

    [SerializeField] private List<ToggleScriptSetImage> toggles;

    public void OnChange(bool state)
    {
        toggles.OnChanged(state);
    }
}
