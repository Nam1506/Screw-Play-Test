using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class NoAdsAnim : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic anim;
    [SerializeField] private Button button;
    [SerializeField] private bool openPopup;

    private void Awake()
    {
        if (openPopup)
        {
            button.onClick.AddListener(() =>
            {
                PopupManager.Instance.ShowNoAds();
            });
        }
    }

    private void OnEnable()
    {
        anim.AnimationState.SetAnimation(0, "animation", true);
    }
}
