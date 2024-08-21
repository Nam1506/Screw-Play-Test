using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;

public class PopupJustFun : PopupForce
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private SkeletonGraphic noAdsAnim;

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
        costText.text = costText.text = "$" + UIManager.Instance.UIShop.GetShopPackData(ShopItemKey.NoAdsJustFun).costValue;
#else
        //costText.text = Kernel.Resolve<BasePurchaser>().GetPriceText((int)ShopItemKey.NoAdsJustFun);
#endif
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        UIManager.Instance.SendCurrencyToBack();

        base.Show(isFadeMask, isSetPause);

        noAdsAnim.DOFade(1, PopupConfig.TIME_FADE_MASK);
    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        UIManager.Instance.SendCurrencyToFront();

        base.Hide(isSetPlaying, isKeepingMask, isFade);

        noAdsAnim.DOFade(0, PopupConfig.TIME_FADE_MASK);
    }

    public override void LoadConfig()
    {
        base.LoadConfig();

    }
}
