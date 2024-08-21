using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupReward : PopupBase
{
    public static event EventHandler OnClaimRewardAction;

    [SerializeField] private Button tapBtn;
    [SerializeField] private SkeletonGraphic titleAnim;

    [SerializeField] private Transform container;
    [SerializeField] private Image black_mask;

    [SerializeField] private GameObject tapText;
    [SerializeField] private GameObject currencyGroup;
    [SerializeField] private GameObject boosterGroup;

    [SerializeField] private UIReward coin;
    [SerializeField] private UIReward liveInfi;
    [SerializeField] private UIReward live;
    [SerializeField] private UIReward addHole;
    [SerializeField] private UIReward addBox;
    [SerializeField] private UIReward hammer;

    [SerializeField] private Transform end;

    private List<Reward> rewards = new();

    private bool isSetPlayingWhenHide;

    [HideInInspector] public float delay;

    private void Awake()
    {
        tapBtn.onClick.AddListener(() =>
        {
            Hidden();
        });
    }

    private void OnEnable()
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

                    UIManager.Instance.UICoin.SetKeepCoin();
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

    private void OnDisable()
    {
        UIManager.Instance.SendCurrencyToFront();

        delay = 0f;

        foreach (Reward reward in rewards)
        {
            switch (reward.id)
            {
                case RewardID.coin:
                    DOVirtual.DelayedCall(delay, () =>
                    {
                        UIManager.Instance.CollectCoins(10, () =>
                        {
                            if (isSetPlayingWhenHide)
                            {
                                GameplayManager.Instance.GameplayState = GameplayState.Playing;
                            }

                        }, coin.transform.position, isSetPlayingWhenHide);
                    });

                    UIManager.Instance.UICoin.SetKeepCoin();

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

        if (delay > 0)
        {
            OnClaimRewardAction?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SetRewards(List<Reward> rewards)
    {
        this.rewards = rewards;
    }

    private bool CheckNoAds()
    {
        bool hasNoAds = false;
        bool hasOther = false;

        foreach (Reward reward in rewards)
        {
            if (reward.type == RewardType.noAds)
            {
                hasNoAds = true;
            }
            else
            {
                hasOther = true;
            }
        }

        return hasNoAds && !hasOther;
    }

    public void Show(bool isSetPlayingWhenHide)
    {
        if (CheckNoAds()) return;

        UIManager.Instance.SendCurrencyToBack(false);

        this.isSetPlayingWhenHide = isSetPlayingWhenHide;

        SoundManager.Instance.PlaySound(KeySound.BtnOpen);

        GameplayManager.Instance.GameplayState = GameplayState.Pausing;

        PopupManager.OnSetInteractableMask?.Invoke(true);

        FadeOutBlackMask();

        gameObject.SetActive(true);
        container.gameObject.SetActive(true);
        container.GetComponent<CanvasGroup>().alpha = 1.0f;

        canvasGroup.alpha = 1f;

        container.DOKill();
        container.localScale = Vector3.one * 0.7f;
        container.DOScale(Vector3.one, PopupConfig.TIME_FADE_MASK).SetEase(Ease.OutBack).OnComplete(() =>
        {
            PopupManager.OnSetInteractableMask?.Invoke(false);
        });

        StartCoroutine(IePlayTitleAnim());
    }

    public void Hidden()
    {
        SoundManager.Instance.PlaySound(KeySound.BtnClose);

        titleAnim.gameObject.SetActive(false);

        //container.gameObject.SetActive(false);
        container.GetComponent<CanvasGroup>().DOFade(0, PopupConfig.TIME_FADE_MASK);

        FadeInBlackMask();
    }

    private IEnumerator IePlayTitleAnim()
    {
        titleAnim.gameObject.SetActive(false);

        yield return new WaitForSeconds(PopupConfig.TIME_FADE_MASK);

        titleAnim.gameObject.SetActive(true);

        titleAnim.Initialize(true);

        titleAnim.AnimationState.SetAnimation(0, "animation", false);
    }

    private void SetBlackMask(bool state)
    {
        black_mask.gameObject.SetActive(state);
    }

    private void FadeInBlackMask()
    {
        black_mask.DOFade(0f, PopupConfig.TIME_FADE_MASK)
            .OnComplete(() =>
            {
                SetBlackMask(false);

                gameObject.SetActive(false);
            });
    }

    private void FadeOutBlackMask()
    {
        SetBlackMask(true);

        black_mask.DOFade(PopupConfig.FADE_MASK_VALUE, PopupConfig.TIME_FADE_MASK);
    }
}
