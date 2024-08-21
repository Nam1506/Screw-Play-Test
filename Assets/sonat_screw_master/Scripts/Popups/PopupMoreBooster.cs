using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMoreBooster : PopupForce
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image titleImage;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image boosterIcon;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button watchAdsBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button freeBtn;
    [SerializeField] private BoosterSO boosterSO;

    [SerializeField] private List<BannerBundle> bannerBundles;

    private int cost;
    private EBoosterType type;
    private bool isPreBooster;

    public bool IsPreBooster => isPreBooster;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel,
                isPreBooster ? "pre_booster" : "more_booster", "Ingame", "close");

            Hide(true, false, true);
        });
        buyBtn.onClick.AddListener(() =>
        {
            if (DataManager.Instance.playerData.coins < cost)
            {
                PopupManager.Instance.PrePopups.Add(this);

                PopupManager.Instance.ShowNotiAlert("Not enough coins");

                bool isPreBooster1 = isPreBooster;

                Hide(false, false, false);

                UIManager.Instance.ShowShop();

                isPreBooster = isPreBooster1;
            }
            else
            {
                DataManager.Instance.ChangeCoins(-cost);


                SonatTracking.LogBuyShopItem(isPreBooster ? "pre_booster" : "booster", type.ToString(), 1, "coin", DataManager.Instance.IsFirstBuy());
                SonatTracking.LogSpendCurrency("coin", "currency", cost, "Ingame", isPreBooster ? "pre_booster" : "booster", type.ToString());

                BoosterManager.Instance.AddBooster(type, 1, true, IsPreBooster);

                //if (isPreBooster)
                //{
                DOVirtual.DelayedCall(PopupConfig.TIME_FADE_MASK, () =>
                {
                    BoosterManager.Instance.ForceUse(type);
                });
                //}

                Hide(true, false, true);
            }
        });
        watchAdsBtn.onClick.AddListener(() =>
        {
            SonatTracking.PlayVideoAds(type.ToString(), () =>
            {
                BoosterManager.Instance.AddBooster(type, 1, true, isPreBooster);

                DOVirtual.DelayedCall(PopupConfig.TIME_FADE_MASK, () =>
                {
                    BoosterManager.Instance.ForceUse(type);
                });

                //if (isPreBooster)
                //{
                //    BoosterManager.Instance.boosterHole.ForceUse();
                //}

            }, isPreBooster ? "pre_booster" : "booster", "Ingame");

            Hide(true, false, true);
        });
        freeBtn.onClick.AddListener(() =>
        {
            DOVirtual.DelayedCall(PopupConfig.TIME_FADE_MASK, () =>
            {
                BoosterManager.Instance.boosterHole.ForceUse(true);
            });

            Hide(true, false, true);
        });
    }

    public void SetBooster(EBoosterType type)
    {
        this.type = type;

        titleImage.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);

        foreach (var booster in boosterSO.boosters)
        {
            if (booster.type == type)
            {
                if (isPreBooster)
                {
                    cost = booster.cost - 10;
                    description.text = "Add 1 hole to beat levels more easily";
                    titleText.text = "LEVEL " + DataManager.Instance.playerData.saveLevelData.currentLevel;
                    titleText.gameObject.SetActive(true);
                }
                else
                {
                    cost = booster.cost;
                    description.text = booster.description;
                    titleImage.sprite = booster.titleIcon;
                    titleImage.gameObject.SetActive(true);
                    titleImage.SetNativeSize();
                }

                costText.text = cost.ToString();
                boosterIcon.sprite = booster.bigIcon;
                //boosterIcon.SetNativeSize();

                return;
            }
        }
    }

    public void ShowPreBooster()
    {
        isPreBooster = true;

        SetBooster(EBoosterType.AddHole);

        Show(true, true);
    }

    private void OnEnable()
    {
        if (DataManager.Instance.playerData.saveLevelData.currentLevel == GameDefine.LEVEL_SHOW_PREBOOSTER && isPreBooster)
        {
            freeBtn.gameObject.SetActive(true);
            watchAdsBtn.gameObject.SetActive(false);
            buyBtn.gameObject.SetActive(false);
            closeBtn.gameObject.SetActive(false);

            //UIManager.Instance.SendCurrencyToFront();
            //UIManager.Instance.ShowCoin();
        }
        else
        {
            freeBtn.gameObject.SetActive(false);
            watchAdsBtn.gameObject.SetActive(true);
            buyBtn.gameObject.SetActive(true);
            closeBtn.gameObject.SetActive(true);

            UIManager.Instance.SendCurrencyToFront();
            UIManager.Instance.ShowCoin();
        }
    }

    //private void OnDisable()
    //{
    //    isPreBooster = false;
    //}

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        base.Hide(isSetPlaying, isKeepingMask, isFade);

        UIManager.Instance.HideCoin(false);

        isPreBooster = false;
    }

    public override void Show(bool isFadeMask, bool isAuto, bool isSetPause = true)
    {
        base.Show(isFadeMask, isAuto, isSetPause);

        //UIManager.Instance.SendCurrencyToFront();
        //UIManager.Instance.ShowCoin();

        if (isPreBooster)
        {
            CheckShowBundle();
        }
        else
        {
            ShowBundle(ShopItemKey.BoosterOffer);
        }
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        UIManager.Instance.ShowCoin();
    }

    public void ShowBundle(ShopItemKey key)
    {
        foreach (var bundle in bannerBundles)
        {
            if (bundle.key == key)
            {
                bundle.gameObject.SetActive(true);
            }
            else
            {
                bundle.gameObject.SetActive(false);
            }
        }
    }

    private void CheckShowBundle()
    {
        if (DataManager.Instance.playerData.saveLevelData.currentLevel <= 30)
        {
            if (PopupManager.Instance.CheckShowValueBundle(false))
            {
                ShowBundle(ShopItemKey.ValueBundle);
            }
            else if (PopupManager.Instance.CheckShowBestValueBundle(false))
            {
                ShowBundle(ShopItemKey.BestValueBundle);
            }
            else if (UIManager.Instance.IsShowPlusOffer())
            {
                ShowBundle(ShopItemKey.PlusOffer);
            }
            else if (UIManager.Instance.IsShowExclusive())
            {
                ShowBundle(ShopItemKey.ExclusiveDeal);
            }
            else if (UIManager.Instance.IsShowWeekendSale())
            {
                ShowBundle(ShopItemKey.WeekendSale);
            }
            else
            {
                ShowBundle(ShopItemKey.BoosterOffer);
            }
        }
        else
        {
            if (UIManager.Instance.IsShowPlusOffer())
            {
                ShowBundle(ShopItemKey.PlusOffer);
            }
            else if (UIManager.Instance.IsShowExclusive())
            {
                ShowBundle(ShopItemKey.ExclusiveDeal);
            }
            else if (UIManager.Instance.IsShowWeekendSale())
            {
                ShowBundle(ShopItemKey.WeekendSale);
            }
            else
            {
                ShowBundle(ShopItemKey.BoosterOffer);
            }
        }
    }
}
