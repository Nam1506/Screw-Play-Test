using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System.Linq;

public class PopupManager : SingletonBase<PopupManager>
{
    [Header("Popup Ref")]
    [SerializeField] private List<PopupBase> popups;
    [SerializeField] private PopupSetting popupSetting;
    [SerializeField] private PopupNoAds popupNoAds;
    [SerializeField] private PopupPolicy popupPolicy;
    [SerializeField] private PopupRestart popupRestart;
    [SerializeField] private PopupWin popupWin;
    [SerializeField] private PopupLose popupLose;
    [SerializeField] private PopupUnlockBooster popupUnlockBooster;
    [SerializeField] private PopupUnlockObstacle popupUnlockObstacle;
    [SerializeField] private PopupMoreBooster popupMoreBooster;
    [SerializeField] private PopupMoreLive popupMoreLive;
    [SerializeField] private PopupHardLevel popupHardLevel;
    [SerializeField] private PopupReward popupReward;
    [SerializeField] private PopupNoAds24h popupNoAds24h;
    [SerializeField] private PopupJustFun popupJustFun;
    [SerializeField] private PopupValueBundle popupValueBundle;
    [SerializeField] private PopupBestValueBundle popupBestValueBundle;
    [SerializeField] private PopupExclusive popupExclusive;
    [SerializeField] private PopupWeekend popupWeekendSale;
    [SerializeField] private PopupStarter popupStarter;
    [SerializeField] private PopupPlusOffer popupPlusOffer;
    [SerializeField] private PopupRate popupRate;
    [SerializeField] private PopupRaceStart popupRaceStart;
    [SerializeField] private PopupRaceLoading popupRaceLoading;
    [SerializeField] private PopupRace popupRace;
    [SerializeField] private PopupMiniShop popupMiniShop;
    [SerializeField] private PopupForceBooster popupForceBooster;
    [SerializeField] private PopupPurchasedFailed popupPurchasedFailed;
    [SerializeField] private PopupLevelChest popupLevelChest;
    [SerializeField] private PopupRewardLevelChest popupRewardLevelChest;

    [Header("Noti alert")]

    [SerializeField] private CanvasGroup notiAlert;
    [SerializeField] private TextMeshProUGUI notiText;

    [SerializeField] private List<PopupForce> configuredPopups;

    private bool isShowRate = false;
    private bool isShowJustFun = false;

    #region MASK_FIELDS

    [Header("Mask")]

    [SerializeField] private Image black_mask;
    [SerializeField] private Image second_black_mask;
    [SerializeField] private Image interactable_mask;

    public static Action<bool> OnSetBlackMask;
    public static Action<bool> OnSetSecondBlackMask;
    public static Action<bool> OnSetInteractableMask;

    public static Action OnFadeOutBlackMask;
    public static Action OnFadeInBlackMask;

    public static Action OnFadeOutSecondBlackMask;
    public static Action OnFadeInSecondBlackMask;

    public bool canSetInteractable = true;

    #endregion
    public List<PopupBase> PrePopups { get; set; }
    public List<Action> ForceShowPopupActions { get; set; }

    public bool IsWinning => popupWin.gameObject.activeInHierarchy && GameManager.Instance.GameState == GameState.Ingame;
    public bool IsShowingMoreLive => popupMoreLive.gameObject.activeInHierarchy;

    public float TimeDelayReward => popupReward.delay;
    public float TimeDelayRewardLevelChest => popupRewardLevelChest.delay;

    private void Awake()
    {
        PrePopups = new List<PopupBase>();
        ForceShowPopupActions = new List<Action>();

        OnSetBlackMask += SetBlackMask;
        OnSetInteractableMask += SetInteractableMask;

        OnFadeOutBlackMask += FadeOutBlackMask;
        OnFadeInBlackMask += FadeInBlackMask;

        OnFadeOutSecondBlackMask += FadeOutSecondBlackMask;
        OnFadeInSecondBlackMask += FadeInSecondBlackMask;

        GameManager.Instance.OnChangeStateAction += GameManager_OnChangeStateAction;
        GameplayManager.Instance.OnStartLevelAction += GameplayManager_OnStartLevelAction;

        IAPManager.OnPurchasedFailedAction += ShowPurchasedFailed;
    }

    private void GameplayManager_OnStartLevelAction(object sender, EventArgs e)
    {

    }

