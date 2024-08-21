using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMainRace : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private List<TextMeshProUGUI> topPlayerNames;
    [SerializeField] private List<TopUserInfo> topPlayerButtons;
    [SerializeField] private List<RaceRoad> roads;
    [SerializeField] private List<RaceItem> raceItems;

    private DateTime timeEnd;

    private bool isFirstJoin = false;

    private Tween delayLoop;

    private void OnEnable()
    {
        if (isFirstJoin)
        {
            isFirstJoin = false;

            foreach (var button in topPlayerButtons)
            {
                button.ToggleInfo();
                button.SetCanClick(false);
            }
        }
        else
        {
            foreach (var button in topPlayerButtons)
            {
                button.HideRewardInfo();
                button.SetCanClick(true);

                button.AddAction(() =>
                {
                    SoundManager.Instance.PlaySound(KeySound.BtnOpen);

                    //foreach (var button1 in topPlayerButtons)
                    //{
                    //    if (button == button1) continue;

                    //    button1.HideRewardInfo();
                    //}

                    button.ToggleInfo();
                });
            }
        }
    }

    public void SetFirstJoin(bool state)
    {
        isFirstJoin = state;
    }

    public void SetData()
    {
        List<RaceMember> raceMembers = new List<RaceMember>(RaceEvent.raceData.raceMembers);
        RaceMember myself = RaceMember.CreateMySelf();
        raceMembers.Insert(2, myself);

        if (RaceEvent.raceData.ranks.Count > 0)
        {
            for (int i = 0; i < topPlayerNames.Count; i++)
            {
                if (RaceEvent.raceData.ranks.Count <= i)
                    break;

                topPlayerNames[i].text = raceMembers.Find(e => e.id == RaceEvent.raceData.ranks[i]).name;
            }
        }
        else
        {
            topPlayerNames[0].text = "1st place";
            topPlayerNames[1].text = "2nd place";
            topPlayerNames[2].text = "3rd place";
        }

        bool haveMove = false;

        for (int i = 0; i < raceMembers.Count; i++)
        {
            if (raceMembers[i].level > RaceEvent.raceData.winLevelTarget - 15)
            {
                haveMove = true;
            }

            roads[i].SetData(raceMembers[i], i + 1);
            raceItems[i].SetData(raceMembers[i], i + 1);
            //grounds[i].SetData(raceMembers[i], i + 1);
        }

        if (haveMove)
        {
            SoundManager.Instance.PlaySound(KeySound.Race_Mine);

            delayLoop = DOVirtual.DelayedCall(0.667f * 2f, () =>
            {
                SoundManager.Instance.PlaySound(KeySound.Race_Mine_Loop, true);
            });

            delayLoop.Play();
        }
        else
        {
            SoundManager.Instance.PlaySound(KeySound.Race_Mine_Loop, true);
        }

        timeEnd = Helper.StringToDate(RaceEvent.raceData.timeStart).AddSeconds(RaceEvent.TIME_EVENT * GameConfig.HOUR_TO_SEC);

        StartCoroutine(IeCountdown());

        foreach (var button in topPlayerButtons)
        {
            button.HideRewardInfo();
        }
    }

    public void ResetData()
    {
        for (int i = 0; i < roads.Count; i++)
        {
            roads[i].Reset();
        }
    }

    private IEnumerator IeCountdown()
    {
        UpdateTimer();

        while (timeEnd > DateTime.Now)
        {
            UpdateTimer();
            yield return null;
        }

        timerText.text = "Time up!";
    }

    private void UpdateTimer()
    {
        TimeSpan t = timeEnd - DateTime.Now;
        string value;

        if (t.TotalDays >= 1)
        {
            value = string.Format("{0:D1}d {1:D2}h", (int)t.TotalDays, t.Hours);
        }
        else
        {
            value = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
        }

        timerText.text = value;
    }

    public void DestroyLoopSound()
    {
        delayLoop.Kill();
        SoundManager.Instance.StopLoopSound();
    }
}
