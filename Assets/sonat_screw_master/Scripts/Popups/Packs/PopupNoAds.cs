using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupNoAds : PopupBase
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private SkeletonGraphic noAdsAnim;
    [SerializeField] private TextMeshProUGUI costText;

    private void Awake()
    {
        closeBtn.onClick.AddListener(() =>
        {
            SonatTracking.LogShowUI("user", DataManager.Instance.playerData.saveLevelData.currentLevel, "no_ads", GameManager.Instance.eScreen.ToString(), "close");

            Hide(GameManager.Instance.GameState == GameState.Ingame, false, true);
        });

        buyBtn.onClick.AddListener(() =>
        {
            
        });
    }

    private void Start()
    {
    }

    public override void Hide(bool isSetPlaying, bool isKeepingMask, bool isFade)
    {
        base.Hide(isSetPlaying, isKeepingMask, isFade);

        noAdsAnim.DOFade(0, PopupConfig.TIME_FADE_MASK);
    }

    public override void Show(bool isFadeMask, bool isSetPause = true)
    {
        base.Show(isFadeMask, isSetPause);

        noAdsAnim.DOFade(1, PopupConfig.TIME_FADE_MASK);
    }
}
