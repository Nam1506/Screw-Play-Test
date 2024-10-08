using Spine.Unity;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;

public class PopupExclusive : PopupForce
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
        costText.text = costText.text = "$" + UIManager.Instance.UIShop.GetShopPackData(ShopItemKey.ExclusiveDeal).costValue;
#else
        //costText.text = Kernel.Resolve<BasePurchaser>().GetPriceText((int)ShopItemKey.ExclusiveDeal);
#endif
    }

    private void OnEnable()
    {
        timeEnd = Helper.Next(DateTime.Today, DayOfWeek.Saturday);

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

    }

    public override void Show(bool isFadeMask, bool isAuto = false, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        if (anim != null)
        {
            anim.Initialize(true);

            anim.AnimationState.SetAnimation(0, "Appear", false);
            anim.AnimationState.AddAnimation(0, "Idle", true, 0);
        }
        else
        {
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        closeBtn.GetComponent<TMP_Text>().alpha = 0f;
        closeBtn.GetComponent<TMP_Text>().DOFade(1f, 0.5f).SetDelay(1f);
    }

    public override void LoadConfig()
    {
        base.LoadConfig();

    }
}
