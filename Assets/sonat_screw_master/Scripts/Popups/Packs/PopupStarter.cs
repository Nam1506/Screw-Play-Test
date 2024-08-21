using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;

public class PopupStarter : PopupForce
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private SkeletonGraphic anim;

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
        costText.text = costText.text = "$" + UIManager.Instance.UIShop.GetShopPackData(ShopItemKey.StarterBundle).costValue;
#else
        //costText.text = Kernel.Resolve<BasePurchaser>().GetPriceText((int)ShopItemKey.StarterBundle);
#endif
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
