using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRace : PopupForce
{
    [SerializeField] private SkeletonGraphic ribbonAnim;
    [SerializeField] private Button closeBtn;
    [SerializeField] private UIMainRace mainRace;
    [SerializeField] private UIRaceResult result;

    [SerializeField] private List<ParticleSystem> confetti;

    [SerializeField] private SkeletonUtilityBone ribbonTxt;

    private float time;

    [SerializeField] float timeDelayConfetti;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Hide(false, false, true);
        });
    }

    private void OnEnable()
    {
        Debug.Log("Play");

        ribbonAnim.Initialize(true);

        ribbonTxt.Reset();

        ribbonAnim.AnimationState.SetAnimation(0, "Appear", false);
        ribbonAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

        mainRace.SetData();

        if (RaceEvent.CheckRaceFinish())
        {
            PopupManager.OnSetInteractableMask?.Invoke(true);
            PopupManager.Instance.canSetInteractable = false;

            DOVirtual.DelayedCall(0.667f * 2f, () =>
            {
                PopupManager.Instance.canSetInteractable = true;
                PopupManager.OnSetInteractableMask?.Invoke(false);
                result.Show();

                int rank = RaceEvent.GetMyRank();

                if (rank <= 5)
                {
                    DOVirtual.DelayedCall(timeDelayConfetti, () =>
                    {
                        foreach (var eff in confetti)
                        {
                            eff.Play();
                        }
                    });
                }
            });

        }
        else
        {
            result.Hide(false);
        }

        time = 0f;
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    private void OnDisable()
    {
        SonatTracking.LogSpentTime("feature", "race", "Home", time);

    }

    public void CheckCloseReward()
    {
        StartCoroutine(IeCheckCloseReward());
    }


    public void EndRace()
    {
        Hide(false, false, false);

        mainRace.ResetData();
    }

    private IEnumerator IeCheckCloseReward()
    {
        yield return new WaitForSeconds(0.4f);

        while (UIManager.Instance.IsReceivingRewards())
        {
            yield return null;
        }

        EndRace();
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        UIManager.Instance.SendCurrencyToBack();
    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        mainRace.DestroyLoopSound();

        if (RaceEvent.raceData.isForceShowStart)
        {
            PopupManager.Instance.ForceShowPopupActions.Add(() =>
            {
                PopupManager.Instance.ShowRaceStart(true);
            });

            PopupManager.Instance.StartForceAction();

            base.Hide(isSetPlaying, true, isFade);
        }
        else
        {
            base.Hide(isSetPlaying, isKeepingMask, isFade);
        }
    }

    public void ShowAllReward()
    {
        mainRace.SetFirstJoin(true);
    }
}
