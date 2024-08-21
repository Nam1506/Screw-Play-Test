using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HomePanel : MonoBehaviour
{
    public EPanelHome ePanelHome;

    [SerializeField] private SkeletonGraphic iconAnim;

    [SerializeField] private ToggleScriptSetImage backgroundToggle;

    [SerializeField] private GameObject panelDisplay;

    [SerializeField] private Button button;

    public void OnSelect(bool state)
    {
        if (panelDisplay.activeSelf != state)
        {
            iconAnim.Initialize(true);

            if (state)
            {
                iconAnim.AnimationState.SetAnimation(0, "Appear", false);
            }
            else
            {
                iconAnim.AnimationState.SetAnimation(0, "Disable", false);
            }
        }

        backgroundToggle.OnChanged(state);

        panelDisplay.SetActive(state);
    }

    public void SetAction(Action action)
    {
        button.onClick.AddListener(() => { action?.Invoke(); });
    }
}
