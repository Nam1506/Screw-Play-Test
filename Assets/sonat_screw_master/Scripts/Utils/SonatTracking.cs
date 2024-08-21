using System;
using System.Collections.Generic;
using UnityEngine;

public class SonatTracking
{
    public enum EventNameEnum
    {
        complete_level,
        complete_tut_gameplay,
        complete_tut_booster,
        complete_rwd,
        paid_ad_impression,
        complete_scene_time
    }

    public static void LogLevelStart(int level, bool isFirstPlay, string continueWith = "", int continueTimes = 0)
    {

    }

    public static void LogLevelEnd(int level, int useBoosterCount, bool success, int play_time, bool is_first_play, string lose_cause,
        int move_count, int score, string continue_with = "", int continueTimes = 0)
    {

        Debug.Log("tiendd End");
        Debug.Log("tiendd level: " + level);
        Debug.Log("tiendd use_booster_count: " + useBoosterCount);
        Debug.Log("tiendd success: " + success);
        Debug.Log("tiendd play_time: " + play_time);
        Debug.Log("tiendd is_first_play: " + is_first_play);
        Debug.Log("tiendd lose_cause: " + lose_cause);
        Debug.Log("tiendd------------------------------------------------");
    }

    public static void LogLevelUp(int level)
    {

    }

    public static void LogUseBooster(int level, string name)
    {

    }

    public static void LogEventTutorialStart(string phase, int step = 0)
    {

        Debug.Log("tiendd tutorial start: " + phase);

        Debug.Log("tiendd------------------------------------------------");
    }

    public static void LogEventTutorialFinish(string phase, int step = 0)
    {

        Debug.Log("tiendd tutorial end: " + phase);

        Debug.Log("tiendd------------------------------------------------");
    }


    public static void ClickIcon(string icon, int level, string placement, Action callback = null)
    {
        callback?.Invoke();



        Debug.Log("tiendd Click Icon: " + icon);
    }

    public static void LogEarnCurrency(string currencyName, string currencyType, int value, string placement, string source, int level,
        string spendItemType = null, string spendItemId = null, bool isFirstBuy = false)
    {

    }

    public static void LogSpendCurrency(string currencyName, string currencyType, int value, string placement,
        string earnItemType = null, string earnItemId = null)
    {

        Debug.Log("tiendd: Spend Currency");
    }

    public static void LogShowRewardedAds(string placement, int level, string item_type, string item_id)
    {

    }

    public static void ShowInterstitial(string placement, Action action, int loadingIndex = 1)
    {

    }

    public static void PlayVideoAds(string item_id, Action reward, string item_type, string placement)
    {


    }

    public static void SetCurrentScreenName(EScreen eScreen)
    {
    }

    public static void LogStartLevelUA(int level)
    {
    }

    public static void LogCompleteLevelUA(int level)
    {

        if (Array.IndexOf(GameDefine.LOG_LEVEL_TRACKING, level) == -1)
        {
            return;
        }

        SendEventAF(EventNameEnum.complete_level.ToString() + $"_{level}");
        SendEventFireBase(EventNameEnum.complete_level.ToString() + $"_{level}");

        Debug.Log("tiendd LogCompleteLevel: " + level);
    }

    public static void LogCompleteRewardAdsUA()
    {
        var num = ++DataManager.Instance.playerData.numCompleteRewardAds;

        DataManager.Instance.Save();

        if (Array.IndexOf(GameDefine.LOG_COMPLETE_REWARD_ADS, num) == -1)
        {
            return;
        }

        var str = $"_{num.ToString("D2")}";

        if (num == 1)
        {
            str = null;
        }

        SendEventAF(EventNameUA.complete_rwd.ToString() + str);
        SendEventFireBase(EventNameUA.complete_rwd.ToString() + str);

        Debug.Log("Namtt LogCompleteRewardAdsUA: " + num);
    }

    public static void LogShowRewardAdsUA()
    {
        SendEventAF(EventNameUA.show_rwd.ToString());
        SendEventFireBase(EventNameUA.show_rwd.ToString());

        Debug.Log("Namtt LogShowRewardAdsUA");
    }

