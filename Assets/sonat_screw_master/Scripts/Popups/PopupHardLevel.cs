using Spine.Unity;
using System.Collections;
using UnityEngine;

public class PopupHardLevel : PopupForce
{
    [SerializeField] private SkeletonGraphic anim;

    public void Show(EDifficulty difficulty)
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 1;

        anim.gameObject.SetActive(false);

        switch (difficulty)
        {
            case EDifficulty.Hard:
                anim.initialSkinName = "Hard";
                break;
            case EDifficulty.Super_Hard:
                anim.initialSkinName = "Super_hard";
                break;
            default:
                return;
        }

        GameplayManager.Instance.GameplayState = GameplayState.Pausing;

        PopupManager.OnSetInteractableMask?.Invoke(true);

        PopupManager.OnFadeOutBlackMask?.Invoke();

        anim.gameObject.SetActive(true);

        StartCoroutine(nameof(IePlayAnim));
    }

    private IEnumerator IePlayAnim()
    {
        anim.Initialize(true);

        anim.AnimationState.SetAnimation(0, "In", false);

        yield return new WaitForSeconds(anim.Skeleton.Data.FindAnimation("In").Duration);

        anim.AnimationState.SetAnimation(0, "Out", false);

        yield return new WaitForSeconds(anim.Skeleton.Data.FindAnimation("Out").Duration);

        Hide(PopupManager.Instance.ForceShowPopupActions.Count <= 1, false, true);

        //PopupManager.OnSetInteractableMask?.Invoke(false);

        //GameplayManager.Instance.GameplayState = GameplayState.Playing;

        //gameObject.SetActive(false);
    }

}
