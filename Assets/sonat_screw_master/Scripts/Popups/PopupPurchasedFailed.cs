using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPurchasedFailed : PopupBase
{
    [SerializeField] private Button closeBtn;

    [SerializeField] private SkeletonGraphic emojiAnim;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            bool isSetPlayingWhenHide = GameManager.Instance.GameState == GameState.Ingame && !PopupManager.Instance.IsShowingMask();

            Hide(isSetPlayingWhenHide, true, true);
        });
    }

    private void OnEnable()
    {
        emojiAnim.Initialize(true);

        emojiAnim.AnimationState.SetAnimation(0, "Appear", false);

        emojiAnim.AnimationState.AddAnimation(0, "Idle", true, 0);
    }
}