    private void GameManager_OnChangeStateAction(object sender, EventArgs e)
    {
        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        if (GameManager.Instance.GameState == GameState.Home)
        {
            if (level >= GameDefine.LEVEL_GO_HOME)
            {
                UIManager.Instance.CheckWidgets();

                CheckEvent();

                CheckPacks();

                if (packShow < 2)
                {
                    if (Array.IndexOf(GameDefine.LEVEL_SHOW_RATE, level) > 0 && !isShowRate)
                    {
                        ForceShowPopupActions.Add(() =>
                        {
                            Helper.WaitForTransition(() =>
                            {
                                isShowRate = true;
                                ShowRate(true);
                            });
                        });
                    }
                }
            }

            var canAnim = UIManager.Instance.UIHome.SetupBox();

            Helper.WaitForTransitionFull(() =>
            {
                UIManager.Instance.UIHome.Setup(canAnim);
            });
        }
    }

    private int packShow = 0;

    private void CheckPacks()
    {
        packShow = 0;

        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        List<PopupForce> forceShowPopups = new List<PopupForce>();

        foreach (PopupForce popup in configuredPopups)
        {
            Debug.Log("popup: " + popup);

            if (level >= popup.Config.levelStart &&
                popup.Config.appOpen &&
                (popup.Config.daily < 0 || popup.DailyShow < popup.Config.daily))
            {
                if (popup.Config.interval == 0 && popup.TimesShow > 0) continue;

                if (popup.Config.interval > 0 && (level - popup.LevelShow) % popup.Config.interval > 0)
                    continue;

                forceShowPopups.Add(popup);
            }
        }

        forceShowPopups = forceShowPopups.OrderBy(p => p.Config.order).ToList();

        foreach (PopupForce popup in forceShowPopups)
        {
            if (popup is PopupExclusive && CheckShowExclusive())
            {
                UpdateConfig(popup);

                CheckShowPacks(() => ShowExclusive(true));
            }
            else if (popup is PopupWeekend && CheckShowWeekendSale())
            {
                UpdateConfig(popup);

                CheckShowPacks(() => ShowWeekendSale(true));
            }
            else if (popup is PopupPlusOffer && CheckShowPlusOffer())
            {
                UpdateConfig(popup);

                CheckShowPacks(() => ShowPlusOffer(true));
            }
            else if (popup is PopupStarter || popup is PopupBestValueBundle)
            {
                if (Helper.IsPurchaserInitFailed()) continue;

                if (popup is PopupBestValueBundle && CheckShowBestValueBundle(true))
                {
                    UpdateConfig(popup);

                    CheckShowPacks(() => ShowBestValueBundle(true));
                }
                else if (popup is PopupStarter && CheckShowStarter())
                {
                    UpdateConfig(popup);

                    CheckShowPacks(() => ShowStarterPack(true));
                }
            }
        }
    }

    private void UpdateConfig(PopupForce popup)
    {
        popup.DailyShow++;
        popup.TimesShow++;
        popup.LevelShow = DataManager.Instance.playerData.saveLevelData.currentLevel;
    }

    private void CheckShowPacks(Action showAction)
    {
        if (packShow < PopupConfig.MAX_POPUP_OPEN)
        {
            packShow++;

            ForceShowPopupActions.Add(() =>
            {
                Helper.WaitForTransition(() => { showAction?.Invoke(); });
            });
        }
    }

    public void CheckEvent()
    {
        var levelChestWidget = UIManager.Instance.LevelChest;
        var raceWidget = UIManager.Instance.Race;

        Action action = null;
        List<Action> completeActions = new();

        if (DataManager.Instance.playerData.saveLevelData.currentLevel < GameDefine.LEVEL_GO_HOME)
        {
            levelChestWidget.gameObject.SetActive(false);
        }
        else
        {
            levelChestWidget.gameObject.SetActive(true);

            // Init
            if (levelChestWidget.CanChange)
            {
                if (LevelChestManager.IsCompleted())
                {
                    completeActions.Add(levelChestWidget.OnSuccess);
                }
                // No need to match because enter game from the home screen (value update by Awake)
            }
            // From Win, Progress Level Chest won't update because of waiting from Animation
            else
            {
                action += levelChestWidget.ForceUpdateProgress;
            }
        }


        if (RaceEvent.CheckRacing())
        {
            if (!raceWidget.CanChange)
            {
                action += raceWidget.ForceUpdateProgress;
            }
        }

        if (RaceEvent.raceData.isForceShowFinish)
        {
            completeActions.Add(() => ShowRace(true));
        }

        if (action != null)
        {
            action += () =>
            {
                DOVirtual.DelayedCall(0.6f, () =>
                {
                    UIManager.Instance.UIHome.PopPlayBtn();
                });
            };

            ForceShowPopupActions.Add(action);
        }

        foreach (var act in completeActions)
        {
            var a = act;

            //a += (() =>
            //{
            //    UIManager.Instance.UIHome.PopPlayBtn();
            //});

            ForceShowPopupActions.Add(act);
        }

        if (RaceEvent.CheckCanStartRacing())
        {
            if (RaceEvent.raceData.isForceShowStart)
            {
                ForceShowPopupActions.Add(() =>
                {
                    UIManager.Instance.UIHome.PopPlayBtn();

                    ShowRaceStart(true);
                });
            };
        }
    }


