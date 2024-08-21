using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using Newtonsoft.Json;

public class PopupWeekend : PopupForce
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private SkeletonGraphic anim;

    private DateTime timeEnd;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
        });

        buyBtn.onClick.AddListener(() =>
        {
            
        });
    }

    private void Start()
    {
#if UNITY_EDITOR
        costText.text = costText.text = "$" + UIManager.Instance.UIShop.GetShopPackData(ShopItemKey.WeekendSale).costValue;
#else
        //costText.text = Kernel.Resolve<BasePurchaser>().GetPriceText((int)ShopItemKey.WeekendSale);
#endif
    }

    private void OnEnable()
    {
        timeEnd = Helper.Next(DateTime.Today, DayOfWeek.Monday);

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

        value = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);

        timerText.text = value;
    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        base.Hide(isSetPlaying, isKeepingMask, isFade);

        anim.DOFade(0, PopupConfig.TIME_FADE_MASK);
    }

    public override void Show(bool isFadeMask, bool isAuto = false, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        anim.DOFade(1, PopupConfig.TIME_FADE_MASK);
    }

    public override void LoadConfig()
    {
        base.LoadConfig();

    }
}
