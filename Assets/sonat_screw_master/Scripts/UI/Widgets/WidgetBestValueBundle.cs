using System;

public class WidgetBestValueBundle : WidgetTimer
{
    private const int DAY_DURATION = 3;

    protected override void Awake()
    {
        button.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon("best_value_bundle", DataManager.Instance.playerData.saveLevelData.currentLevel, "Home");

            PopupManager.Instance.ShowBestValueBundle(false);
        });
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (string.IsNullOrEmpty(DataManager.Instance.playerData.timeShowBestValueBundle))
        {
            DataManager.Instance.playerData.timeShowBestValueBundle = Helper.DateTimeToString(DateTime.Now);
            DataManager.Instance.Save();
        }

        timeEnd = Helper.StringToDate(DataManager.Instance.playerData.timeShowBestValueBundle).AddDays(DAY_DURATION);

        StartCountdown();
    }

    public override bool IsEnded()
    {
        if (string.IsNullOrEmpty(DataManager.Instance.playerData.timeShowBestValueBundle))
        {
            return false;
        }
        else
        {
            timeEnd = Helper.StringToDate(DataManager.Instance.playerData.timeShowBestValueBundle).AddDays(DAY_DURATION);

            return DateTime.Now > timeEnd;
        }
    }

    public override bool HasPurchased()
    {
        return IAPManager.HasPurchasedPack(ShopItemKey.BestValueBundle);
    }
}
