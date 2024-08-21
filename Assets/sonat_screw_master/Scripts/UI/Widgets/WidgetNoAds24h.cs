using System;

public class WidgetNoAds24h : WidgetTimer 
{
    private const int DAY_DURATION = 2;

    protected override void Awake()
    {
        button.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon("no_ads_24h", DataManager.Instance.playerData.saveLevelData.currentLevel, "Home");

            PopupManager.Instance.ShowNoAds24h(false);
        });
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (string.IsNullOrEmpty(DataManager.Instance.playerData.timeShowNoAds24h))
        {
            DataManager.Instance.playerData.timeShowNoAds24h = Helper.DateTimeToString(DateTime.Now);
            DataManager.Instance.Save();
        }

        timeEnd = Helper.StringToDate(DataManager.Instance.playerData.timeShowNoAds24h).AddDays(DAY_DURATION);

        StartCountdown();
    }

    public override bool IsEnded()
    {
        if (string.IsNullOrEmpty(DataManager.Instance.playerData.timeShowNoAds24h))
        {
            return false;
        }
        else
        {
            timeEnd = Helper.StringToDate(DataManager.Instance.playerData.timeShowNoAds24h).AddDays(DAY_DURATION);

            return DateTime.Now > timeEnd;
        }
    }

    public override bool HasPurchased()
    {
        return IAPManager.HasPurchasedPack(ShopItemKey.NoAds24h);
    }
}
