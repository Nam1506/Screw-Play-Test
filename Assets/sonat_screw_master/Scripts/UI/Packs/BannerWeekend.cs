using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BannerWeekend : BannerBundle
{
    [SerializeField] private TextMeshProUGUI timerText;

    private DateTime timeEnd;

    private void OnEnable()
    {
        timeEnd = Helper.Next(DateTime.Today, DayOfWeek.Monday);

        StartCoroutine(IeCountdown());
    }

    private IEnumerator IeCountdown()
    {
        UpdateTimer();

        while (timeEnd > DateTime.Now)
        {
            UpdateTimer();
            yield return null;
        }

        Hide();
    }
    private void UpdateTimer()
    {
        TimeSpan t = timeEnd - DateTime.Now;
        string value;

        value = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);

        timerText.text = value;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
