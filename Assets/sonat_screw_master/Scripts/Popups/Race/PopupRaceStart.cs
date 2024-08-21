using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PopupRaceStart : PopupForce
{
    [SerializeField] private SkeletonGraphic ribbonAnim;
    [SerializeField] private SkeletonGraphic minerAnim;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button joinBtn;

    [SerializeField] private ParticleSystem effect;

    private float time;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Hide(false, false, true);

            RaceEvent.OnNoJoin();
        });
        joinBtn.onClick.AddListener(() =>
        {
            if (PopupManager.Instance.HasAnyForceAction())
            {
                PopupManager.Instance.ForceShowPopupActions.Insert(1, () =>
                {
                    PopupManager.Instance.ShowRaceLoading();
                });

                Hide(false, true, true);
            }
            else
            {
                Hide(false, true, true);

                PopupManager.Instance.ForceShowPopupActions.Add(() =>
                {
                    PopupManager.Instance.ShowRaceLoading();
                });

                PopupManager.Instance.StartForceAction();
            }

            RaceEvent.RelaunchEvent();

        });
    }

    private void OnEnable()
    {
        SoundManager.Instance.PlaySound(KeySound.Race_Board, true);

        effect.Stop();

        ribbonAnim.Initialize(true);
        ribbonAnim.AnimationState.SetAnimation(0, "Appear", false);
        ribbonAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

        minerAnim.Initialize(true);
        minerAnim.AnimationState.SetAnimation(0, "Appear", false);
        minerAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

        DOVirtual.DelayedCall(Helper.GetTimeAnim(minerAnim, "Appear") - 0.8f, () =>
        {
            effect.Play();
        });

        time = 0f;
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    private void OnDisable()
    {
        SonatTracking.LogSpentTime("feature", "race_start", "Home", time);

        SoundManager.Instance.StopLoopSound();
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        //UIManager.Instance.SendCurrencyToBack();

        minerAnim.DOFade(1, PopupConfig.TIME_FADE_MASK);
    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        base.Hide(isSetPlaying, isKeepingMask, isFade);

        minerAnim.DOFade(0, PopupConfig.TIME_FADE_MASK);
    }
}
