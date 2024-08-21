using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupNoAds24h : PopupForce
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private SkeletonGraphic noAdsAnim;
    [SerializeField] private TextMeshProUGUI oldCost;
    [SerializeField] private TextMeshProUGUI newCost;
    [SerializeField] private TextMeshProUGUI timerText;

    private DateTime timeEnd;

    private const int DAY_DURATION = 2;

    private const string NOADS_ANIM_NAME = "animation";

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "no_ads_24h", 
                GameManager.Instance.eScreen.ToString(), "close");

            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
        });
        buyBtn.onClick.AddListener(() =>
        {
            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);

           
        });
    }

    private void Start()
    {
#if UNITY_EDITOR
        oldCost.text = "$" + UIManager.Instance.UIShop.GetShopPackData(ShopItemKey.NoAds).costValue;
        newCost.text = "$" + UIManager.Instance.UIShop.GetShopPackData(ShopItemKey.NoAds24h).costValue;
#else
        //oldCost.text = Kernel.Resolve<BasePurchaser>().GetPriceText((int)ShopItemKey.NoAds);
        //newCost.text = Kernel.Resolve<BasePurchaser>().GetPriceText((int)ShopItemKey.NoAds24h);
#endif
    }

    private void OnEnable()
    {
        noAdsAnim.AnimationState.SetAnimation(0, NOADS_ANIM_NAME, true);

        closeBtn.transform.localScale = Vector3.zero;
        closeBtn.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.4f).OnComplete(() =>
        {
            closeBtn.transform.localScale = Vector3.one;
        });

        timeEnd = Helper.StringToDate(DataManager.Instance.playerData.timeShowNoAds24h).AddDays(DAY_DURATION);

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

        Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
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

        timerText.text = "Ends in: " + value;
    }


    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        base.Hide(isSetPlaying, isKeepingMask, isFade);

        noAdsAnim.DOFade(0, PopupConfig.TIME_FADE_MASK);
    }

    public override void Show(bool isFadeMask, bool isAuto = false, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        noAdsAnim.DOFade(1, PopupConfig.TIME_FADE_MASK);
    }
}
