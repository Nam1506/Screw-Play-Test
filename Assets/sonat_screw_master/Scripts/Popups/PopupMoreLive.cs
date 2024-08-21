using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMoreLive : PopupBase
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button watchAds;
    [SerializeField] private TextMeshProUGUI coinAmount;
    [SerializeField] private TextMeshProUGUI liveAmount;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private ToggleScript[] toggleLive;

    private int currentLives;
    private DateTime nextLiveTime;
    private DateTime lastAddedTime;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "more_live", GameManager.Instance.eScreen.ToString(), "close");

            if (GameManager.Instance.GameState == GameState.Home)
            {
                Hide(false, false, true);
                return;
            }

            if (DataManager.Instance.playerData.lives.amount > 0)
            {
                if (PopupManager.Instance.ShowPrePopup(() => { UIManager.Instance.HideCoin(false); }))
                {
                    Hide(false, true, false);
                }
                else
                {
                    Hide(true, false, true);

                    UIManager.Instance.HideCoin();
                }
            }
            else
            {
                GameplayManager.Instance.ResetData();

                Hide(false, false, true);

                GameManager.Instance.GameState = GameState.Home;
            }
        });

        buyBtn.onClick.AddListener(() =>
        {
            //int liveToMax = GameDefine.MAX_LIVES - currentLives;
            //int totalCost = GameDefine.LIVE_COST * liveToMax;
            int totalCost = GameDefine.LIVE_COST * GameDefine.MAX_LIVES;

            if (totalCost > DataManager.Instance.playerData.coins)
            {
                Hide(false, false, false);

                if (GameManager.Instance.GameState == GameState.Ingame)
                {
                    PopupManager.Instance.PrePopups.Add(this);

                    UIManager.Instance.ShowShop();

                    PopupManager.Instance.ShowNotiAlert("Not enough coins");
                }
                else
                {
                    HomeController.Instance.SelectPanel(EPanelHome.Shop);
                }
            }
            else
            {
                if (UIManager.Instance.IsShowingShop || GameManager.Instance.GameState == GameState.Home)
                {
                    Hide(false, false, true);
                    PopupManager.Instance.PrePopups.Clear();
                }
                else
                {
                    var prePopup = PopupManager.Instance.PrePopups[PopupManager.Instance.PrePopups.Count - 1];

                    if (prePopup is PopupLose)
                    {
                        if (PopupManager.Instance.ShowPrePopup(() => { UIManager.Instance.HideCoin(false); }))
                        {
                            Hide(false, true, false);
                        }
                    }
                    else if (prePopup is PopupRestart)
                    {
                        if (PopupManager.Instance.PrePopups.Count != 0)
                        {
                            PopupReward.OnClaimRewardAction += PopupReward_OnClaimRewardAction;

                            PopupManager.Instance.PrePopups.Clear();
                        }
                    }
                    else
                    {
                        if (GameplayManager.Instance.IsLose)
                        {
                            Hide(false, true, false);
                            UIManager.Instance.HideCoin(false);
                            PopupManager.Instance.ShowLose();
                        }
                        else
                        {
                            Hide(true, false, true);
                            UIManager.Instance.HideCoin();
                        }
                    }
                }

                DataManager.Instance.ChangeCoins(-totalCost);
                DataManager.Instance.AddLives(GameDefine.MAX_LIVES, false);

                List<Reward> rewards = new List<Reward>() { new Reward(RewardID.live, RewardType.currency, 5) };
                PopupManager.Instance.ShowReward(rewards);

                SonatTracking.LogBuyShopItem("other_source", "lives", GameDefine.MAX_LIVES, "coin", DataManager.Instance.IsFirstBuy());

                SonatTracking.LogSpendCurrency("coin", "currency", totalCost, GameManager.Instance.eScreen.ToString(), "other_source", "live");
                SonatTracking.LogEarnCurrency("live", "other_source", GameDefine.MAX_LIVES, GameManager.Instance.eScreen.ToString(), "non_iap",
                    DataManager.Instance.playerData.saveLevelData.currentLevel, "currency", "coin");
            }
        });
        watchAds.onClick.AddListener(() =>
        {
            SonatTracking.PlayVideoAds("live", () =>
            {
                if (UIManager.Instance.IsShowingShop || GameManager.Instance.GameState == GameState.Home)
                {
                    Hide(false, false, true);
                    PopupManager.Instance.PrePopups.Clear();
                }
                else
                {
                    //if (PopupManager.Instance.ShowPrePopup(() => { UIManager.Instance.HideCoin(false); }))
                    //{
                    //    Hide(false, true, false);
                    //}

                    if (PopupManager.Instance.PrePopups.Count != 0)
                    {
                        PopupReward.OnClaimRewardAction += PopupReward_OnClaimRewardAction;

                        PopupManager.Instance.PrePopups.Clear();
                    }
                    else
                    {
                        if (GameplayManager.Instance.IsLose)
                        {
                            Hide(false, true, false);
                            UIManager.Instance.HideCoin(false);
                            PopupManager.Instance.ShowLose();
                        }
                        else
                        {
                            Hide(true, false, true);
                            UIManager.Instance.HideCoin();
                        }
                    }
                }

                DOVirtual.DelayedCall(0.1f, () =>
                {
                    DataManager.Instance.AddLives(1, false);
                });

                List<Reward> rewards = new List<Reward>() { new Reward(RewardID.live, RewardType.currency, 1) };
                PopupManager.Instance.ShowReward(rewards);

            }, "other_source", GameManager.Instance.eScreen.ToString());
        });
    }

    private void OnDisable()
    {
        DataManager.Instance.OnUpdateLiveAction -= DataManager_OnUpdateLiveAction;
        PopupReward.OnClaimRewardAction -= PopupReward_OnClaimRewardAction;
    }

    private void OnEnable()
    {
        Load();

        UpdateLives();
        UpdateTimer();

        int liveToMax = GameDefine.MAX_LIVES - currentLives;
        //liveAmount.text = liveToMax.ToString();
        coinAmount.text = (GameDefine.LIVE_COST * GameDefine.MAX_LIVES).ToString();

        if (gameObject.activeSelf)
            StartCoroutine(nameof(RestoreLive));

        DataManager.Instance.OnUpdateLiveAction += DataManager_OnUpdateLiveAction;
    }

    private void PopupReward_OnClaimRewardAction(object sender, System.EventArgs e)
    {
        UIManager.Instance.ShowLive();

        DOVirtual.DelayedCall(PopupManager.Instance.TimeDelayReward + 1f, () =>
        {
            SonatTracking.ShowInterstitial("start_level", () =>
            {
                OnRestartLevel();
                UIManager.Instance.HideLive();
            });

        });

    }

    private void DataManager_OnUpdateLiveAction(object sender, EventArgs e)
    {
        Load();

        UpdateTimer();
        UpdateLives();

        StartCoroutine(RestoreLive());
    }

    private IEnumerator RestoreLive()
    {
        UpdateTimer();
        UpdateLives();

        while (currentLives < GameDefine.MAX_LIVES)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextLiveTime;
            bool isAdding = false;

            while (currentTime > counter)
            {
                if (currentLives < GameDefine.MAX_LIVES)
                {
                    isAdding = true;
                    currentLives++;

                    UpdateLives();

                    DateTime timeToAdd = lastAddedTime > counter ? lastAddedTime : counter;
                    counter = Helper.AddDuration(timeToAdd, GameDefine.RESTORELIVES_DURATION);
                }
                else
                {
                    break;
                }
            }

            if (isAdding)
            {
                lastAddedTime = DateTime.Now;
                nextLiveTime = counter;

                DataManager.Instance.SaveLive(currentLives, nextLiveTime, lastAddedTime);
            }

            UpdateTimer();

            yield return null;
        }
    }

    private void UpdateTimer()
    {
        if (currentLives >= GameDefine.MAX_LIVES || DataManager.Instance.IsUnlimitedLives)
        {
            StopCoroutine(nameof(RestoreLive));

            if (PopupManager.Instance.ShowPrePopup(() => UIManager.Instance.HideCoin(false)))
            {
                Hide(false, true, false);
            }
            else
            {
                if (GameManager.Instance.GameState == GameState.Ingame)
                {
                    Hide(true, false, true);
                    UIManager.Instance.HideCoin(false);
                }
                else
                {
                    Hide(false, false, true);
                }
            }
        }
        else
        {
            TimeSpan t = nextLiveTime - DateTime.Now;
            string value = string.Format("{0:D2}m {1:D2}s", (int)t.TotalMinutes, t.Seconds);

            timerText.text = value;
        }
    }

    private void UpdateLives()
    {
        //liveAmount.text = $"+{GameDefine.MAX_LIVES - currentLives}";
        liveAmount.text = $"+{GameDefine.MAX_LIVES}";

        for (int i = 0; i < toggleLive.Length; i++)
        {
            toggleLive[i].OnChanged(i < currentLives);
        }
    }

    private void Load()
    {
        currentLives = DataManager.Instance.playerData.lives.amount;
        nextLiveTime = Helper.StringToDate(DataManager.Instance.playerData.lives.nextLiveTime);
        lastAddedTime = Helper.StringToDate(DataManager.Instance.playerData.lives.lastAddedTime);
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        if (DataManager.Instance.playerData.lives.amount >= GameDefine.MAX_LIVES || DataManager.Instance.IsUnlimitedLives)
        {
            PopupManager.Instance.ShowPrePopup(null);
            return;
        }

        base.Show(isFadeMask, isSetPause);

        UIManager.Instance.ShowCoin();
    }

    private void OnRestartLevel()
    {
        if (DataManager.Instance.playerData.saveLevelData.currentLevel >= GameDefine.LEVEL_SHOW_PREBOOSTER)
        {
            Hide(false, true, true);
        }
        else
        {
            if (GameplayManager.Instance.difficulty != EDifficulty.Normal || TutorialManager.Instance.IsLevelUnlockObs())
            {
                Hide(false, true, true);
            }
            else
            {
                Hide(true, false, true);
            }
        }

        GameplayManager.Instance.StartLevel();
    }
}