    public void ClearForceAction()
    {
        ForceShowPopupActions.Clear();
    }

    public void StartForceAction()
    {
        Debug.Log("StartForce");

        if (HasAnyForceAction())
        {
            ForceShowPopupActions[0].Invoke();
        }
    }

    public void CompleteForceAction()
    {
        if (HasAnyForceAction())
        {
            ForceShowPopupActions.RemoveAt(0);

            StartForceAction();
        }
    }

    public bool HasAnyForceAction()
    {
        return ForceShowPopupActions.Count > 0;
    }

    public int GetLastAction()
    {
        return ForceShowPopupActions.Count - 1;
    }

    public void AddMoreAction(Action action, int index)
    {
        ForceShowPopupActions[index] += action;
    }

    public void ShowNotiAlert(string text)
    {
        if (notiAlert.gameObject.activeInHierarchy)
        {
            return;
        }

        notiAlert.alpha = 0;
        notiAlert.gameObject.SetActive(true);
        notiText.text = text;

        notiAlert.DOFade(1, 0.35f).SetUpdate(true);
        notiAlert.DOFade(0, 0.25f).SetDelay(1).SetUpdate(true).OnComplete(() =>
        {
            notiAlert.gameObject.SetActive(false);
        });
    }

    public bool ShowPrePopup(Action success)
    {
        if (PrePopups.Count > 0)
        {
            success?.Invoke();

            PopupBase prePopup = PrePopups[PrePopups.Count - 1];
            PrePopups.RemoveAt(PrePopups.Count - 1);

            prePopup.Show(true);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddCurPopupToPre()
    {
        var popup = GetShowingPopup();

        if (popup == null) return;

        PrePopups.Add(popup);
    }

    public PopupBase GetShowingPopup()
    {
        foreach (PopupBase popup in popups)
        {
            if (popup.gameObject.activeInHierarchy)
                return popup;
        }
        return null;
    }

    public void CloseAllPopups()
    {
        foreach (PopupBase popup in popups)
        {
            popup.gameObject.SetActive(false);
        }

        SetBlackMask(false);
    }

    public void CloseAllPopupFadeAll()
    {
        foreach (PopupBase popup in popups)
        {
            popup.HideOriginal(false, false, true);
        }
    }

    public void ShowSetting()
    {
        popupSetting.Show(true);
    }

    public void ShowNoAds()
    {
        popupNoAds.Show(true);
    }

    public void ShowPolicy()
    {
        popupPolicy.Show(true);
    }

    public void ShowRestart(ERestartFrom eRestartFrom)
    {
        popupRestart.AddAction(eRestartFrom);

        popupRestart.Show(true);
    }

    public void ShowWin()
    {
        CloseAllPopups();

        SonatTracking.LogShowUI("auto", DataManager.Instance.playerData.saveLevelData.currentLevel, "Win", "Ingame", "open");

        popupWin.Show(false);

        SonatTracking.SetCurrentScreenName(EScreen.Win);

        UIManager.Instance.ShowCoin();
        UIManager.Instance.ShowLive();
    }

    public void ShowLose()
    {
        CloseAllPopups();

        SonatTracking.LogShowUI("auto", DataManager.Instance.playerData.saveLevelData.currentLevel, "Lose", "Ingame", "open");

        popupLose.Show(true);

        SonatTracking.SetCurrentScreenName(EScreen.Lose);
    }

    public void ShowUnlockBooster(EBoosterType type)
    {
        SonatTracking.LogShowUI("auto", DataManager.Instance.playerData.saveLevelData.currentLevel, "unlock_booster", "Ingame", "open");

        popupUnlockBooster.SetBooster(type);
        popupUnlockBooster.Show(true);
    }

    public void ShowForceBooster(EBoosterType type)
    {
        SonatTracking.LogShowUI("auto", DataManager.Instance.playerData.saveLevelData.currentLevel, "force_booster", "Ingame", "open");

        popupForceBooster.SetBooster(type);
        popupForceBooster.Show(true);
    }

    public void ShowUnlockObstacle(EObstacleType type)
    {
        SonatTracking.LogShowUI("auto", DataManager.Instance.playerData.saveLevelData.currentLevel, "unlock_obstacle", "Ingame", "open");

        popupUnlockObstacle.SetObstalce(type);
        popupUnlockObstacle.Show(true);
    }

    public void ShowMoreBooster(EBoosterType type)
    {
        SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "more_booster",
        GameManager.Instance.eScreen.ToString(), "open");

        popupMoreBooster.SetBooster(type);
        popupMoreBooster.Show(true, false, true);
    }

    public void ShowPreBooster()
    {
        SonatTracking.LogShowUI("auto", DataManager.Instance.playerData.saveLevelData.currentLevel, "pre_booster",
        GameManager.Instance.eScreen.ToString(), "open");

        popupMoreBooster.ShowPreBooster();
    }

    public void ShowMoreLives()
    {
        SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "more_live",
        GameManager.Instance.eScreen.ToString(), "open");

        popupMoreLive.Show(true);
    }

