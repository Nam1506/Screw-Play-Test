using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UILive : UICurrency
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private ToggleScript liveIcon;
    [SerializeField] private RectTransform rectTransform;

    private DateTime nextLiveTime;
    private DateTime lastAddedTime;
    private DateTime unlimitedTimeEnd;
    private int currentLives;
    private int level = 1;

    protected override void DoMoreAction()
    {
        base.DoMoreAction();

        if (UIManager.Instance.IsShowingShop || !moreIcon.activeSelf) return;

        int level = DataManager.Instance.playerData.saveLevelData.currentLevel;

        SonatTracking.ClickIcon("lives", level, GameManager.Instance.eScreen.ToString());

        if (currentLives >= GameDefine.MAX_LIVES || DataManager.Instance.IsUnlimitedLives || UIManager.Instance.IsShowingShop 
            || PopupManager.Instance.IsShowingMoreLive)
        {
            return;
        }
        else
        {
            SoundManager.Instance.PlaySound(KeySound.BtnOpen);

            PopupBase showingPopup = PopupManager.Instance.GetShowingPopup();

            if (showingPopup != null)
            {
                PopupManager.Instance.PrePopups.Add(showingPopup);
                showingPopup.HideImmediate();
            }

            PopupManager.Instance.ShowMoreLives();

            SonatTracking.LogShowUI("user", level, "more_live", GameManager.Instance.eScreen.ToString(), "open");

            gameObject.SetActive(GameManager.Instance.GameState == GameState.Home);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        DataManager.Instance.OnUpdateLiveAction -= DataManager_OnUpdateLiveAction;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Load();

        UpdateLives();
        UpdateTimer();

        StartCoroutine(RestoreLive());

        if (DataManager.Instance.IsUnlimitedLives)
        {
            StartCoroutine(CountdownUnlimitedTime());
        }

        DataManager.Instance.OnUpdateLiveAction += DataManager_OnUpdateLiveAction;
    }

    private void DataManager_OnUpdateLiveAction(object sender, EventArgs e)
    {
        Load();

        UpdateTimer();
        UpdateLives();

        StartCoroutine(RestoreLive());

        if (DataManager.Instance.IsUnlimitedLives)
        {
            StartCoroutine(CountdownUnlimitedTime());
        }
    }

    protected override void UIShop_OnCloseShopAction(object sender, EventArgs e)
    {
        base.UIShop_OnCloseShopAction(sender, e);

        UpdateLives();
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

                DataManager.Instance.SaveLive(currentLives, nextLiveTime, lastAddedTime, unlimitedTimeEnd);
            }

            UpdateTimer();

            yield return null;
        }
    }

    public void UpdateTimer()
    {
        if (DataManager.Instance.IsUnlimitedLives)
        {
            TimeSpan t = unlimitedTimeEnd - DateTime.Now;
            string value;

            if (t.TotalHours >= 1)
            {
                value = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
            }
            else
            {
                value = string.Format("{0:D2}:{1:D2}", (int)t.TotalMinutes, t.Seconds);
            }

            timerText.text = value;
        }
        else if (currentLives >= GameDefine.MAX_LIVES)
        {
            timerText.text = "MAX";
        }
        else
        {
            TimeSpan t = nextLiveTime - DateTime.Now;
            string value = string.Format("{0:D2}:{1:D2}", (int)t.TotalMinutes, t.Seconds);

            timerText.text = value;
        }
    }

    public void UpdateLives()
    {
        if (!DataManager.Instance.IsUnlimitedLives)
        {
            livesText.text = currentLives.ToString();

            if (moreIcon != null)
            {            
                if (PopupManager.Instance.IsWinning || UIManager.Instance.IsShowingShop)
                {
                    moreIcon.SetActive(false);
                }
                else
                {
                    moreIcon.SetActive(currentLives < GameDefine.MAX_LIVES);
                }
            }
        }
        else
        {
            livesText.text = "";

            if (moreIcon != null)
                moreIcon.SetActive(false);
        }

        liveIcon.OnChanged(DataManager.Instance.IsUnlimitedLives);
    }

    private IEnumerator CountdownUnlimitedTime()
    {
        UpdateLives();

        while (unlimitedTimeEnd > DateTime.Now)
        {
            UpdateTimer();
            yield return null;
        }

        currentLives = GameDefine.MAX_LIVES;

        UpdateTimer();
        UpdateLives();

        DataManager.Instance.SaveLive(currentLives, nextLiveTime, lastAddedTime);
    }

    private void Load()
    {
        currentLives = DataManager.Instance.playerData.lives.amount;
        nextLiveTime = Helper.StringToDate(DataManager.Instance.playerData.lives.nextLiveTime);
        lastAddedTime = Helper.StringToDate(DataManager.Instance.playerData.lives.lastAddedTime);
        unlimitedTimeEnd = Helper.StringToDate(DataManager.Instance.playerData.lives.unlimitedTimeEnd);
    }
}
