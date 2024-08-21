using System;
using UnityEngine;

public class WidgetPlusOffer : WidgetTimer
{
    protected override void Awake()
    {
        button.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon("1+1_offer", DataManager.Instance.playerData.saveLevelData.currentLevel, GameManager.Instance.eScreen.ToString());

            PopupManager.Instance.ShowPlusOffer(false);
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

        if ((day <= DayOfWeek.Tuesday) || (day >= DayOfWeek.Saturday))
        {
            return true;
        }

        return false;
    }

    public bool IsBuyThisWeek()
    {
        DateTime timeBuy = Helper.StringToDate(DataManager.Instance.playerData.timeBuyPlusOffer);
        return Helper.IsSameWeek(timeBuy, DateTime.Now);
    }

    public override bool HasPurchased()
    {
        return IAPManager.HasPurchasedPack(ShopItemKey.PlusOffer);
    }
}
