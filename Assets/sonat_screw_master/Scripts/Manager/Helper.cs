using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public static class Helper
{
    public static void OnChanged<T>(this IEnumerable<T> scripts, bool value) where T : ToggleScript
    {
        foreach (var toggleScript in scripts)
            if (toggleScript == null)
                Debug.LogError("IndexBindingScript is null");
            else
                toggleScript.OnChanged(value);
    }

    public static List<T> GetRandomElemntsInList<T>(List<T> list, int count)
    {
        List<T> org = new List<T>(list);
        List<T> t = new List<T>();
        List<int> allIndexs = new List<int>(Enumerable.Range(0, list.Count));
        List<float> chances = new List<float>(Enumerable.Repeat(1f, list.Count));

        for (int i = 0; i < count; i++)
        {
            int rdIndex = GetRandomIndexInList(chances);
            if (rdIndex >= 0)
            {
                t.Add(org[allIndexs[rdIndex]]);
                chances.RemoveAt(rdIndex);
                allIndexs.RemoveAt(rdIndex);
            }
        }
        return t;
    }

    public static int GetRandomIndexInList(List<float> chances)
    {
        float total = 0;
        foreach (float c in chances)
        {
            total += c;
        }
        float rg = GetRandomFloat() * total;
        int index = 0;
        float checkChance = 0;
        foreach (float c in chances)
        {
            checkChance += c;
            if (rg < checkChance) return index;
            index++;
        }
        return -1;
    }

    public static float GetRandomFloat()
    {
        return (float)NextDouble();
    }

    public static double NextDouble()
    {
        System.Random rand = new System.Random();

        return (double)rand.NextDouble();
    }

    public static string LoadTextFileFromResource(string path)
    {
        string filePath = path.Replace(".json", "");
        TextAsset file = Resources.Load<TextAsset>(filePath);
        return file.text;
    }

    public static Color GetColorFromScrew(EScrewColor screwColor)
    {
        switch (screwColor)
        {
            case EScrewColor.Blue:
                return new Color(0.1215686f, 0.8509804f, 1);
            case EScrewColor.Green:
                return new Color(0.7294118f, 0.972549f, 0.1568628f);
            case EScrewColor.Ocean_Blue:
                return new Color(0.2039216f, 0.5686275f, 1);
            case EScrewColor.OldGreen:
                return new Color(0.01960784f, 0.8901961f, 0.5294118f);
            case EScrewColor.Orange:
                return new Color(1, 0.627451f, 0.09019608f);
            case EScrewColor.Pink:
                return new Color(1, 0.4509804f, 0.854902f);
            case EScrewColor.Purple:
                return new Color(0.6941177f, 0.4627451f, 1);
            case EScrewColor.Red:
                return new Color(0.972549f, 0.1529412f, 0.2039216f);
            case EScrewColor.Violet:
                return new Color(0.8627451f, 0.6588235f, 1);
            case EScrewColor.Yellow:
                return new Color(0.9921569f, 0.854902f, 0.08627451f);
        }

        return Color.blue;
    }

    public static EBoosterType ConvertRewardToBooster(RewardID id)
    {
        switch (id)
        {
            case RewardID.hammer:
                return EBoosterType.Hammer;
            case RewardID.addHole:
                return EBoosterType.AddHole;
            case RewardID.addBox:
                return EBoosterType.AddBox;
            default:
                return EBoosterType.None;
        }
    }

    public static void SetActiveAll<T>(this IList<T> list, bool value) where T : Component
    {
        foreach (var x1 in list)
            x1.gameObject.SetActive(value);
    }

    public static Vector3 GetFixedPosition(Vector3 position)
    {

        return new Vector3(RoundFloat(position.x), RoundFloat(position.y), RoundFloat(position.z));
    }

    public static float RoundFloat(float value)
    {
        return Mathf.Round(value * 100f) / 100f;
    }
    public static string GetLayerName(int layer)
    {
        return "Layer " + layer;
    }

    public static int GetRandNum(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static DateTime StringToDate(this string date)
    {
        if (string.IsNullOrEmpty(date))
        {
            //return DateTime.Now;
            return DateTime.MinValue;
        }

        try
        {
            return DateTime.ParseExact(date, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return DateTime.Now;
        //bool success = DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);

        //return result;
    }

    public static string DateTimeToString(this DateTime dateTime)
    {
        return dateTime.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
    }

    public static string GetUnixTimeSecStr()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    }

    public static long GetUnixTimeSec()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public static DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }

    public static DateTime AddDuration(DateTime time, float duration)
    {
        return time.AddSeconds(duration);
    }

    public static DateTime Next(this DateTime from, DayOfWeek dayOfWeek)
    {
        int start = (int)from.DayOfWeek;
        int target = (int)dayOfWeek;
        if (target <= start)
            target += 7;
        return from.AddDays(target - start);
    }

    public static void SetAlpha(this SpriteRenderer sprite, float alpha)
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
    }

    public static void FadeObject(bool isFadeIn, CanvasGroup canvasGroup, float duration, Action action = null)
    {
        if (isFadeIn)
        {
            canvasGroup.gameObject.SetActive(true);

            canvasGroup.alpha = 0f;

            canvasGroup.DOFade(1f, duration).OnComplete(() => action?.Invoke());
        }
        else
        {
            canvasGroup.alpha = 1f;

            canvasGroup.DOFade(0f, duration).OnComplete(() =>
            {
                canvasGroup.gameObject.SetActive(false);
                canvasGroup.alpha = 1f;
                action?.Invoke();
            });
        }
    }

    public static void SetLocalRotateZ(this Transform transform, float z)
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
    }

    public static void SetPositionZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    public static void SetLocalPositionX(this Transform transform, float x)
    {
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }

    public static void SetLocalPositionZ(this Transform transform, float z)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
    }

    public static void SetLocalScaleX(this Transform transform, float scaleX)
    {
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    public static void SetLocalScaleY(this Transform transform, float scaleY)
    {
        transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
    }

    public static void SetAlpha(this Color color, float alpha)
    {
        color.a = alpha;
    }

    public static void SetSizeDelta(this RectTransform rectTransform, Vector2 sizeDelta)
    {
        rectTransform.sizeDelta = sizeDelta;
    }

    public static void SetSizeDeltaX(this RectTransform rectTransform, float x)
    {
        rectTransform.sizeDelta = new Vector2(x, rectTransform.sizeDelta.y);
    }

    public static void SetSizeDeltaY(this RectTransform rectTransform, float y)
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, y);
    }

    public static void WaitForTransition(Action callback)
    {
        if (CanvasManager.Instance.IsPlayingTransiton())
        {
            DOVirtual.DelayedCall(GameDefine.TRANSITION_DURATION_IDLE + GameDefine.TRANSITION_DURATION_OUT, () =>
            {
                callback?.Invoke();
            });
        }
        else
        {
            callback?.Invoke();
        }
    }

    public static void WaitForTransitionFull(Action callback)
    {
        if (CanvasManager.Instance.IsPlayingTransiton())
        {
            DOVirtual.DelayedCall(GameDefine.TRANSITION_DURATION_IN + GameDefine.TRANSITION_DURATION_IDLE + GameDefine.TRANSITION_DURATION_OUT, () =>
            {
                callback?.Invoke();
            });
        }
        else
        {
            callback?.Invoke();
        }
    }

    public static void DelayForTransition(Action callback)
    {
        DOVirtual.DelayedCall(GameDefine.TRANSITION_DURATION_IN, () =>
        {
            callback?.Invoke();
        });
    }

    public static bool IsPurchaserInitFailed()
    {
        return true;
    }

    public static bool IsSameWeek(DateTime date1, DateTime date2)
    {
        // Ensure both dates are in the same calendar week and year
        Calendar calendar = CultureInfo.InvariantCulture.Calendar;

        // Get the ISO 8601 week of the year for each date
        int week1 = GetIso8601WeekOfYear(date1);
        int week2 = GetIso8601WeekOfYear(date2);

        // Get the year for each date considering the week number
        int year1 = GetIso8601Year(date1);
        int year2 = GetIso8601Year(date2);

        // Check if the week numbers and years match
        return week1 == week2 && year1 == year2;
    }

    private static int GetIso8601WeekOfYear(DateTime date)
    {
        // Seriously cheat. If its Monday, Tuesday or Wednesday, then it will
        // always be the same week number as whatever Thursday, Friday or Saturday
        // are, and we always get those right
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            date = date.AddDays(3);
        }

        // Return the week of our adjusted day
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    private static int GetIso8601Year(DateTime date)
    {
        // Get the week number
        int week = GetIso8601WeekOfYear(date);

        // Adjust the year if needed
        if (week == 1 && date.Month == 12)
        {
            return date.Year + 1;
        }
        else if (week >= 52 && date.Month == 1)
        {
            return date.Year - 1;
        }

        return date.Year;
    }

    public static float GetTimeAnim(SkeletonGraphic anim, string name)
    {
        return anim.Skeleton.Data.FindAnimation(name).Duration;
    }

    public static float GetTimeAnim(SkeletonAnimation anim, string name)
    {
        return anim.Skeleton.Data.FindAnimation(name).Duration;
    }

    public static Vector3 GetTopScreen()
    {
        Vector3 topScreenPosition = new Vector3(Screen.width / 2, Screen.height, 0);

        Vector3 topWorldPosition = Camera.main.ScreenToWorldPoint(topScreenPosition);

        return topWorldPosition;
    }

    public static float UnitsPerPixel(this Camera cam)
    {
        var p1 = cam.ScreenToWorldPoint(Vector3.zero);
        var p2 = cam.ScreenToWorldPoint(Vector3.right);
        return Vector3.Distance(p1, p2);
    }

    public static float PixelsPerUnit(this Camera cam)
    {
        return 1 / UnitsPerPixel(cam);
    }
}
