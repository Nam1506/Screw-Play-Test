using Spine.Unity;
using System.Collections;
using UnityEngine;

public class PopupRaceLoading : PopupForce
{
    [SerializeField] private SkeletonGraphic canoAnim;

    private void OnEnable()
    {
        //canoAnim.AnimationState.SetAnimation(0, "Appear", false);
        //canoAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

        StartCoroutine(IeAutoHide());
    }

    private IEnumerator IeAutoHide()
    {
        PopupManager.Instance.ShowAllRewardRace();

        UIManager.Instance.Race.ResetProgress(false, RaceEvent.RACE_RANGE);

        yield return new WaitForSeconds(1.5f);

        if (PopupManager.Instance.HasAnyForceAction())
        {
            PopupManager.Instance.ForceShowPopupActions.Insert(1, () =>
            {
                PopupManager.Instance.ShowRace(true);
            });
        }
        else
        {
            UIManager.Instance.SendCurrencyToBack();
            PopupManager.Instance.ShowRace(true);
        }

        Hide(false, false, true);
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        UIManager.Instance.SendCurrencyToBack();
    }
}
