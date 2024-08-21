using System;
using UnityEngine;

public class WidgetWeekend : WidgetTimer
{
    protected override void Awake()
    {
        button.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon("weekend_sale", DataManager.Instance.playerData.saveLevelData.currentLevel, GameManager.Instance.eScreen.ToString());

            PopupManager.Instance.ShowWeekendSale(false);
        });
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        timeEnd = Helper.Next(DateTime.Today, DayOfWeek.Monday);

        StartCountdown();
    }

    public override bool IsEnded()
    {
        DayOfWeek day = DateTime.Now.DayOfWeek;

        if ((day != DayOfWeek.Sunday) && (day != DayOfWeek.Saturday))
        {
            return true;
        }

        return false;
    }

    public bool IsBuyThisWeek()
    {
        DateTime timeBuy = Helper.StringToDate(DataManager.Instance.playerData.timeBuyWeekendSale);
        return Helper.IsSameWeek(timeBuy, DateTime.Now);
    }

    public override bool HasPurchased()
    {
        return IAPManager.HasPurchasedPack(ShopItemKey.WeekendSale);
    }
}
