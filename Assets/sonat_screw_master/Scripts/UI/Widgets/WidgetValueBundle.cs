using System;

public class WidgetValueBundle : WidgetTimer
{
    private const int DAY_DURATION = 2;

    protected override void Awake()
    {
        button.onClick.AddListener(() =>
        {
            SonatTracking.ClickIcon("value_bundle", DataManager.Instance.playerData.saveLevelData.currentLevel, "Home");

            PopupManager.Instance.ShowValueBundle(false);
        });
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (string.IsNullOrEmpty(DataManager.Instance.playerData.timeShowValueBundle))
        {
            DataManager.Instance.playerData.timeShowValueBundle = Helper.DateTimeToString(DateTime.Now);
            DataManager.Instance.Save();
        }

        timeEnd = Helper.StringToDate(DataManager.Instance.playerData.timeShowValueBundle).AddDays(DAY_DURATION);

        StartCountdown();
    }

    public override bool IsEnded()
    {
        if (string.IsNullOrEmpty(DataManager.Instance.playerData.timeShowValueBundle))
        {
            return false;
        }
        else
        {
            timeEnd = Helper.StringToDate(DataManager.Instance.playerData.timeShowValueBundle).AddDays(DAY_DURATION);

            return DateTime.Now > timeEnd;
        }
    }

    public override bool HasPurchased()
    {
        return IAPManager.HasPurchasedPack(ShopItemKey.ValueBundle);
    }
}
