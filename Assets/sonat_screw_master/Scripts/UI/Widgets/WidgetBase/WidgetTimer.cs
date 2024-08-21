using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WidgetTimer : MonoBehaviour
{
    [SerializeField] protected Button button;
    [SerializeField] protected TextMeshProUGUI timerText;

    protected DateTime timeEnd;

    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {
        Debug.Log("abc " + name + " enable");
    }

    public virtual void StartCountdown()
    {
        if (gameObject.activeInHierarchy)
        {
            Debug.Log("abc " + name + " start countdown");
            StartCoroutine(IeCountdown());
        }
    }

    public virtual bool IsEnded()
    {
        return false;
    }

    public virtual bool HasPurchased()
    {
        return false;
    }

    private IEnumerator IeCountdown()
    {
        UpdateTimer();

        while (timeEnd > DateTime.Now)
        {
            UpdateTimer();
            yield return null;
        }

        gameObject.SetActive(false);
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
}
