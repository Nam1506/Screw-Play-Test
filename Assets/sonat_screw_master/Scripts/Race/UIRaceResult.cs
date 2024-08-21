using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRaceResult : MonoBehaviour
{
    [SerializeField] private Image black_mask;
    [SerializeField] private SkeletonGraphic ribbonAnim;
    [SerializeField] private SkeletonGraphic giftAnim;
    [SerializeField] private SkeletonGraphic finishAnim;
    [SerializeField] private GameObject rewardObj;
    [SerializeField] private GameObject noRewardObj;
    [SerializeField] private GameObject tapObj;
    [SerializeField] private Button tapBtn;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private PopupRace popupRace;

    [Space]

    [SerializeField] private GameObject currencyGroup;
    [SerializeField] private GameObject boosterGroup;
    [SerializeField] private UIReward coin;
    [SerializeField] private UIReward liveInfi;
    [SerializeField] private UIReward live;
    [SerializeField] private UIReward addHole;
    [SerializeField] private UIReward addBox;
    [SerializeField] private UIReward hammer;
    [SerializeField] private Transform end;

    private List<Reward> rewards;

    private void Init()
    {
        ribbonAnim.gameObject.SetActive(false);
        giftAnim.gameObject.SetActive(false);
        finishAnim.gameObject.SetActive(false);
        tapObj.SetActive(false);
        rewardObj.SetActive(false);
        noRewardObj.SetActive(false);

        rewards = RaceEvent.GetRewards();

        if (rewards.Count > 0)
        {
            InitRewards();
        }

        int rank = RaceEvent.GetMyRank();

        if (rank <= 5)
            rankText.text = $"#{rank}";
        else
            rankText.text = "Unfinished";
    }

    private void StartAction()
    {
        finishAnim.gameObject.SetActive(true);
        finishAnim.Initialize(true);

        SoundManager.Instance.PlaySound(KeySound.Race_Banner_ComeIn);

        DOVirtual.DelayedCall(1f, () =>
        {
            SoundManager.Instance.PlaySound(KeySound.Race_Clock_Disappear);
            DOVirtual.DelayedCall(0.35f, () =>
            {
                SoundManager.Instance.PlaySound(KeySound.Race_Banner_ComeOut);
            });
        });

        finishAnim.AnimationState.SetAnimation(0, "In", false);
        finishAnim.AnimationState.AddAnimation(0, "Out", false, 0).Complete += (track) =>
        {
            ribbonAnim.gameObject.SetActive(true);
            ribbonAnim.Initialize(true);
            ribbonAnim.AnimationState.SetAnimation(0, "Appear", false);
            ribbonAnim.AnimationState.AddAnimation(0, "Idle", true, 0);

            tapBtn.onClick.RemoveAllListeners();

            if (rewards.Count > 0)
            {
                tapBtn.onClick.AddListener(OnClaimReward);

                giftAnim.gameObject.SetActive(true);
                giftAnim.Initialize(true);

                SoundManager.Instance.PlaySound(KeySound.Race_Gift_Appear);

                giftAnim.AnimationState.SetAnimation(0, "Appear", false).Complete += (track) =>
                {
                    rewardObj.SetActive(true);

                    rewardObj.transform.localScale = Vector3.zero;
                    rewardObj.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.3f).SetEase(Ease.OutQuad);

                    RectTransform rect = rewardObj.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(0, 64);
                    rect.DOAnchorPos(new Vector2(0, 153f), 0.5f).SetDelay(0.3f).SetEase(Ease.OutQuad);

                    SoundManager.Instance.PlaySound(KeySound.Race_Gift_Open);
                };
                giftAnim.AnimationState.AddAnimation(0, "Open", false, 0).Complete += (track) =>
                {
                    tapObj.SetActive(true);
                };
                giftAnim.AnimationState.AddAnimation(0, "Idle", true, 0);
            }
            else
            {
                tapBtn.onClick.AddListener(OnContinue);

                noRewardObj.SetActive(true);
                tapObj.SetActive(true);
            }
        };
    }

    private void InitRewards()
    {
        currencyGroup.SetActive(false);
        boosterGroup.SetActive(false);

        coin.gameObject.SetActive(false);
        liveInfi.gameObject.SetActive(false);
        live.gameObject.SetActive(false);
        addHole.gameObject.SetActive(false);
        addBox.gameObject.SetActive(false);
        hammer.gameObject.SetActive(false);

        foreach (Reward reward in rewards)
        {
            switch (reward.id)
            {
                case RewardID.coin:
                    currencyGroup.SetActive(true);
                    coin.gameObject.SetActive(true);

                    coin.Init(null, reward.amount);
                    break;
                case RewardID.unlimitedLives:
                    currencyGroup.SetActive(true);
                    liveInfi.gameObject.SetActive(true);

                    liveInfi.Init(null, reward.time);
                    break;
                case RewardID.live:
                    currencyGroup.SetActive(true);
                    live.gameObject.SetActive(true);

                    live.Init(null, reward.amount, true, false);
                    break;
                case RewardID.addHole:
                    boosterGroup.SetActive(true);
                    addHole.gameObject.SetActive(true);

                    addHole.Init(null, reward.amount);
                    break;
                case RewardID.addBox:
                    boosterGroup.SetActive(true);
                    addBox.gameObject.SetActive(true);

                    addBox.Init(null, reward.amount);
                    break;
                case RewardID.hammer:
                    boosterGroup.SetActive(true);
                    hammer.gameObject.SetActive(true);

                    hammer.Init(null, reward.amount);
                    break;
            }
        }
    }

    private void OnClaimReward()
    {
        Hide(false);

        RaceEvent.OnClaimReward();

        float delay = 0f;

        foreach (Reward reward in rewards)
        {
            switch (reward.id)
            {
                case RewardID.coin:
                    DOVirtual.DelayedCall(delay, () =>
                    {
                        UIManager.Instance.CollectCoins(10, null, coin.transform.position);
                    });
                    delay += 0.2f;
                    break;
                case RewardID.unlimitedLives:
                    DOVirtual.DelayedCall(delay, () =>
                    {
                        UIManager.Instance.ClaimReward(reward, liveInfi.transform, UIManager.Instance.UILive.transform, 0.2f);
                    });
                    delay += 0.2f;
                    break;
                case RewardID.live:
                    DOVirtual.DelayedCall(delay, () =>
                    {
                        UIManager.Instance.ClaimReward(reward, live.transform, UIManager.Instance.UILive.transform, 0.2f);
                    });
                    delay += 0.2f;
                    break;
                case RewardID.addHole:
                    DOVirtual.DelayedCall(delay, () =>
                    {
                        UIManager.Instance.ClaimReward(reward, addHole.transform, end, 0.2f);
                    });
                    delay += 0.2f;
                    break;
                case RewardID.addBox:
                    DOVirtual.DelayedCall(delay, () =>
                    {
                        UIManager.Instance.ClaimReward(reward, addBox.transform, end, 0.2f);
                    });
                    delay += 0.2f;
                    break;
                case RewardID.hammer:
                    DOVirtual.DelayedCall(delay, () =>
                    {
                        UIManager.Instance.ClaimReward(reward, hammer.transform, end, 0.2f);
                    });
                    delay += 0.2f;
                    break;
            }
        }

        IAPManager.Claim(rewards, "race_event_rank_" + RaceEvent.GetMyRank());

        popupRace.CheckCloseReward();
    }

    private void OnContinue()
    {
        Hide(false);

        RaceEvent.OnContinue();

        popupRace.EndRace();
    }

    private void SetBlackMask(bool state)
    {
        black_mask.gameObject.SetActive(state);
    }

    public void Hide(bool isFade)
    {
        if (isFade)
        {
            black_mask.DOFade(0f, PopupConfig.TIME_FADE_MASK).OnComplete(() =>
            {
                SetBlackMask(false);
                gameObject.SetActive(false);
            });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        Init();

        gameObject.SetActive(true);

        SetBlackMask(true);

        black_mask.DOFade(PopupConfig.FADE_MASK_VALUE, PopupConfig.TIME_FADE_MASK).onComplete += StartAction;
    }
}
