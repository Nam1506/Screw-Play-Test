using System;
using UnityEngine;

public class WidgetExclusive : WidgetTimer
{
    protected override void Awake()
    {
        button.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon("exclusive_deal", DataManager.Instance.playerData.saveLevelData.currentLevel, GameManager.Instance.eScreen.ToString());

            PopupManager.Instance.ShowExclusive(false);
        });
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        timeEnd = Helper.Next(DateTime.Today, DayOfWeek.Saturday);

        StartCountdown();
    }

    public override bool IsEnded()
    {
        DayOfWeek day = DateTime.Now.DayOfWeek;

        if ((day == DayOfWeek.Sunday) || (day == DayOfWeek.Saturday))
        {          
            Debug.Log("Exclusive deal is ended");

            return true;
        }

        Debug.Log("Exclusive deal is not ended");

        return false;
    }

    public bool IsBuyThisWeek()
    {
        DateTime timeBuy = Helper.StringToDate(DataManager.Instance.playerData.timeBuyExclusive);
        return Helper.IsSameWeek(timeBuy, DateTime.Now);
    }

    public override bool HasPurchased()
    {
        return IAPManager.HasPurchasedPack(ShopItemKey.ExclusiveDeal);
    }
}