    public static void LogShowAdsUA()
    {
        var num = ++DataManager.Instance.playerData.numShowAds;

        DataManager.Instance.Save();

        if (Array.IndexOf(GameDefine.LOG_SHOW_ADS, num) == -1)
        {
            return;
        }

        var str = $"_{num.ToString("D2")}";

        if (num == 1)
        {
            str = null;
        }

        SendEventAF(EventNameUA.paid_ad_impression.ToString() + str);
        SendEventFireBase(EventNameUA.paid_ad_impression.ToString() + str);

        Debug.Log("Namtt LogShowAdsUA: " + num);
    }

    public static void LogUseBoosterUA()
    {
        SendEventAF(EventNameUA.use_booster_ua.ToString());
        SendEventFireBase(EventNameUA.use_booster_ua.ToString());

        Debug.Log("Namtt LogUseBoosterUA");
    }

    public static void LogClickIconShortcutUA()
    {
        SendEventAF(EventNameUA.click_icon_shortcut_ua.ToString());
        SendEventFireBase(EventNameUA.click_icon_shortcut_ua.ToString());

        Debug.Log("Namtt LogClickIconShortcutUA");
    }

    public static void LogEarnVirtualCurrencyUA()
    {
        SendEventAF(EventNameUA.earn_virtual_currency_ua.ToString());
        SendEventFireBase(EventNameUA.earn_virtual_currency_ua.ToString());

        Debug.Log("Namtt LogEarnVirtualCurrencyUA");
    }

    public static void LogCompleteTutGameplay(int level)
    {
        SendEventAF(EventNameEnum.complete_tut_gameplay.ToString() + $"_{level}");
        SendEventFireBase(EventNameEnum.complete_tut_gameplay.ToString() + $"_{level}");

        Debug.Log("tiendd LogCompleteTutGameplay: " + level);
    }

    public static void LogCompleteTutBooster(int level)
    {
        SendEventAF(EventNameEnum.complete_tut_booster.ToString() + $"_{level}");
        SendEventFireBase(EventNameEnum.complete_tut_booster.ToString() + $"_{level}");

        Debug.Log("tiendd LogCompleteTutBooster: " + level);
    }

    public static void LogCompleteRewardAds(int times)
    {
        SendEventAF(EventNameEnum.complete_rwd.ToString() + (times <= 1 ? "" : $"_{times}"));
        SendEventFireBase(EventNameEnum.complete_rwd.ToString() + (times <= 1 ? "" : $"_{times}"));

        Debug.Log("tiendd LogCompleteRewardAds: " + times);
    }

    public static void LogCountShowAds(int times)
    {
        SendEventAF(EventNameEnum.paid_ad_impression.ToString() + (times <= 1 ? "" : $"_{times}"));
        SendEventFireBase(EventNameEnum.paid_ad_impression.ToString() + (times <= 1 ? "" : $"_{times}"));

        Debug.Log("tiendd LogCountShowAds: " + times);
    }

    public static void SendEventAF(string s)
    {
    }

    public static void SendEventFireBase(string s)
    {
    }



    public static void LogCompleteSceneTime(int level, int time_msec, string placement)
    {
 
    }

    public static void LogCancelShopItem(string item_type, string item_id, string placement)
    {
 
    }

    public static void LogOpenShop()
    {
    }

    public static void LogShowUI(string openBy, int level, string name, string placement, string action)
    {
       
    }

    public static void LogBuyShopItem(string item_type, string item_id, float value, string currency, bool is_first_buy)
    {


    }

    public static void LogSpentTime(string type, string name, string placement, float time)
    {
 
    }
}

public enum EventNameUA
{
    start_level = 0,
    complete_level = 1,
    complete_tut_gameplay = 2,
    complete_tut_booster = 3,
    complete_rwd = 4,
    show_rwd = 5,
    paid_ad_impression = 6,
    use_booster_ua = 7,
    click_icon_shortcut_ua = 8,
    earn_virtual_currency_ua = 9,
}