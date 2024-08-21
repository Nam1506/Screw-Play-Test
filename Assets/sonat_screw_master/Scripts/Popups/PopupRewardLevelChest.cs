using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupRewardLevelChest : PopupBase
{
    public static event EventHandler OnClaimRewardAction;

    [SerializeField] private Button tapBtn;
    [SerializeField] private SkeletonGraphic chestAnim;

    [SerializeField] private TMP_Text tapText;

    [SerializeField] private UIReward coin;
    [SerializeField] private UIReward liveInfi;
    [SerializeField] private UIReward live;
    [SerializeField] private UIReward addHole;
    [SerializeField] private UIReward addBox;
    [SerializeField] private UIReward hammer;

    [SerializeField] private List<UIReward> uiRewards;
    [SerializeField] private List<Transform> listPos;

    [SerializeField] private Transform end;

    private List<Reward> rewards = new();

    private bool isSetPlayingWhenHide;

    [HideInInspector] public float delay;

    [SerializeField] private ParticleSystem shineBox;

    private void Awake()
    {
        tapBtn.onClick.AddListener(() =>
        {
            Hidden();
        });
    }

    private void OnEnable()
    {
        foreach (var reward in uiRewards)
        {
            reward.transform.localScale = Vector3.zero;
            reward.transform.localPosition = new Vector3(0, 60f, 0f);
        }

        tapBtn.interactable = false;
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

    public void Show()
    {
        if (CheckNoAds()) return;

        UIManager.Instance.SendCurrencyToBack(false);

        SoundManager.Instance.PlaySound(KeySound.BtnOpen);

        PopupManager.OnSetInteractableMask?.Invoke(true);

        tapText.alpha = 0f;
        canvasGroup.alpha = 0f;

        gameObject.SetActive(true);
        canvasGroup.DOFade(1f, PopupConfig.TIME_FADE_MASK);

        shineBox.Stop();

        StartCoroutine(IEPlayChestAnim());
    }

    public void Hidden()
    {
        SoundManager.Instance.PlaySound(KeySound.BtnClose);

        chestAnim.gameObject.SetActive(false);
        shineBox.gameObject.SetActive(false);

        canvasGroup.DOFade(0f, PopupConfig.TIME_FADE_MASK)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    public float gapTime;

    private IEnumerator IEPlayChestAnim()
    {
        chestAnim.gameObject.SetActive(false);
        shineBox.gameObject.SetActive(false);

        yield return new WaitForSeconds(PopupConfig.TIME_FADE_MASK);

        chestAnim.Initialize(true);

        chestAnim.gameObject.SetActive(true);
        shineBox.gameObject.SetActive(true);

        SoundManager.Instance.PlaySound(KeySound.Chest_Level_Appear);

        chestAnim.AnimationState.SetAnimation(0, "Appear", false);

        yield return new WaitForSeconds(Helper.GetTimeAnim(chestAnim, "Appear"));

        SoundManager.Instance.PlaySound(KeySound.Chest_Level_Open);

        chestAnim.AnimationState.SetAnimation(0, "Open", false);

        yield return new WaitForSeconds(Helper.GetTimeAnim(chestAnim, "Open") - 0.5f);

        var items = GetListRewardItem();

        Repos(items.Count == 2 ? false : true);

        var firstItem = items[0];

        int posIndex = 0;

        if (items.Count == 2) posIndex++;

        firstItem.transform.DOJump(listPos[posIndex++].transform.position, jumpPower, numJump, duration);
        firstItem.transform.DOScale(1f, duration);

        if (items.Count > 1)
        {
            yield return new WaitForSeconds(0.4f);
        }

        for (int i = 1; i < items.Count; i++)
        {
            SoundManager.Instance.PlayComboSound(KeyCombo.Chest_Level_Open, i - 1);

            var item = items[i];

            item.transform.DOJump(listPos[posIndex++].transform.position, jumpPower, numJump, duration);
            item.transform.DOScale(1f, duration);

            chestAnim.Initialize(true);

            chestAnim.AnimationState.SetAnimation(0, "Open2", false);

            yield return new WaitForSeconds(Helper.GetTimeAnim(chestAnim, "Open2") + gapTime);
        }

        chestAnim.AnimationState.AddAnimation(0, "Idle_after", true, 0);

        shineBox.Play();

        tapText.DOFade(1f, 0.5f);

        PopupManager.OnSetInteractableMask?.Invoke(false);

        tapBtn.interactable = true;
    }

    private List<UIReward> GetListRewardItem()
    {
        List<UIReward> items = new();

        foreach (Reward reward in rewards)
        {
            switch (reward.id)
            {
                case RewardID.coin:
                    coin.Init(null, reward.amount);

                    items.Add(coin);
                    break;
                case RewardID.unlimitedLives:
                    liveInfi.Init(null, reward.time);

                    items.Add(liveInfi);
                    break;
                case RewardID.live:
                    live.Init(null, reward.amount, true, false);

                    items.Add(live);
                    break;
                case RewardID.addHole:
                    addHole.Init(null, reward.amount);

                    items.Add(addHole);
                    break;
                case RewardID.addBox:
                    addBox.Init(null, reward.amount);

                    items.Add(addBox);
                    break;
                case RewardID.hammer:
                    hammer.Init(null, reward.amount);

                    items.Add(hammer);
                    break;
            }
        }

        return items;
    }

    public float jumpPower;
    public int numJump;
    public float duration;

    private void Repos(bool isDefault = true)
    {
        if (isDefault)
        {
            listPos[1].SetLocalPositionX(-300f);
            listPos[2].SetLocalPositionX(300f);
        }
        else
        {
            listPos[1].SetLocalPositionX(-200f);
            listPos[2].SetLocalPositionX(200f);
        }
    }
}