    public void ShowHardLevel(EDifficulty difficulty)
    {
        popupHardLevel.Show(difficulty);
    }

    public void ShowReward(List<Reward> rewards, bool isSetPlayingWhenHide = false)
    {
        popupReward.SetRewards(rewards);
        popupReward.Show(isSetPlayingWhenHide);
    }

    public void ShowRewardLevelChest(List<Reward> rewards)
    {
        popupRewardLevelChest.SetRewards(rewards);
        popupRewardLevelChest.Show();
    }

    private bool isShowNoads24h = false;
    private bool isShowValueBundle = false;
    private bool isShowBestValueBundle = false;
    private bool isShowExclusive = false;
    private bool isShowWeekendSale = false;
    private bool isShowStarter = false;
    private bool isShowPlusOffer = false;

    public void ShowNoAds24h(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "no_ads_24h", "Home", "open");

        popupNoAds24h.Show(true, isAuto);

        if (isAuto)
        {
            isShowNoads24h = true;
        }
    }

    public bool CheckShowNoAds24h(bool isAuto)
    {
        return false;
    }

    public void ShowValueBundle(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "value_bundle",
            GameManager.Instance.eScreen.ToString(), "open");

        popupValueBundle.Show(true, isAuto);

        if (isAuto)
        {
            isShowValueBundle = true;
        }
    }

    public bool CheckShowValueBundle(bool isAuto)
    {
        return false;
    }

    public void ShowBestValueBundle(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "best_value_bundle",
            GameManager.Instance.eScreen.ToString(), "open");

        popupBestValueBundle.Show(true, isAuto);

        if (isAuto)
        {
            isShowBestValueBundle = true;
        }
    }

    public bool CheckShowBestValueBundle(bool isAuto)
    {
        return false;
    }

    public void ShowExclusive(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "exclusive_deal", "Home", "open");

        popupExclusive.Show(true, isAuto);

        if (isAuto)
        {
            isShowExclusive = true;
        }
    }

    private bool CheckShowExclusive()
    {
        return /*!isShowWeekendSale &&*/ UIManager.Instance.IsShowExclusive();
    }

    public void ShowWeekendSale(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "weekend_sale", "Home", "open");

        popupWeekendSale.Show(true, isAuto);

        if (isAuto)
        {
            isShowWeekendSale = true;
        }
    }

    private bool CheckShowWeekendSale()
    {
        return !isShowWeekendSale && UIManager.Instance.IsShowWeekendSale();
    }

    public void ShowStarterPack(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "starter_bundle", "Home", "open");

        popupStarter.Show(true, isAuto);

        if (isAuto)
        {
            isShowStarter = true;
        }
    }

    private bool CheckShowStarter()
    {
        return /*!isShowStarter && */UIManager.Instance.IsShowStarterPack();
    }

    public void ShowPlusOffer(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "plus_offer",
            GameManager.Instance.eScreen.ToString(), "open");

        popupPlusOffer.Show(true, isAuto);

        if (isAuto)
        {
            isShowPlusOffer = true;
        }
    }

    private bool CheckShowPlusOffer()
    {
        return /*!isShowPlusOffer && */UIManager.Instance.IsShowPlusOffer();
    }

    public void ShowJustFun(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "no_ads_just_fun",
            GameManager.Instance.eScreen.ToString(), "open");

        popupJustFun.Show(true);
    }

    public void ShowRate(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "rate",
            GameManager.Instance.eScreen.ToString(), "open");

        popupRate.Show(true, isAuto);
    }

    public void ShowRaceStart(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "race_start",
            GameManager.Instance.eScreen.ToString(), "open");

        popupRaceStart.Show(true);
    }

    public void ShowRaceLoading()
    {
        popupRaceLoading.Show(true);
    }

    public void ShowRace(bool isAuto)
    {
        SonatTracking.LogShowUI(isAuto ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, "race",
            GameManager.Instance.eScreen.ToString(), "open");

        popupRace.Show(false);
    }

    public void ShowAllRewardRace()
    {
        popupRace.ShowAllReward();
    }

    public bool IsShowingMiniShop => popupMiniShop.gameObject.activeSelf;
    public bool IsShowingReward => popupReward.gameObject.activeSelf;

    public void ShowMiniShop()
    {
        SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "mini_shop",
        GameManager.Instance.eScreen.ToString(), "open");

        popupMiniShop.Show(true);
    }

    public void ShowPurchasedFailed()
    {
        SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "purchased_failed",
        GameManager.Instance.eScreen.ToString(), "open");

        popupPurchasedFailed.Show(false);
    }

    public void ShowLevelChest()
    {
        SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "level_chest",
        GameManager.Instance.eScreen.ToString(), "open");

        popupLevelChest.Show(true);
    }

    public void StartWaitForReward(bool canRevive)
    {
        StartCoroutine(IeWaitForReward(canRevive));
    }

    private IEnumerator IeWaitForReward(bool canRevive)
    {
        yield return new WaitForSeconds(0.2f);

        while (UIManager.Instance.IsReceivingRewards())
        {
            yield return null;
        }

        if (canRevive)
        {
            popupLose.Hide(true, false, true);
            popupLose.Revive(false);

            CloseAllPopupFadeAll();
        }
    }

    public void LoadConfig()
    {
        Debug.Log("LoadConfig");

        foreach (PopupForce popup in configuredPopups)
        {
            popup.LoadConfig();
        }

    }

    #region MASK_FUNCTION

    private void SetBlackMask(bool state)
    {
        black_mask.gameObject.SetActive(state);
    }

    private void SetSecondBlackMask(bool state)
    {
        second_black_mask.gameObject.SetActive(state);
    }

    private void SetInteractableMask(bool state)
    {
        if (!canSetInteractable) return;

        Debug.Log("set: " + state);

        interactable_mask.gameObject.SetActive(state);
    }

    private void FadeInBlackMask()
    {
        black_mask.DOFade(0f, PopupConfig.TIME_FADE_MASK)
            .OnComplete(() =>
            {
                SetBlackMask(false);
            });
    }

    private void FadeOutBlackMask()
    {
        SetBlackMask(true);

        black_mask.DOFade(PopupConfig.FADE_MASK_VALUE, PopupConfig.TIME_FADE_MASK);
    }

    private void FadeInSecondBlackMask()
    {
        second_black_mask.DOFade(0f, PopupConfig.TIME_FADE_MASK)
            .OnComplete(() =>
            {
                SetSecondBlackMask(false);
            });
    }

    private void FadeOutSecondBlackMask()
    {
        SetSecondBlackMask(true);

        second_black_mask.DOFade(PopupConfig.FADE_MASK_VALUE, PopupConfig.TIME_FADE_MASK);
    }

    private void TapToFade(bool onHold, CanvasGroup canvasGroup)
    {
        if (!onHold)
        {
            black_mask.DOFade(PopupConfig.FADE_MASK_VALUE, PopupConfig.TIME_FADE_MASK);
            canvasGroup.DOFade(1, PopupConfig.TIME_FADE_MASK);
        }
        else
        {
            black_mask.DOFade(0f, PopupConfig.TIME_FADE_MASK);
            canvasGroup.DOFade(0, PopupConfig.TIME_FADE_MASK);
        }
    }

    public bool IsShowingMask()
    {
        return black_mask.gameObject.activeSelf;
    }
    #endregion
}

public enum ERestartFrom
{
    Lose = 0,
    SettingReplay = 1,
    SettingHome = 2
}