using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupValueBundle : PopupForce
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private SkeletonGraphic anim;

    private DateTime timeEnd;

    private const int DAY_DURATION = 2;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            Hide(false, false, true);
        });

        buyBtn.onClick.AddListener(() =>
        {
           
        });
    }

    private void Start()
    {
#if UNITY_EDITOR
        costText.text = costText.text = "$" + UIManager.Instance.UIShop.GetShopPackData(ShopItemKey.ValueBundle).costValue;
#else
        //costText.text = Kernel.Resolve<BasePurchaser>().GetPriceText((int)ShopItemKey.ValueBundle);
#endif
    }

    private void OnEnable()
    {
        timeEnd = Helper.StringToDate(DataManager.Instance.playerData.timeShowValueBundle).AddDays(DAY_DURATION);

        StartCoroutine(IeCountdown());

        anim.AnimationState.SetAnimation(0, "Appear", false);
        anim.AnimationState.AddAnimation(0, "Idle", true, 0);
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

        anim.DOFade(0, PopupConfig.TIME_FADE_MASK);
    }

    public override void Show(bool isFadeMask, bool isAuto = false, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        anim.DOFade(1, PopupConfig.TIME_FADE_MASK);
    }
}
