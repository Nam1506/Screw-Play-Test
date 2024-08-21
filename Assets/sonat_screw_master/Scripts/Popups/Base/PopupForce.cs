using System;
using UnityEngine;

public class PopupForce : PopupBase
{
    public PopupRemoteConfig Config;

    public int DailyShow
    {
        get
        {
            string s = PlayerPrefs.GetString(name + "_TIMESHOW".ToUpper(), "");

            if (string.IsNullOrEmpty(s) || s.StringToDate().Date != DateTime.Now.Date)
            {
                DailyShow = 0;
                return 0;
            }
            else
            {
                return PlayerPrefs.GetInt(name + "_DAILYSHOW".ToUpper(), 0);
            }
        }

        set
        {
            PlayerPrefs.SetInt(name + "_DAILYSHOW".ToUpper(), value);

            string s = PlayerPrefs.GetString(name + "_TIMESHOW".ToUpper(), "");

            if (string.IsNullOrEmpty(s) || s.StringToDate().Date != DateTime.Now.Date)
            {
                PlayerPrefs.SetString(name + "_TIMESHOW".ToUpper(), DateTime.Now.DateTimeToString());
            }
        }
    }

    public int LevelShow { get; set; }
    public int TimesShow { get; set; }

    protected bool isAutoOpened;

    public virtual void Show(bool isFadeMask, bool isAuto, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        isAutoOpened = isAuto;

        UIManager.Instance.SendCurrencyToBack();

        Debug.Log("abc Show: " + name);
    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        PopupManager.Instance.CompleteForceAction();

        if (PopupManager.Instance.HasAnyForceAction())
        {
            base.Hide(isSetPlaying, true, isFade);
        }
        else
        {
            UIManager.Instance.SendCurrencyToFront();

            base.Hide(isSetPlaying, isKeepingMask, isFade);
        }

        SonatTracking.LogShowUI(isAutoOpened ? "auto" : "user", DataManager.Instance.playerData.saveLevelData.currentLevel, name,
            GameManager.Instance.eScreen.ToString(), "close");
    }

    public virtual void LoadConfig()
    {
        LevelShow = DataManager.Instance.playerData.saveLevelData.currentLevel;
    }
}
